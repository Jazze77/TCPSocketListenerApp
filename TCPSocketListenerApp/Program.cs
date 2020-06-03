using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPSocketListenerApp
{
    class Program
    {
        public class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime TimeStamp { get; set; }
        }

        public static string data = null;
        public static string result = null;
        public static void StartListening()
        {
            // Data buffer for incoming data.  
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.  
            // Dns.GetHostName returns the name of the
            // host running the application.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
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

                    Console.WriteLine("Tämä on testi! <EOF>");
                    // Program is suspended while waiting for an incoming connection.  
                    Socket handler = listener.Accept();
                    data = null;

                    // An incoming connection needs to be processed.  
                    while (true)
                    {
                       
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.Default.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            // Remove <EOF>
                            result = data.Remove(data.Length - 5); 
                            break;
                        }
                    }

                    // Show the data on the console.                      
                    Console.WriteLine("Text received : {0}", result);

                    List<Person> mylist =
                        JsonConvert.DeserializeObject<List<Person>>(result);

                    foreach(Person iter in mylist)
                    {
                        Console.WriteLine(iter.FirstName + " " + iter.LastName + " " +iter.TimeStamp);
                        iter.FirstName += "qqq";
                        iter.LastName += "qqq";
                        iter.TimeStamp = DateTime.Now;
                    }

                    string jsonmessage = JsonConvert.SerializeObject(mylist);

                    // Echo the data back to the client.  
                    byte[] msg = Encoding.Default.GetBytes(jsonmessage);

                    handler.Send(msg);
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
            Console.ReadLine();

            return 0;
        }
    }
}
