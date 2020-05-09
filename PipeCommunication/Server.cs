using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeCommunication
{
    public class Server
    {
        private string status = string.Empty;
        private string serverName = string.Empty;
        private static object sync = new object();
        private NamedPipeServerStream server;
        private StreamReader reader;
        private StreamWriter writer;
        private bool working;

        public event EventHandler Started;
        public event EventHandler Broken;
        public event EventHandler AwaitingClient;
        public event EventHandler ClientConnected;
        public event EventHandler ClientDisconnected;
        
        public Server(string serverName)
        {
            this.serverName = serverName;
        }

        public void Start()
        {
            Task.Factory.StartNew(() =>
            {
                Proceed();
            });
        }
        public void Stop()
        {
            working = false;
            server.WaitForPipeDrain();
            server.Disconnect();
            server.Close();
            server.Dispose();
        }
        private void OnStarted()
        {
            status = ServerState.Unknown.ToString();
            Started?.Invoke(this, EventArgs.Empty);
        }
        private void OnBreak()
        {
            status = ServerState.Failure.ToString();
            Broken?.Invoke(this, EventArgs.Empty);
        }
        private void OnAwaitingClient()
        {
            AwaitingClient?.Invoke(this, EventArgs.Empty);
        }
        private void OnClientConnected()
        {
            ClientConnected?.Invoke(this, EventArgs.Empty);
        }
        private void OnClientDisconnected()
        {
            ClientDisconnected?.Invoke(this, EventArgs.Empty);
        }
        private void Proceed()
        {
            server = new NamedPipeServerStream(serverName);
            OnAwaitingClient();
            server.WaitForConnection();
            if (!server.IsConnected)
                return;

            OnClientConnected();
            working = true;

            reader = new StreamReader(server);
            writer = new StreamWriter(server);

            while (working)
            {
                var command = reader.ReadLine();
                if (Respond(command) != 0)
                    break;
            }
        }
        private int Respond(string command)
        {
            switch (command)
            {
                case "start": lock (sync) writer.WriteLine("ACK"); OnStarted(); break;
                case "status": lock (sync) writer.WriteLine(status); break;
                case "break": lock (sync) writer.WriteLine("ACK"); OnBreak(); break;
                case "disconnect": lock (sync) ClientDisconnects(); return 1;
                case "kill": lock (sync) writer.WriteLine("ACK"); break;   //TODO: EXTEND KILL METHOD!
                default: lock (sync) writer.WriteLine("NACK"); break;
            }
            writer.Flush();
            return 0;
        }
        public void ChangeStatus(ServerState status)
        {
            if (status == ServerState.Unknown)
                return;
            lock (sync)
                this.status = status.ToString();
        }
        private void ClientDisconnects()
        {
            server.WaitForPipeDrain();
            reader.DiscardBufferedData();
            writer.Flush();
            server.Close();
            server.Dispose();
            OnClientDisconnected();
            //Proceed();
            Start();
        }
    }
}
