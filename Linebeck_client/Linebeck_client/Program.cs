using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;

public class SimpleTcpClient
{

    public static string GetFixedLengthString(string input, int length)
    {
        input = input ?? string.Empty;
        input = input.Length > length ? input.Substring( 0, length ) : input;
        return string.Format( "{0,-" + length + "}", input );
    }
    public static byte[] GetBytesFromFile(string fullFilePath) //loads song to byte array
    {
        FileStream fs = File.OpenRead(fullFilePath);
        try
        {
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
            fs.Close();
            return bytes;
        }
        finally
        {
            fs.Close();
        }
    }

    public static void Main()
    {
        byte[] data = new byte[1024];
        string input, stringData;
        IPEndPoint ipep = new IPEndPoint(
                        IPAddress.Parse("127.0.0.1"), 8009);

        Socket server = new Socket(AddressFamily.InterNetwork,
                       SocketType.Stream, ProtocolType.Tcp);

        try
        {
            server.Connect(ipep);
        }
        catch (SocketException e)
        {
            Console.WriteLine("Unable to connect to server.");
            Console.WriteLine(e.ToString());
            return;
        }


        int recv = server.Receive(data);
        stringData = Encoding.ASCII.GetString(data, 0, recv);
        Console.WriteLine(stringData);
        {
            input = Console.ReadLine();
            if (!File.Exists(input)) // so, we can send commands, if not a song
            {
                server.Send(Encoding.ASCII.GetBytes(GetFixedLengthString(Convert.ToString(input.Length), 12)));
                server.Send(Encoding.ASCII.GetBytes(input));
            }
            else
            {
                byte[] fileToSend = GetBytesFromFile(input);

                string filelength = GetFixedLengthString(Convert.ToString(fileToSend.Length), 12);

                server.Send(Encoding.ASCII.GetBytes(filelength));

                server.Send(fileToSend);
            }
            data = new byte[1024];
            recv = server.Receive(data);
            stringData = Encoding.ASCII.GetString(data, 0, recv);
            Console.WriteLine(stringData);
            if (input == "gcq")
            {
                data = new byte[1000000];
                recv = server.Receive(data);
                stringData = Encoding.ASCII.GetString(data, 0, recv);
                string[] queue = stringData.Split('\r');
                foreach (string str in queue)
                {
                    Console.WriteLine(str);
                }
            }

        }

        Console.WriteLine("Disconnecting from server...");
        server.Shutdown(SocketShutdown.Both);
        server.Close();
        Console.ReadKey();
    }
}