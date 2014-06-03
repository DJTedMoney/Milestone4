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

namespace ServerDatabase
{
    class TCPServer
    {
        static TcpListener listener;
        protected static PlayerSocket[] activePlayers;

        public static LoginDatabase dB;
        public static GameMechanicsManager gmm;

        static int numberPlayers;

        public TCPServer()
        { // start constructor

            dB = new LoginDatabase();
            gmm = new GameMechanicsManager();

            numberPlayers = gmm.getNumberPlayers();

            activePlayers = new PlayerSocket[numberPlayers];

            listener = new TcpListener(4300);
            listener.Start();

            dB.connectToDatabase();

            // dB.createTable();

            Console.Write("Press Enter to start the server:  ");
            Console.Read();

            // counting by t
            for (int t = 0; t < gmm.getNumberPlayers(); ++t)
            { // start for loop 
                Socket sock = listener.AcceptSocket();
                Console.WriteLine("Player Socket has accepted the socket");

                NetworkStream nws = new NetworkStream(sock);

                activePlayers[t] = new PlayerSocket(nws, sock, t);

                gmm.gamePlayers[t] = new Player(t);
                gmm.gamePlayers[t].connect();

                Console.Write("bottom of for loop in TCPServer constructor\n");
            } // end for loop 

        } // end constructor

        public string[] parseMessage(string input)
        {
            string[] parsedInput = new string[7];
            int numberParses = 0;

            parsedInput[0] = input.Substring(0, input.IndexOf('$'));
            input = input.Substring(input.IndexOf('$') + 1);

            if (parsedInput[0] == "0")
            {
                numberParses = 2;
            }

            else if (parsedInput[0] == "2")
            {
                numberParses = 3;
            }

            // counting by e
            for (int e = 1; e < numberParses; e++)
            {
                parsedInput[e] = input.Substring(0, input.IndexOf('$'));
                input = input.Substring(input.IndexOf('$') + 1);
            }

            return parsedInput;
        }

        public void gameLoop()
        { // start multi threading game loop 
            // moved the multi threading game loop out of TCPServer constructor 

            // counting by j
            for (int j = 0; j < gmm.getNumberPlayers(); j++)
            {
                // creates a ReadThread, passes in the player value t
                ReadThread thread = new ReadThread(j);

                activePlayers[j].psThread = new Thread(new ThreadStart(thread.Service));
                activePlayers[j].psThread.Start();
            }

            while (true)
            {  //put the main game loop logic in here:

                // go through received messages here
                // counting players by s
                for (int s = 0; s < gmm.getNumberPlayers(); s++)
                { // start for loop 

                    if (activePlayers[s].pSock.Connected)
                    { // if connected 

                        if (activePlayers[s].incomingMessages.Count > 0)
                        { // if player has any messages 

                            string newCommand;
                            lock (activePlayers[s].incomingMessages)
                            {
                                newCommand = activePlayers[s].incomingMessages.Dequeue();
                            }

                            string[] parsedCommand = parseMessage(newCommand);

                            // disconnect check loop to see if this player is disconnecting from the game  
                            if (parsedCommand[0] == "0")
                            {
                                // disconnect the player, leave the socket open for a new player
                                activePlayers[s].pSock.Disconnect(true);

                                string disconnectMessage = "5$" + s.ToString() + "$";
                                notifyAllPlayers(disconnectMessage);
                            }

                            // request to log in 
                            // parsedCommand 0 = 1
                            // parsedCommand 1 = userName
                            // parsedCommand 2 = elephant 
                            // parsedCommand 3 = password
                            if (parsedCommand[0] == "1")
                            { // if instruction login 
                                int loginVal = dB.attemptToLogin(parsedCommand[1], parsedCommand[3]);
                                Console.WriteLine(loginVal);

                                // loginVal == 0, wrong U/P combination, player fails to log in 
                                if (loginVal == 0)
                                {
                                    // sendMessage(activePlayers[client].psnws, "0$");
                                    activePlayers[s].pSock.Disconnect(true);
                                }

                                // loginVal == 1, successful login as returning user 
                                // give the player their starting position 
                                // starting size and starting velocity are hard coded into Client
                                else if ( (loginVal == 1) || (loginVal == 3) )
                                {
                                    string newPlayer = "";

                                    if(loginVal == 1)
                                    {
                                        newPlayer = "1$";
                                    }

                                    else if (loginVal == 3)
                                    {
                                        newPlayer = "3$";
                                    }
                                    
                                    // add the player ID to newPlayer
                                    newPlayer += s.ToString() + "$";

                                    // add player X and Y coordinates (randomly generated by server) to newPlayer
                                    newPlayer += (gmm.gamePlayers[s].getX_string() + "$");
                                    newPlayer += (gmm.gamePlayers[s].getY_string() + "$");

                                    Console.WriteLine("  new player message " + newPlayer);

                                    notifyAllPlayers(newPlayer);
                                }

                                // sendMessage();
                            } // end if instruction login

                            // parsed command 0 = 2
                            // parsed command 1 = direction to move U D L R 
                            // parsed command 2 = player ID 
                            if (parsedCommand[0] == "2")
                            {
                                gmm.executeCommand(parsedCommand);

                                int playID = Convert.ToInt32(parsedCommand[2]);

                                string moveMessage = "2$" + parsedCommand[2] + "$";
                                moveMessage += gmm.gamePlayers[playID].getLeftRightString() + "$";
                                moveMessage += gmm.gamePlayers[playID].getUpDownString() + "$";

                                notifyAllPlayers(moveMessage);
                            }
                        } // end if player has any messages

                        //move loop
                        gmm.gamePlayers[s].move();

                        /*

                        bool collided = false;
                        int collidingEnemy;
                        int pelletCollide;

                        //wall collision loop
                        if (gmm.detectCollisionsWithWalls(s))
                        {
                            gmm.gamePlayers[s].kill();

                            collided = true;
                        }

                        //pellet collision loop
                        pelletCollide = gmm.detectCollisionWithPellets(s);
                        if (pelletCollide != -1)
                        {
                            gmm.relocatePellet(pelletCollide);

                            gmm.gamePlayers[s].grow(20);
                            gmm.gamePlayers[s].slowDown(1);
                            gmm.gamePlayers[s].gainPoints(1);

                            collided = true;
                        }

                        //player collision loop
                        collidingEnemy = gmm.detectCollisionPlayers(s);
                        if (collidingEnemy != -1)
                        { // if collided with enemy 

                            collided = true;

                            // if the colliding enemy is larger, kill the client and award the colliding enemy
                            if (gmm.gamePlayers[collidingEnemy].getSize() > gmm.gamePlayers[s].getSize())
                            {
                                gmm.gamePlayers[collidingEnemy].grow(80);
                                gmm.gamePlayers[collidingEnemy].slowDown(4);
                                gmm.gamePlayers[collidingEnemy].gainPoints(10);

                                gmm.gamePlayers[s].kill();
                            }

                            // if client is bigger than colliding enemy, kill colliding enemy and award client
                            else if (gmm.gamePlayers[collidingEnemy].getSize() < gmm.gamePlayers[s].getSize())
                            {
                                gmm.gamePlayers[s].grow(80);
                                gmm.gamePlayers[s].slowDown(4);
                                gmm.gamePlayers[s].gainPoints(10);

                                gmm.gamePlayers[collidingEnemy].kill();
                            }

                            // if both players are the same size, kill them both 
                            else
                            {
                                gmm.gamePlayers[s].kill();
                                gmm.gamePlayers[collidingEnemy].kill();
                            }

                        } // end if collided with enemy 

                        // win condition check
                        //win condition loop
                        if (gmm.gamePlayers[s].getSize() > 400)
                        {
                            string winMessage = "6$" + s + "$" + gmm.gamePlayers[s].getScoreString() + "$";
                            notifyAllPlayers(winMessage);
                        }


                        if (collided)
                        { // begin game state update if a collision was detected 

                            string collideMessage = "2$";

                            // using a for loop to append each player's game state to the collidedMessage
                            // counting by u
                            for (int u = 0; u < gmm.getNumberPlayers(); u++)
                            {
                                collideMessage += gmm.gamePlayers[u].getX_string() + "$";
                                collideMessage += gmm.gamePlayers[u].getY_string() + "$";

                                collideMessage += gmm.gamePlayers[u].getLeftRightString() + "$";
                                collideMessage += gmm.gamePlayers[u].getUpDownString() + "$";

                                collideMessage += gmm.gamePlayers[u].getSpeed_string() + "$";
                                collideMessage += gmm.gamePlayers[u].getSize_string() + "$";
                            }

                            // use a for loop to append all pellet information to collidedMessage
                            // counting by o
                            for (int o = 0; o < 4; o++)
                            {
                                collideMessage += gmm.gamePellets[o].getPosX() + "$";
                                collideMessage += gmm.gamePellets[o].getPosY() + "$";
                            }

                            Console.WriteLine("   RESISTANCE IS FUTILE  " + collideMessage);

                            notifyAllPlayers(collideMessage);

                        } // end if a collision was detected 

                        */

                        if(activePlayers[s].outgoingMessages.Count > 0)
                        {
                            lock(activePlayers[s].outgoingMessages)
                            {
                                sendMessage(activePlayers[s].psnws, s, activePlayers[s].outgoingMessages.Dequeue() );
                            }
                        }
                    } // end if connected 

                } // end for loop 

            } // end game loop 

        } // end gameLoop

        public void sendMessage(NetworkStream theStream, int ID, String message)
        {
            Byte[] sendData = System.Text.Encoding.ASCII.GetBytes(message);
            // Send the message to the connected TcpServer. 
            activePlayers[ID].psnws.Write(sendData, 0, sendData.Length);
            Console.WriteLine("Sent: " + message);
        }

        void notifyAllPlayers(string command)
        {
            Console.WriteLine("     ***** Notifying All Players " + command);

            // counting by y
            for (int y = 0; y < gmm.getNumberPlayers(); y++)
            {
                if (activePlayers[y].pSock.Connected)
                {
                    // Byte[] commandToAll = System.Text.Encoding.ASCII.GetBytes(command);
                    // activePlayers[y].psnws.Write(commandToAll, 0, commandToAll.Length);

                    // sendMessage(activePlayers[y].psnws, y, command);

                    activePlayers[y].outgoingMessages.Enqueue(command);
                }
            }
        }

        class ReadThread
        { // start readThread class 

            int client = -1;
            char delimiter = '$';
            string clientString;
            string responseData;

            public ReadThread(int newNumber)
            { // begin constructor
                client = newNumber;
                clientString = newNumber.ToString();
                responseData = "";
            } // end constructor


            public void Service() // for an individual operating thread
            { // begin service
                try
                { // start try

                    // getMessage(activePlayers[client].psnws);
                    // Console.WriteLine(responseData);
                    
                    // getMessage(activePlayers[client].psnws);
                    // Console.WriteLine(responseData);
                    // sendMessage(activePlayers[client].psnws, "Test plus info$" + client + "$");

                    lock (activePlayers[client].incomingMessages)
                    {
                        activePlayers[client].incomingMessages.Enqueue("hello$");
                    }
                    

                    //Game Loop goes here!

                    while (activePlayers[client].pSock.Connected)
                    {
                        // waits on a message from this player
                        getMessage(activePlayers[client].psnws);

                        lock(activePlayers[client].incomingMessages)
                        {
                            activePlayers[client].incomingMessages.Enqueue(responseData);
                        }
                        
                        /*
                        //spliting the serverdata into instruction
                        string[] instruction = new string[11];

                        if(responseData[0].Equals('1') )
                        {
                            instruction = parseMessageSizeFour(responseData);
                        }

                        else if(responseData[0].Equals('2') )
                        {
                            instruction = parseMessageSizeThree(responseData);
                        }
                        

                        Console.Write("Instruction [0] " + instruction[0] + " ; ");
                        Console.Write("Instruction [1] " + instruction[1] + " ; ");
                        Console.Write("Instruction [2] " + instruction[2] + " ; ");
                       // Console.Write("Instruction [3] " + instruction[3] + " ; ");

                        
                        */

                    } // end of thread while loop that reads the stream 





                } // end try

                catch (Exception beiber)
                {
                    Console.WriteLine("Exception " + beiber.Message);
                }

            } // end service
            

            public void getMessage(NetworkStream theStream)
            { // begin getMessage 
                Byte[] data = new Byte[4096];

                Console.WriteLine("attempting to get message");

                // String to store the response ASCII representation.
                //String receivedData = String.Empty;
                responseData = String.Empty;

                // Read the TcpClient response bytes.
                Int32 buffer;
                try
                {
                    Console.WriteLine("In the getMessage Try");

                    buffer = theStream.Read(data, 0, 4096);
                    Console.WriteLine("buffer" + buffer);

                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, buffer);

                    Console.WriteLine("responseData " + responseData);
                }
                catch (Exception arg)
                {
                    Console.WriteLine("Exception: " + arg.Message);
                }
            } // end getMessage

            // each string passed in is assumed to have 3 delimited commands and a delimiter on the end 
            public string[] parseMessageSizeThree(string data)
            { // begin parseMessage
                string[] parsed = new string[3];

                // counting by g
                for (int g = 0; g < 3; g++)
                {
                    parsed[g] = data.Substring(0, data.IndexOf(delimiter));
                    data = data.Substring(data.IndexOf(delimiter) + 1);
                }
          

                return parsed;
            } // end parseMessage

            // each string passed in is assumed to have 4 delimited commands and a delimiter on the end 
            public string[] parseMessageSizeFour(string data)
            { // begin parseMessage
                string[] parsed = new string[4];

                // counting by g
                for (int g = 0; g < 4; g++)
                {
                    parsed[g] = data.Substring(0, data.IndexOf(delimiter));
                    data = data.Substring(data.IndexOf(delimiter) + 1);
                }

                return parsed;
            } // end parseMessage

        } // end readThread class
    }
}


