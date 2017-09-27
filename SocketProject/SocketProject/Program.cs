using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Globalization;

public class SynchronousSocketListener{

    // Incoming data from the client.
    public static string data = null;

    public static void StartListening()
    {
        // Data buffer for incoming data.
        byte[] bytes = new Byte[1024];
        DateTime localTime = DateTime.Now;
        DateTime utcTime = DateTime.UtcNow;
        DateTime localClientTime = DateTime.
        string CultureName = "de-DE";
        var CultureInfo = new CultureInfo(CultureName);
        string msgUtcTime = "UTC time: " + utcTime.ToString(CultureInfo) + "\r\n";
        string msgLocalTime = "local time: " + localTime.ToString(CultureInfo) + "\r\n";

        // Establish the local endpoint for the socket.
        // Dns.GetHostName returns the name of the 
        // host running the application.
        IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

        // Create a TCP/IP socket.
        Socket listener = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

        // Bind the socket to the local endpoint and 
        // listen for incoming connections.
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(10);

            // Start listening for connections.
            while (true)
            {
                Console.WriteLine("Waiting for a connection...");
                // Program is suspended while waiting for an incoming connection.
                Socket handler = listener.Accept();
                data = null;
                string msgStr = "Send 'UTC' to get UTC-Time and 'local' to get local time \r\n";
                byte[] msg = Encoding.ASCII.GetBytes(msgStr);
                handler.Send(msg);

                // An incoming connection needs to be processed.
                while (true)
                {
                    bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (data.IndexOf("UTC") > -1)
                    {
                        byte[] msgUtc = Encoding.ASCII.GetBytes(msgUtcTime);
                        handler.Send(msgUtc);
                        break;
                    }
                    else if (data.IndexOf("local") > -1)
                    {
                        byte[] msgLocal = Encoding.ASCII.GetBytes(msgLocalTime);
                        handler.Send(msgLocal);
                        break;
                    }
                    else if (data.IndexOf("close") > -1)
                    {
                        break;
                    }
                }

                // Show the data on the console.
                Console.WriteLine("Text received : {0}", data);

                // Echo the data back to the client.
                //byte[] msg = Encoding.ASCII.GetBytes(data);
                //string msgLocalTime = "local Time: " + localTime.ToString(CultureInfo) + "\n";
                //string msgUtcTime = "UTC time: " + utcTime.ToString(CultureInfo) + "\n";
                //byte[] msgLocal = Encoding.ASCII.GetBytes(msgLocalTime);
                //byte[] msgUtc = Encoding.ASCII.GetBytes(msgUtcTime);
                //handler.Send(msg);
                //handler.Send(msgLocal);
                //handler.Send(msgUtc);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        Console.WriteLine("\nPress ENTER to continue...");
        Console.Read();

    }

    public static int Main(String[] args)
    {
        StartListening();
        return 0;
    }
}