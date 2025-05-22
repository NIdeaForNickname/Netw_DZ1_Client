using System.Diagnostics;

namespace Netw_DZ1_client;

using System.Net;
using System.Net.Sockets;
using System.Text;
 
class Client
{
    private const int DEFAULT_BUFLEN = 512;
    private const string DEFAULT_PORT = "27015";
 
    static void Main()
    {
        Console.Title = "CLIENT SIDE";
        try
        {
            var ipAddress = IPAddress.Loopback; 
            var remoteEndPoint = new IPEndPoint(ipAddress, int.Parse(DEFAULT_PORT));
            var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            try {
                clientSocket.Connect(remoteEndPoint);
            }
            // Launch server if it's offline 
            // Linux only
            catch (Exception e){
                if (e.Message.Contains("Connection refused"))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "konsole",
                        Arguments = "-e bash -c \"dotnet run --project ~/RiderProjects/Netw_DZ1_server/Netw_DZ1_server; exec bash\"",
                    });
                    Thread.Sleep(5000);
                    clientSocket.Connect(remoteEndPoint);
                }
            }
 
            while (true)
            {
                Console.Write("Enter request:");
                var message = Console.ReadLine();
                if (message == "exit")
                {
                    clientSocket.Shutdown(SocketShutdown.Send);
                    clientSocket.Close();
                    Console.WriteLine("Connection closed.");
                    break;
                }
                else
                {
                    byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                    clientSocket.Send(messageBytes);
                    Console.WriteLine($"Sent: {message}");
 
                    var buffer = new byte[DEFAULT_BUFLEN];
                    int bytesReceived = clientSocket.Receive(buffer);
                    string response = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                    Console.WriteLine($"Response: {response}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
    }
}