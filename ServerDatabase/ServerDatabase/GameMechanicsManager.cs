using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDatabase
{
    class GameMechanicsManager
    {
        const int maxPlayers = 4;

        public Pellet[] gamePellets = new Pellet[4];

        public Player[] gamePlayers = new Player[4];

        Random roller;

        // send is used to determine if the game manager is sending a message to the TCPServer or not
        bool send;

        // command is the instruction that is being read and parsed 
        string command;

        public GameMechanicsManager()
        {
            roller = new Random();

            // counting by p
            for (int p = 0; p < 4; p++)
            {
                gamePellets[p] = new Pellet();

                int startX = roller.Next(-450, 450);
                int startY = roller.Next(-450, 450);

                gamePellets[p].setPosition(startX, startY);
            }
        }

        // pass in the player ID, and check for all collisions
        public void detectCollision(int pID)
        {
            // count opponents by o
            for (int o = 0; o < 4; o++)
            {
                // if
            }
        }

        public int getNumberPlayers()
        {
            return maxPlayers;
        }
    }
}
