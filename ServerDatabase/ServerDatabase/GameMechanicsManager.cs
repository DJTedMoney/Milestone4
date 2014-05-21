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

        // pass in the player ID, and check for all collisions with other players 
        // returns the player ID of the first player that is detected to be colliding with pID
        // returns -1 if no player collisions are detected 
        public int detectCollisionPlayers(int pID)
        {
            // count opponents by o
            for (int o = 0; o < 4; o++)
            {
                // if o != clientID, check their distance as > or < (pID.size + o.size)
                if ( (o != pID) && (gamePlayers[o].connected) )
                {
                    int checkDist = distanceBetweenTwoPlayers(pID, o);

                    int combinedSize = gamePlayers[pID].getSize() + gamePlayers[o].getSize();

                    if (checkDist < combinedSize)
                    {
                        return o;
                    }
                }
            }

            return -1;
        }

        // pass in the ID of a player
        // checks all pellets for collision with the player
        // if collision between a player and a pellet is detected, 
        // returns the index of the pellet that is colliding with the player
        // else returns -1
        public int detectCollisionWithPellets(int pID)
        {
            // counting the pellets by p
            for (int p = 0; p < 4; p++)
            {
                int foodDist = distanceBetweenPlayerAndPellet(pID, p);

                int combSize = gamePlayers[pID].getSize() + 20;

                if (foodDist > combSize)
                {
                    return p;
                }
            }

            return -1;
        }

        public bool detectCollisionsWithWalls(int p)
        {
            if( (gamePlayers[p].getLocX() > 500) || (gamePlayers[p].getLocX() < -500) || (gamePlayers[p].getLocY() > 500) || 
                    (gamePlayers[p].getLocY() < -500) )
            {
                return true;
            }

            return false;
        }

        public int getNumberPlayers()
        {
            return maxPlayers;
        }

        int distanceBetweenTwoPlayers(int p1, int p2)
        {
            int distance = 0;
            double distDouble = 0;

            double distDiff_X = (gamePlayers[p1].getLocX() - gamePlayers[p2].getLocX() );
            double disX_squared = Math.Pow(distDiff_X, 2);

            double distDiff_Y = (gamePlayers[p1].getLocY() - gamePlayers[p2].getLocY() );
            double distY_squared = Math.Pow(distDiff_Y, 2);

            distDouble = Math.Sqrt(disX_squared + distY_squared);

            distance = (int) distDouble;

            return distance;
        }

        int distanceBetweenPlayerAndPellet(int player, int pellet)
        {
            int dist = 0;
            double distDoub = 0;

            double distX = (gamePlayers[player].getLocX() - gamePellets[pellet].getLocX() );
            double distX_sq = Math.Pow(distX, 2);

            double distY = (gamePlayers[player].getLocY() - gamePellets[pellet].getLocY() );
            double distY_sq = Math.Pow(distY, 2);

            distDoub = Math.Sqrt(distX_sq + distY_sq);

            dist = (int) distDoub;

            return dist;
        }

        public void relocatePellet(int c)
        {
            int moveX = roller.Next(-450, 450);
            int moveY = roller.Next(-450, 450);

            gamePellets[c].setPosition(moveX, moveY);

        }
    }
}
