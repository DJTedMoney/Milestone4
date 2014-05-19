using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDatabase
{
    class GameMechanicsManager
    {
        Pellet[] gamePellets = new Pellet[4];

        Player[] gamePlayers = new Player[4];

        Random roller;

        // send is used to determine if the game manager is sending a message to the TCPServer or not
        bool send;

        // command is the instruction that 
        string command;

        GameMechanicsManager()
        {
            roller = new Random();

            // counting by p
            for (int p = 0; p < 4; p++)
            {
                int startX = roller.Next(-450, 450);
                int startY = roller.Next(-450, 450);

                gamePellets[p].setPosition(startX, startY);
            }

            // counting players by l
            for (int l = 0; l < 4; l++)
            {

            }
        }

    }
}
