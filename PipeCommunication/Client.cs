using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeCommunication
{
    public class Client
    {
        StreamReader reader;
        StreamWriter writer;
        NamedPipeClientStream client;
        private string serverName;

        public Client(string serverName)
        {
            this.serverName = serverName;
        }

        public bool Connect(int timeout = 0)
        {
            client = new NamedPipeClientStream(serverName);
            if (timeout <= 0)
                client.Connect();
            else
                client.Connect(timeout);

            if (!client.IsConnected)
                return false;

            reader = new StreamReader(client);
            writer = new StreamWriter(client);

            return true;
        }
        public bool Disconnect()
        {
            try
            {
                writer.WriteLine("disconnect");
                writer.Flush();
                writer.Close();
                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                client.Close();
                client.Dispose();
            }
            return true;
        }

        public bool Start()
        {
            writer.WriteLine("start");
            writer.Flush();
            if (reader.ReadLine() != "ACK")
                return false;
            return true;
        }

        public ServerState CheckStatus()
        {
            writer.WriteLine("status");
            writer.Flush();
            string temp = reader.ReadLine();

            if (Enum.TryParse(temp, out ServerState serverState))
                return serverState;
            else
                return ServerState.Unknown;
        }

        public bool Break()
        {
            writer.WriteLine("break");
            writer.Flush();
            if (reader.ReadLine() != "ACK")
                return false;
            return true;
        }
    }
}
