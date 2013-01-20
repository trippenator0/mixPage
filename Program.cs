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

            Console.WriteLine("Usage: pingback [-l local-ip]");

            Console.WriteLine(" [-r recieve] [-s destination] ");

            Console.WriteLine();

            Console.WriteLine("Available options:");

            Console.WriteLine("     -l local-ip         Local IP address to bind to");

            Console.WriteLine("     -r                  Receive UDP data");

            Console.WriteLine("     -s destination      Send UDP data to given destination");

            Console.WriteLine();

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

                            case 'f':
                              	sendString = "Front";
                            
								break;

                            case 'd':
                                sendString = "Dance";

                                break;

                            case 'h':
                                sendString = "Havana";
                                
								break;

							case 't':
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

                // Create an unconnected socket since if invoked as a receiver we don't necessarily

                //    want to associate the socket with a single endpoint. Also, for a sender socket

                //    specify local port of zero (to get a random local port) since we aren't receiving

                //    anything.

                Console.WriteLine("Creating connectionless socket...");

                if (udpSender == false)

                    udpSocket = new UdpClient(new IPEndPoint(localAddress, portNumber));

                else

                    udpSocket = new UdpClient(new IPEndPoint(localAddress, 0));



                // Join any multicast groups specified
                byte[] byData = System.Text.Encoding.ASCII.GetBytes(sendString);
   				 

                Console.WriteLine("Joining any multicast groups specified...");

                for (i = 0; i < multicastGroups.Count; i++)
                {

                    if (localAddress.AddressFamily == AddressFamily.InterNetwork)
                    {

                        // For IPv4 multicasting only the group is specified and not the

                        //    local interface to join it on. This is bad as on a multihomed

                        //    machine, the application won't really know which interface

                        //    it is joined on (and there is no way to change it via the UdpClient).

                        udpSocket.JoinMulticastGroup((IPAddress)multicastGroups[i]);

                    }

                    else if (localAddress.AddressFamily == AddressFamily.InterNetworkV6)
                    {

                        // For some reason, the IPv6 multicast join allows the local interface index

                        //    to be specified such that the application can join multiple groups on

                        //    any interface which is great (but lacking in IPv4 multicasting with the

                        //    UdpClient). IPv6 multicast groups should be specified with the scope id

                        //    when passed on the command line (e.g. fe80::1%4).

                        udpSocket.JoinMulticastGroup((int)((IPAddress)multicastGroups[i]).ScopeId, (IPAddress)multicastGroups[i]);

                    }

                }



                // If you want to send data with the UdpClient it must be connected -- either by

                //    specifying the destination in the UdpClient constructor or by calling the

                //    Connect method. You can call the Connect method multiple times to associate

                //    a different endpoint with the socket.

                if (udpSender == true)
                {

                    udpSocket.Connect(destAddress, portNumber);

                    Console.WriteLine("Connect() is O7K...");

                }



                if (udpSender == true)
                {

                    // Send the requested number of packets to the destination

                   /* Console.WriteLine("Sending the requested number of packets to the destination, Send()...");
                    byte[] sendBy = Encoding.ASCII.GetBytes(sendString);
                    foreach(byte element in sendBy) {
                        Console.WriteLine("{0} = {1}", element, (char)element);
                    }
                    for (i = 0; i < sendCount; i++)
                    {
                        
                        rc = udpSocket.Send(sendBy, sendBy.Length);

                        Console.WriteLine("Sent {0} bytes to {1}", rc, destAddress.ToString());

                    }
*
                    // Send a few zero length datagrams to indicate to the receive to exit. Put a short

                    //    sleep between them since UDP is unreliable and zero byte datagrams are really

                    //    fast (and the local stack can actually drop datagrams before they even make it

                    //    onto the wire).

                    Console.WriteLine("HAving some sleep, Sleep(250)...");
                

                    /*for (i = 0; i < 3; i++)
                    {

                        rc = udpSocket.Send(sendBuffer, 0);

                        System.Threading.Thread.Sleep(250);

                    }

                }*/
					udpSocket.Send(byData,byData.Length);
				}
                else
                {

                    IPEndPoint senderEndPoint = new IPEndPoint(localAddress, 0);

                    // Receive datagrams in a loop until a zero byte datagram is received.

                    //    Note the difference with the UdpClient in that the source address

                    //    is simply an IPEndPoint that doesn't have to be cast to and EndPoint

                    //    object as is the case with the Socket class.

                   // Console.WriteLine("Receiving datagrams in a loop until a zero byte datagram is received...");
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

						Console.WriteLine("Page from {0} message from {1} at {2} UTC",  machineName, recieveStr, dt);



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

                    // Clean things up by dropping any multicast groups we joined

                   // Console.WriteLine("Cleaning things up by dropping any multicast groups we joined...");
					Console.WriteLine("Successful, cleaning up..");

                    for (i = 0; i < multicastGroups.Count; i++)
                    {

                        if (localAddress.AddressFamily == AddressFamily.InterNetwork)
                        {

                            udpSocket.DropMulticastGroup((IPAddress)multicastGroups[i]);

                        }

                        else if (localAddress.AddressFamily == AddressFamily.InterNetworkV6)
                        {

                            // IPv6 multicast groups should be specified with the scope id when passed

                            //    on the command line

                            udpSocket.DropMulticastGroup(

                                (IPAddress)multicastGroups[i],

                                (int)((IPAddress)multicastGroups[i]).ScopeId

                                );

                        }

                    }

                    // Free up the underlying network resources

                    Console.WriteLine("Sucess: Closing the socket.");

                    udpSocket.Close();

                }

            }

        }
    }
}