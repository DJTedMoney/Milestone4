using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDatabase
{
    class Pellet
    {
        int x;
        int y;

        int size;

        public Pellet()
        {
            size = 20;
        }

        public void setPosition(int newX, int newY)
        {
            x = newX;
            y = newY;
        }

        public string getPosX()
        {
            return x.ToString();
        }

        public string getPosY()
        {
            return y.ToString();
        }
    }
}
