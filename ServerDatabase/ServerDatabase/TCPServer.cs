﻿// Andrew Franowicz 29297832
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

            else if (parsedInput[0] == "1")
            {
                numberParses = 4;
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
                {
                    System.Threading.Thread.Sleep(15);
                    //put the main game loop logic in here:

                    // go through received messages here
                    // counting players by s
                    for (int s = 0; s < gmm.getNumberPlayers(); s++)
                    { // start for loop 

                        if (activePlayers[s].pSock.Connected)
                        { // if connected 

                            int incomingMessageCount = 0;

                            lock (activePlayers[s].incomingMessages)
                            {
                                incomingMessageCount = activePlayers[s].incomingMessages.Count();
                            }

                            if (incomingMessageCount> 0)
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
                                    Console.WriteLine("Returned value of DB: " + loginVal);

                                    // loginVal == 0, wrong U/P combination, player fails to log in 
                                    if (loginVal == 0)
                                    {
                                        // sendMessage(activePlayers[client].psnws, "0$");
                                        activePlayers[s].pSock.Disconnect(true);
                                    }

                                    // loginVal == 1, successful login as returning user 
                                    // give the player their starting position 
                                    // starting size and starting velocity are hard coded into Client
                                    else if ((loginVal == 1) || (loginVal == 3))
                                    {
                                        // new player is sent to the player that just logged in 
                                        string newPlayer = "";

                                        // addNewPlayer is sent to all other active players, telling them that a new player
                                        //      has just logged in 
                                        string addNewPlayer;

                                        if (loginVal == 1)
                                        {
                                            newPlayer = "1$";
                                        }

                                        else if (loginVal == 3)
                                        {
                                            newPlayer = "3$";
                                        }

                                        // add the player ID to newPlayer
                                        newPlayer += s.ToString() + "$";
                                        addNewPlayer = "4$" + s.ToString() + "$";

                                        // add player X and Y coordinates (randomly generated by server) to newPlayer
                                        newPlayer += (gmm.gamePlayers[s].getX_string() + "$");
                                        newPlayer += (gmm.gamePlayers[s].getY_string() + "$");

                                        addNewPlayer += (gmm.gamePlayers[s].getX_string() + "$");
                                        addNewPlayer += (gmm.gamePlayers[s].getY_string() + "$");

                                        // send all players the username of the new player 
                                        addNewPlayer += parsedCommand[1] + "$";

                                        Console.WriteLine("  new player message " + newPlayer);

                                        sendMessage(activePlayers[s].psnws, s, newPlayer);

                                        // counting by n
                                        for (int n = 0; n < gmm.getNumberPlayers(); n++)
                                        {
                                            if ( activePlayers[n].pSock.Connected)
                                            {
                                                sendMessage(activePlayers[n].psnws, n, addNewPlayer);
                                            }
                                        }

                                        // counting by m
                                        for (int m = 0; m < gmm.getNumberPlayers(); m++)
                                        {
                                            if (activePlayers[m].pSock.Connected)
                                            {
                                                // user id 
                                                string opponentStatus = "7$" + m.ToString() + "$";

                                                // x and y position
                                                opponentStatus += gmm.gamePlayers[m].getX_string() + "$";
                                                opponentStatus += gmm.gamePlayers[m].getY_string() + "$";

                                                // x and y direction 
                                                opponentStatus += gmm.gamePlayers[m].getLeftRightString() + "$";
                                                opponentStatus += gmm.gamePlayers[m].getUpDownString() + "$";

                                                // size, score, speed, username
                                                opponentStatus += gmm.gamePlayers[m].getSize_string() + "$";
                                                opponentStatus += gmm.gamePlayers[m].getScoreString() + "$";
                                                opponentStatus += gmm.gamePlayers[m].getSpeed_string() + "$";
                                                opponentStatus += parsedCommand[1] + "$";

                                                sendMessage(activePlayers[s].psnws, s, opponentStatus);
                                            }
                                        }

                                        // update where all the pellets are for each player
                                        for (int j = 0; j < 4; j++)
                                        {
                                            string newPellet = "8$" + j + "$";
                                            newPellet += gmm.gamePellets[j].getPosX() + "$";
                                            newPellet += gmm.gamePellets[j].getPosY() + "$";

                                            notifyAllPlayers(newPellet);
                                        }
                                    }

                                } // end if instruction login

                                // parsed command 0 = 2
                                // parsed command 1 = direction to move U D L R 
                                // parsed command 2 = player ID 
                                if (parsedCommand[0] == "2")
                                {
                                    gmm.executeCommand(parsedCommand);

                                    int playID = Convert.ToInt32(parsedCommand[2]);

                                    string moveMessage = "2$" + parsedCommand[2] + "$";

                                    // x and y direction of travel 
                                    moveMessage += gmm.gamePlayers[playID].getLeftRightString() + "$";
                                    moveMessage += gmm.gamePlayers[playID].getUpDownString() + "$";

                                    // x and y position
                                    moveMessage += gmm.gamePlayers[playID].getX_string() + "$";
                                    moveMessage += gmm.gamePlayers[playID].getY_string() + "$";

                                    notifyAllPlayers(moveMessage);
                                }
                            } // end if player has any messages

                            //move loop
                            gmm.gamePlayers[s].move();

                            bool collided = false;
                            int pelletCollide;
                            int collidingEnemy;

                            //wall collision loop
                            if (gmm.detectCollisionsWithWalls(s))
                            {
                                gmm.gamePlayers[s].kill();

                                collided = true;

                                // 6$ is the universal player status message after a collision
                                // 6$ id $ x position $ y position $
                                string wallCollide = "6$" + s.ToString() + "$";
                                wallCollide += gmm.gamePlayers[s].getX_string() + "$";
                                wallCollide += gmm.gamePlayers[s].getY_string() + "$";

                                // x direction $ y direction $
                                wallCollide += gmm.gamePlayers[s].getLeftRightString() + "$";
                                wallCollide += gmm.gamePlayers[s].getUpDownString() + "$";

                                // size $ score $ speed $
                                wallCollide += gmm.gamePlayers[s].getSize_string() + "$";
                                wallCollide += gmm.gamePlayers[s].getScoreString() + "$";
                                wallCollide += gmm.gamePlayers[s].getSpeed_string() + "$";

                                notifyAllPlayers(wallCollide);
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

                                // 6$ is the universal player status message after a collision
                                // 6$ id $ x position $ y position $
                                string pelletEater = "6$" + s.ToString() + "$";
                                pelletEater += gmm.gamePlayers[s].getX_string() + "$";
                                pelletEater += gmm.gamePlayers[s].getY_string() + "$";

                                // x direction $ y direction $
                                pelletEater += gmm.gamePlayers[s].getLeftRightString() + "$";
                                pelletEater += gmm.gamePlayers[s].getUpDownString() + "$";

                                // size $ score $ speed $
                                pelletEater += gmm.gamePlayers[s].getSize_string() + "$";
                                pelletEater += gmm.gamePlayers[s].getScoreString() + "$";
                                pelletEater += gmm.gamePlayers[s].getSpeed_string() + "$";

                                notifyAllPlayers(pelletEater);

                                // gmm.relocatePellet(pelletCollide);
                                string newPellet = "8$" + pelletCollide.ToString() + "$";
                                newPellet += gmm.gamePellets[pelletCollide].getPosX() + "$";
                                newPellet += gmm.gamePellets[pelletCollide].getPosY() + "$";

                                notifyAllPlayers(newPellet);
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

                                // update the world on the status of player s
                                string colliderUpdate = "6$" + s.ToString() + "$";
                                colliderUpdate += gmm.gamePlayers[s].getX_string() + "$";
                                colliderUpdate += gmm.gamePlayers[s].getY_string() + "$";

                                // x direction $ y direction $
                                colliderUpdate += gmm.gamePlayers[s].getLeftRightString() + "$";
                                colliderUpdate += gmm.gamePlayers[s].getUpDownString() + "$";

                                // size $ score $ speed $
                                colliderUpdate += gmm.gamePlayers[s].getSize_string() + "$";
                                colliderUpdate += gmm.gamePlayers[s].getScoreString() + "$";
                                colliderUpdate += gmm.gamePlayers[s].getSpeed_string() + "$";

                                notifyAllPlayers(colliderUpdate);


                                // update the world on the status of colliding enemy 
                                colliderUpdate = "6$" + collidingEnemy.ToString() + "$";
                                colliderUpdate += gmm.gamePlayers[collidingEnemy].getX_string() + "$";
                                colliderUpdate += gmm.gamePlayers[collidingEnemy].getY_string() + "$";

                                // x direction $ y direction $
                                colliderUpdate += gmm.gamePlayers[collidingEnemy].getLeftRightString() + "$";
                                colliderUpdate += gmm.gamePlayers[collidingEnemy].getUpDownString() + "$";

                                // size $ score $ speed $
                                colliderUpdate += gmm.gamePlayers[collidingEnemy].getSize_string() + "$";
                                colliderUpdate += gmm.gamePlayers[collidingEnemy].getScoreString() + "$";
                                colliderUpdate += gmm.gamePlayers[collidingEnemy].getSpeed_string() + "$";

                                notifyAllPlayers(colliderUpdate);

                            } // end if collided with enemy 

                            // win condition check
                            //win condition loop
                            if (gmm.gamePlayers[s].getSize() > 400)
                            {
                                string winMessage = "9$" + s + "$" + gmm.gamePlayers[s].getScoreString() + "$";
                                notifyAllPlayers(winMessage);
                            }

                            int newCount = 0;
                            lock (activePlayers[s].outgoingMessages)
                            {
                                newCount = activePlayers[s].outgoingMessages.Count;
                            }
                            if (newCount > 0)
                            {
                                lock (activePlayers[s].outgoingMessages)
                                {
                                    sendMessage(activePlayers[s].psnws, s, activePlayers[s].outgoingMessages.Dequeue());
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

                    lock(activePlayers[y].outgoingMessages)
                    {
                        activePlayers[y].outgoingMessages.Enqueue(command);
                    }
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

                    //lock (activePlayers[client].incomingMessages)
                    lock (activePlayers[client].outgoingMessages)
                    {
                        //activePlayers[client].incomingMessages.Enqueue("hello$");
                        activePlayers[client].outgoingMessages.Enqueue("hello$");
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
                    Console.WriteLine("Size of buffer in getMessage: " + buffer);

                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, buffer);

                    Console.WriteLine("Received data: " + responseData);
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


