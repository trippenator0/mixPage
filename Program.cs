using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
// This program is for paging from POS terminals
namespace HelloWorld
{
    class Hello
    {
        static public void usage()
        {
            Console.WriteLine("usage: pingback [-l local-ip]");
			Console.WriteLine(" [-r to recieve] [-s destination] [(-f, -h) ");
            Console.WriteLine();
            Console.WriteLine("Available options:");
			Console.WriteLine("     -l local-ip         Local IP address to bind to");
            Console.WriteLine("     -r                  Receive UDP data");
            Console.WriteLine("     -s destination      Send UDP data to given destination");
			Console.WriteLine("     {-f,-h}             Request assistance at Front or Havana Respectively\n");
        }
        static void Main(string[] args)
        {
            ArrayList multicastGroups = new ArrayList();
            IPAddress localAddress = IPAddress.Any, destAddress = null;
			localAddress = IPAddress.Parse("127.0.0.1");
            ushort portNumber = 9998;
			string sendString = "NULL";
            bool udpSender = false;
			int  i;
            if(args.Length < 1)
				usage();
			//Get local interface IP addr
			IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
			localAddress = localIPs[0];
            // Parse the command line
            for (i = 0; i < args.Length; i++)
            {
                try
                {
                    if ((args[i][0] == '-') || (args[i][0] == '/'))
                    {

                        switch (Char.ToLower(args[i][1]))
                        {
                            case 'm':       // Multicast groups to join
                                multicastGroups.Add(IPAddress.Parse(args[++i]));
                                break;

                            case 'l':       // Local interface to bind to
								
                                localAddress = IPAddress.Parse(args[++i]);
                                break;

                            case 'r':       // Indicates UDP receiver

								udpSender = false;
                                break;

                            case 's':       // Indicates UDP sender as well as the destination address

                                udpSender = true;
                                destAddress = IPAddress.Parse(args[++i]);
                                break;

                            case 'f': 		//Indicates request came from/for the front

                              	sendString = "Front";
								break;

                            case 'h':		//Indicates request came from/for Havana

                                sendString = "Havana";
								break;

							case 't':		//Indicates request came from/for the front terminal

								sendString = "Enterance";
								break;

                            default:

                                usage();
								return;
                        }
			
                    }

                }
				 
                catch
                {
                    //usage();
					return;

                }
            }

			UdpClient udpSocket = null;

            try
            {
                Console.WriteLine("creating null socket...");

                if (udpSender == false)

                    udpSocket = new UdpClient(new IPEndPoint(localAddress, portNumber));

                else

                    udpSocket = new UdpClient(new IPEndPoint(localAddress, 0));



                byte[] byData = System.Text.Encoding.ASCII.GetBytes(sendString);
   				 

                Console.WriteLine("Joining any multicast groups specified...");

                for (i = 0; i < multicastGroups.Count; i++)
                {

                    if (localAddress.AddressFamily == AddressFamily.InterNetwork)
                    {
                        udpSocket.JoinMulticastGroup((IPAddress)multicastGroups[i]);

                    }

                    else if (localAddress.AddressFamily == AddressFamily.InterNetworkV6)
                    {


                        udpSocket.JoinMulticastGroup((int)((IPAddress)multicastGroups[i]).ScopeId, (IPAddress)multicastGroups[i]);

                    }

                }


                if (udpSender == true)
                {

                    udpSocket.Connect(destAddress, portNumber);
                    Console.WriteLine("Connect() is OK...");

                }



                if (udpSender == true)
                {
                    //send
					udpSocket.Send(byData,byData.Length);
				}
                else
                {
					//recieve
                    IPEndPoint senderEndPoint = new IPEndPoint(localAddress, 0);

           
					string machineName;
                    while (true)
                    {
					
                      	Byte[]  receiveBuffer = udpSocket.Receive(ref senderEndPoint);

					  	string recieveStr = System.Text.Encoding.Default.GetString(receiveBuffer);
			            switch(senderEndPoint.ToString()) {

						case "10.0.0.11":
							machineName = "Term1 Hallway.";
							break;
						case "10.0.0.13":
							machineName = "Term3 Front far.";
							break;
						case "10.0.0.12":
							machineName = "Term2.";
							break;
						case "10.0.0.14":
							machineName = "Term4.";
							break;
						case "10.0.0.15":
							machineName = "Term5.";
							break;
						case "10.0.0.16":
							machineName = "Term6.";
							break;	
						default:
							machineName = "Terminal Unknown.";
							break;
						}
						DateTime dt = DateTime.UtcNow.ToLocalTime();

						Console.WriteLine("Page from {0}, message: {1} at {2}\n",  machineName, recieveStr, dt);



                        if (receiveBuffer.Length == 0)

                            break;

                    }

                }

            }

            catch (SocketException err)
            {

                Console.WriteLine("Socket error occurred: {0}", err.Message);

                Console.WriteLine("Stack: {0}", err.StackTrace);

            }

            finally
            {

                if (udpSocket != null)
                {
                  
					Console.WriteLine("Successful, cleaning up..");

                    for (i = 0; i < multicastGroups.Count; i++)
                    {

                        if (localAddress.AddressFamily == AddressFamily.InterNetwork)
                        {

                            udpSocket.DropMulticastGroup((IPAddress)multicastGroups[i]);

                        }

                        else if (localAddress.AddressFamily == AddressFamily.InterNetworkV6)
                        {

							 
                            udpSocket.DropMulticastGroup(

                                (IPAddress)multicastGroups[i],

                                (int)((IPAddress)multicastGroups[i]).ScopeId

                                );

                        }

                    }


                    Console.WriteLine("Success: closing the socket.");
                    udpSocket.Close();

                }

            }

        }
    }
}