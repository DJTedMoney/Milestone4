using System;
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

        // game mechanics to be implemented so that Player has a Speed value, how far they move
        // left/right value is either 1, -1, or 0
        // up/down value is either 1, -1, or 0
        // each frame, a player moves its left/right * speed and its up/down * speed
        // one of the two values will always be 0 (players cannot move diagonal)

        // speed x and y are the player speed left/right and up/down
        public int leftRight;
        public int upDown;
        public int moveSpeed;

        // size is size
        public int size;

        // determines if the player is currently connected to the server or not
        public bool connected;

        Random decider;

        // constructor produces location and direction, sets size to 40
        public Player()
        {
            decider = new Random();

            locX = decider.Next(-100, 100);
            locY = decider.Next(-100, 100);

            leftRight = 0;
            upDown = -1;
            moveSpeed = 10;

            size = 40;

            connected = false;
        }

        public void connect()
        {
            connected = true;
        }

        public void disconnect()
        {
            connected = false;
        }

        // move increments the x and y locations by x and y speeds
        public void move()
        {
            locX += (leftRight * moveSpeed);
            locY += (upDown * moveSpeed);
        }

        // grow increments player size
        public void grow(int food)
        {
            size += food;
        }

        public void slowDown(int slowFactor)
        {
            if (slowFactor > moveSpeed)
            {
                moveSpeed = 1;
            }

            else
            {
                moveSpeed -= slowFactor;
            }
        }

        public void setSpeed(int newSpeed)
        {
            moveSpeed = newSpeed;
        }

        public string getLocX()
        {
            return locX.ToString();
        }

        public string getLocY()
        {
            return locY.ToString();
        }

        public string getSpeed()
        {
            return moveSpeed.ToString();
        }

        public string getSize()
        {
            return size.ToString();
        }

        public void goLeft()
        {
            leftRight = -1;
            upDown = 0;
        }

        public void goRight()
        {
            leftRight = 1;
            upDown = 0;
        }

        public void goUp()
        {
            leftRight = 0;
            upDown = 1;
        }

        public void goDown()
        {
            leftRight = 0;
            upDown = -1;
        }
    }
}
