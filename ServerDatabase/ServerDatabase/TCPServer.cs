// Andrew Franowicz 29297832
// Jason Heckard  84851006
// Nathan Stengel 28874701

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

using System.Diagnostics;

namespace SQLiteTest
{
    class TCPServer
    {
        static TcpListener listener;
        const int numberPlayers = 1;
        protected static PlayerSocket[] activePlayers = new PlayerSocket[numberPlayers];
        static Queue<string> movesMade;

        public static LoginDatabase dB;

        public TCPServer()
        { // start constructor

            dB = new LoginDatabase();

            listener = new TcpListener(4300);
            listener.Start();

            movesMade = new Queue<string>();

            dB.connectToDatabase();

            Console.Write("Press Enter to start the server:  ");
            Console.Read();

            // counting by t
            for (int t = 0; t < numberPlayers; ++t)
            { // start for loop 
                Socket sock = listener.AcceptSocket();
                Console.WriteLine("Player Socket has accepted the socket");

                NetworkStream nws = new NetworkStream(sock);
                //StreamReader reader = new StreamReader(nws);
                // StreamWriter writer = new StreamWriter(nws);
                //writer.AutoFlush = true;

                // Console.WriteLine("stream created");

                activePlayers[t] = new PlayerSocket(nws, sock);
                activePlayers[t].connected = true;

                // Console.WriteLine(" connected true");

                // string data = activePlayers[t].playerReader.ReadLine();
                // Console.Write(data);

                // creates a ReadThread, passes in the player value t
                ReadThread thread = new ReadThread(t);

                activePlayers[t].psThread = new Thread(new ThreadStart(thread.Service) );
                activePlayers[t].psThread.Start();
                Console.Write("bottom of for loop!\n");
            } // end for loop 




        } // end constructor

        public class ReadThread
        { // start readThread class 

            int client = -1;
            char delimiter = '$';
            string clientString;
            string responseData;

            public ReadThread(int newNumber)
            { // begin constructor
                client = newNumber;
                clientString = newNumber.ToString();
            } // end constructor

            public void Service() // for an individual operating thread
            { // begin service
                try
                { // start try

                    while (activePlayers[client].connected)
                    { // actual game loop for an individual player

                        getMessage(activePlayers[client].psnws);

                        //spliting the serverdata into instruction
                        string[] instruction = new string[11];
                        instruction[0] = responseData.Substring(0, responseData.IndexOf(delimiter));
                        responseData = responseData = responseData.Substring(responseData.IndexOf(delimiter) + 1); 

                        instruction[1] = responseData.Substring(0, responseData.IndexOf(delimiter));
                        responseData = responseData = responseData.Substring(responseData.IndexOf(delimiter) + 1); 

                        instruction[2] = responseData.Substring(0, responseData.IndexOf(delimiter));
                        responseData = responseData = responseData.Substring(responseData.IndexOf(delimiter) + 1); 

                        instruction[3] = responseData.Substring(0, responseData.IndexOf(delimiter));
                        responseData = responseData = responseData.Substring(responseData.IndexOf(delimiter) + 1);

                        Console.Write("Instruction [0] " + instruction[0] + " ; ");
                        Console.Write("Instruction [1] " + instruction[1] + " ; ");
                        Console.Write("Instruction [2] " + instruction[2] + " ; ");
                        Console.Write("Instruction [3] " + instruction[3] + " ; ");

                        // if instruction[0] == "1" -> command to attempt login
                        // indexes of instruction   [0]     [1]                         [2]                         [3]
                        // expected sentence:       1   $   userName (pre-encrypted) $  elephant (pre-crypted) $    password (pre-encrypted) $ 
                        if (instruction[0] == "1")
                        {
                            dB.login(instruction[1], instruction[3]);
                            sendMessage(activePlayers[client].psnws, "1$" + client.ToString());
                            
                        }

                        // if instruction[0] == "2" -> command to change directions
                        // indexes of instruction   [0]     [1]                         [2]                                     [3]
                        // expected sentence:       2 $     tostada (pre-crypted) $     { U D L R } $ "*****" (pre-crypt) $     { number of player who made the move }

                        if (instruction[0] == "2")
                        {
                            // update direction that the indicated player is traveling 

                            // counting by w


                            for (int w = 0; w < numberPlayers; ++w)
                            {
                                sendMessage(activePlayers[client].psnws, "2$");
                            }
                        }

                        

                    } // end game loop for a player

                } // end try

                catch (Exception beiber)
                {
                    Console.WriteLine("Exception " + beiber.Message);
                }
                
            } // end service

            public void sendMessage(NetworkStream theStream, String message)
            {
                Byte[] sendData = System.Text.Encoding.ASCII.GetBytes(message);
                // Send the message to the connected TcpServer. 
                activePlayers[client].psnws.Write(sendData, 0, sendData.Length);
                Console.WriteLine("Sent: " + message);
            }

            public void getMessage(NetworkStream theStream)
            {
                Byte[] data = new Byte[4096];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the TcpClient response bytes.
                Int32 buffer;
                try
                {
                    buffer = theStream.Read(data, 0, 4096);
                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, buffer);
                    Console.WriteLine("Received: " + responseData);
                }
                catch (Exception arg)
                {
                    Console.WriteLine("Exception: " + arg.Message);
                }
            }

        } // end readThread class
    }
}
