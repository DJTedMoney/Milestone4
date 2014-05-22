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
        static Queue<string> movesMade;

        public static LoginDatabase dB;
        public static GameMechanicsManager gmm;

        static int numberPlayers;

        public TCPServer()
        { // start constructor

            dB = new LoginDatabase();
            gmm = new GameMechanicsManager();

            numberPlayers = gmm.getNumberPlayers();

            activePlayers = new PlayerSocket[ numberPlayers ];

            listener = new TcpListener(4300);
            listener.Start();

            movesMade = new Queue<string>();

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
                //StreamReader reader = new StreamReader(nws);
                // StreamWriter writer = new StreamWriter(nws);
                //writer.AutoFlush = true;

                // Console.WriteLine("stream created");

                activePlayers[t] = new PlayerSocket(nws, sock, t);

                gmm.gamePlayers[t] = new Player();
                gmm.gamePlayers[t].connect();

                // Console.WriteLine(" connected true");

                // string data = activePlayers[t].playerReader.ReadLine();
                // Console.Write(data);

                // creates a ReadThread, passes in the player value t
                ReadThread thread = new ReadThread(t);

                activePlayers[t].psThread = new Thread(new ThreadStart(thread.Service) );
                activePlayers[t].psThread.Start();
                Console.Write("bottom of for loop in TCPServer constructor\n");
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
                responseData = "";
            } // end constructor

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

                Console.WriteLine("attempting to get message");

                // String to store the response ASCII representation.
                //String receivedData = String.Empty;
                responseData = String.Empty;

                // Read the TcpClient response bytes.
                Int32 buffer;
                try
                {
                    Console.WriteLine("  In the Try");

                    buffer = theStream.Read(data, 0, 4096);
                    Console.WriteLine("buffer" + buffer);

                    responseData = System.Text.Encoding.ASCII.GetString(data, 0, buffer);

                    Console.WriteLine("responseData " + responseData);

                    Console.WriteLine("Received: " + responseData);
                }
                catch (Exception arg)
                {
                    Console.WriteLine("Exception: " + arg.Message);
                }
            }

            public void Service() // for an individual operating thread
            { // begin service
                try
                { // start try

                    while (activePlayers[client].pSock.Connected )
                    { // actual game loop for an individual player
                        Console.WriteLine("in service while loop for player " + client);

                        gmm.gamePlayers[client].move();

                        bool collided = false;

                        if( gmm.detectCollisionsWithWalls(client) )
                        {
                            gmm.gamePlayers[client].kill();

                            collided = true;
                        }

                        int pelletCollide = gmm.detectCollisionWithPellets(client);

                        if(pelletCollide != -1)
                        {
                            gmm.relocatePellet(pelletCollide);

                            gmm.gamePlayers[client].grow(20);
                            gmm.gamePlayers[client].slowDown(1);
                            gmm.gamePlayers[client].gainPoints(1);

                            collided = true;
                        }

                        int collidingEnemy = gmm.detectCollisionPlayers(client);

                        if(collidingEnemy != -1)
                        {
                            collided = true;

                            // if the colliding enemy is larger, kill the client and award the colliding enemy
                            if(gmm.gamePlayers[collidingEnemy].getSize() > gmm.gamePlayers[client].getSize() )
                            {
                                gmm.gamePlayers[collidingEnemy].grow(80);
                                gmm.gamePlayers[collidingEnemy].slowDown(4);
                                gmm.gamePlayers[collidingEnemy].gainPoints(10);

                                gmm.gamePlayers[client].kill();
                            }

                            // if client is bigger than colliding enemy, kill colliding enemy and award client
                            else if (gmm.gamePlayers[collidingEnemy].getSize() < gmm.gamePlayers[client].getSize())
                            {
                                gmm.gamePlayers[client].grow(80);
                                gmm.gamePlayers[client].slowDown(4);
                                gmm.gamePlayers[client].gainPoints(10);

                                gmm.gamePlayers[collidingEnemy].kill();
                            }

                            // if both players are the same size, kill them both 
                            else
                            {
                                gmm.gamePlayers[client].kill();
                                gmm.gamePlayers[collidingEnemy].kill();
                            }

                        }

                        Console.WriteLine("Preparing to get message");
                        getMessage(activePlayers[client].psnws);


                        if (collided)
                        {
                            // counting by t to update every player of board status after a collision
                            for (int t = 0; t < numberPlayers; t++)
                            {
                                // abbreviating the current active player to p
                                Player p = gmm.gamePlayers[client];

                                // 2 $ locX $ locY $ dirX $ dirY $ speed $ size $  pellet1_x $ pellet1_y $ 
                                //      pellet2_x $ pellet2_y $ pellet3_x $ pellet3_y $ pellet4_x $ pellet4_y $
                                string moveMessage = "2$" + p.getX_string() + "$" + p.getY_string() + "$" + p.getLeftRightString() +
                                    "$" + p.getUpDownString() + "$" + p.getSpeed_string() + "$" + p.getSize_string() +
                                    "$" + p.getScoreString() + "$" + gmm.gamePellets[0].getPosX() + "$" + gmm.gamePellets[0].getPosY() +
                                    "$" + gmm.gamePellets[1].getPosX() + "$" + gmm.gamePellets[1].getPosY() + "$" + gmm.gamePellets[2].getPosX() +
                                    "$" + gmm.gamePellets[2].getPosY() + "$" + gmm.gamePellets[3].getPosX() + "$" + gmm.gamePellets[3].getPosY() + "$";

                                sendMessage(activePlayers[t].psnws, moveMessage);
                            }
                        }

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

                        ////// attempt to login now has return values 
                        //          return 1 -> player successfully logged in 
                        //          return 0 -> incorrect password, login failed
                        //          return 2 -> new username/pw added to database as new player 
                        if (instruction[0] == "1")
                        { // begin instruction 1
                            int loginStatus;

                            Console.WriteLine("Attempting to log in");

                            loginStatus = dB.attemptToLogin(instruction[1], instruction[3]);

                            string loginMessage;

                            // if loginStatus == 1
                            // returning user has successfully logged in
                            // if loginStatus == 3
                            // new user successfully created
                            if ( (loginStatus == 1) || (loginStatus == 3) )
                            {
                                sendMessage(activePlayers[client].psnws, loginStatus.ToString() + "$" + client.ToString());

                                loginMessage = "4$" + client.ToString() + "$" + gmm.gamePlayers[client].getX_string()
                                + "$" + gmm.gamePlayers[client].getY_string() + "$";

                                // update all players of the status of the new player's position
                                // counting by u
                                for (int u = 0; u < numberPlayers; u++)
                                {
                                    if ( (gmm.gamePlayers[u] != null) && (gmm.gamePlayers[u].connected) )
                                    {
                                        sendMessage(activePlayers[u].psnws, loginMessage);
                                    }
                                }

                                // update the new player of the position of all other players 
                                // counting by i

                                //problem here.  when only one player is connected, it still tries to run the loop for numberPlayers (who might not all be connected).
                                //so for anything after the first tile the loop runs is a null object exception
                                for(int i = 0; i < numberPlayers; i++)
                                {
                                    loginMessage = "4$" + client.ToString() + "$" + gmm.gamePlayers[i].getX_string()
                                        + "$" + gmm.gamePlayers[i].getY_string() + "$";

                                    sendMessage(activePlayers[client].psnws, loginMessage);
                                }

                                // sendMessage(activePlayers[client].psnws, loginMessage);
                            }

                            else if (loginStatus == 0)
                            {
                                sendMessage(activePlayers[client].psnws, "0$" + "Wrong password, try again" + "$");
                            }
                        } // end instruction 1

                        // if instruction[0] == "2" -> command to change directions
                        // indexes of instruction   [0]     [1]                         [2]                                     [3]
                        // expected sentence:       2 $     tostada (pre-crypted) $     { U D L R } $ "*****" (pre-crypt) $     { number of player who made the move } $ 
                        if (instruction[0] == "2")
                        { // begin instruction 2
                            // update direction that the indicated player is traveling 
                            string directionToMove = instruction[2];

                            // mP = moving player.  This is stored to make the variable name shorter 
                            int mP = Convert.ToInt32( instruction[3] );

                            if (directionToMove == "U")
                            {
                                gmm.gamePlayers[mP].goUp();
                            }

                            else if (directionToMove == "D")
                            {
                                gmm.gamePlayers[mP].goDown();
                            }

                            else if (directionToMove == "L")
                            {
                                gmm.gamePlayers[mP].goLeft();
                            }

                            else if (directionToMove == "R")
                            {
                                gmm.gamePlayers[mP].goRight();
                            }

                            // send a message to every other player with the new direction of the player who turned 
                            // counting by w
                            for (int w = 0; w < numberPlayers; w++)
                            {
                                // abbreviating the current active player to p
                                Player p = gmm.gamePlayers[client];

                            // 2 $ locX $ locY $ dirX $ dirY $ speed $ size $  pellet1_x $ pellet1_y $ 
                            //      pellet2_x $ pellet2_y $ pellet3_x $ pellet3_y $ pellet4_x $ pellet4_y $
                                string moveMessage = "2$" + p.getX_string() + "$" + p.getY_string() + "$" + p.getLeftRightString() +
                                    "$" + p.getUpDownString() + "$" + p.getSpeed_string() + "$" +p.getSize_string() + 
                                    "$" + p.getScoreString() + "$" + gmm.gamePellets[0].getPosX() + "$" + gmm.gamePellets[0].getPosY() + 
                                    "$" + gmm.gamePellets[1].getPosX() + "$" + gmm.gamePellets[1].getPosY() + "$" + gmm.gamePellets[2].getPosX() + 
                                    "$" + gmm.gamePellets[2].getPosY() + "$" + gmm.gamePellets[3].getPosX() + "$" + gmm.gamePellets[3].getPosY() + "$";

                                sendMessage(activePlayers[client].psnws, moveMessage);
                            }
                        } // end instruction 2

                        // instruction [0] == "0" -> client wants to disconnect
                        if (instruction[0] == "0")
                        {

                        }

                    } // end game loop for a player

                } // end try

                catch (Exception beiber)
                {
                    Console.WriteLine("Exception " + beiber.Message);
                }
                
            } // end service

        } // end readThread class
    }
}
