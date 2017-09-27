using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;

namespace Multi_Threaded_TCP
{

    class TcpServer
    {
        private TcpListener _server;
        private Boolean _isRunning;

        public TcpServer(int port)
        {
            _server = new TcpListener(IPAddress.Any, port);
            _server.Start();

            _isRunning = true;

            LoopClients();
        }

        public void LoopClients()
        {
            while (_isRunning)
            {
                // wait for client connection
                TcpClient newClient = _server.AcceptTcpClient();

                // client found.
                // create a thread to handle communication
                Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                t.Start(newClient);
            }
        }

        public void HandleClient(object obj)
        {
            // retrieve client from parameter passed to thread
            TcpClient client = (TcpClient)obj;

            // sets two streams
            StreamWriter sWriter = new StreamWriter(client.GetStream(), Encoding.ASCII);
            StreamReader sReader = new StreamReader(client.GetStream(), Encoding.ASCII);
            HttpListener httpListen = new HttpListener();
            // you could use the NetworkStream to read and write, 
            // but there is no forcing flush, even when requested

            Boolean bClientConnected = true;
            String sData = null;

            while (bClientConnected)
            {
                httpListen.Start();
                // reads from stream
                sData = sReader.ReadLine();
                httpListen.Prefixes.Add(sData);
                HttpListenerContext context = httpListen.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
                sWriter.Write(responseString);
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                //// Get a response stream and write the response to it.
                //response.ContentLength64 = buffer.Length;
                //System.IO.Stream output = response.OutputStream;
                //output.Write(buffer, 0, buffer.Length);
                //// You must close the output stream.
                //output.Close();

                // shows content on the console.
                Console.WriteLine("Client: " + sData);

                if (sData == "close")
                {
                    Console.WriteLine("closing");
                    client.Close();
                    break;
                }

                // to write something back.
                // sWriter.WriteLine("Meaningfull things here");
                // sWriter.Flush();
            }
            httpListen.Stop();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Multi-Threaded TCP Server Demo");
            TcpServer server = new TcpServer(5555);
        }
    }
}
