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

        public int getLocX()
        {
            return x;
        }

        public string getPosX()
        {
            return x.ToString();
        }

        public int getLocY()
        {
            return y;
        }

        public string getPosY()
        {
            return y.ToString();
        }
    }
}
