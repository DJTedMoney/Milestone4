﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDatabase
{
    class Player
    {
        // loc x and y are the player location
        public int locX;
        public int locY;

        // speed x and y are the player speed left/right and up/down
        public int speedX;
        public int speedY;

        // size is size
        public int size;

        // constructor passes in location and direction, sets size to 40
        public Player(int x, int y, int dirX, int dirY)
        {
            locX = x;
            locY = y;

            speedX = dirX;
            speedY = dirY;

            size = 40;
        }

        // move increments the x and y locations by x and y speeds
        public void move()
        {
            locX += speedX;
            locY += speedY;
        }

        // grow increments player size
        public void grow(int food)
        {
            size += food;
        }

        public void slowDown(int slowFactor)
        {
            for(int s = 0; s < slowFactor; s++)
            {
                if (speedX > 1)
                {
                    speedX *= (speedX - 1) / speedX;
                }

                if (speedY > 1)
                {
                    speedY *= (speedY - 1) / speedY;
                }
            }
        }

        public void setSpeed(int newX, int newY)
        {
            speedX = newX;
            speedY = newY;
        }

        public int getSpeedX()
        {
            return speedX;
        }

        public int getSpeedY()
        {
            return speedY;
        }

        public int getLocX()
        {
            return locX;
        }

        public int getLocY()
        {
            return locY;
        }

        public int getSize()
        {
            return size;
        }

    }
}
