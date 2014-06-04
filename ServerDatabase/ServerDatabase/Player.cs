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
        int leftRight;
        int upDown;
        public int moveSpeed;

        // size is size
        public int size;

        // determines if the player is currently connected to the server or not
        public bool connected;

        // score for win condition
        public int score;

        public int clientNumber;

        Random decider;

        // constructor produces location and direction, sets size to 40
        public Player(int newNumber)
        {
            decider = new Random();

            locX = decider.Next(-100, 100);
            locY = decider.Next(-100, 100);

            leftRight = 0;
            upDown = -1;
            moveSpeed = 1;

            size = 40;
            score = 0;

            connected = false;

            clientNumber = newNumber;
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
            if (slowFactor >= moveSpeed)
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

        public int getLocX()
        {
            return locX;
        }

        public string getX_string()
        {
            return locX.ToString();
        }

        public int getLocY()
        {
            return locY;
        }

        public string getY_string()
        {
            return locY.ToString();
        }

        public int getSpeed()
        {
            return moveSpeed;
        }

        public string getSpeed_string()
        {
            return moveSpeed.ToString();
        }

        public string getLeftRightString()
        {
            return leftRight.ToString();
        }

        public string getUpDownString()
        {
            return upDown.ToString();
        }

        public int getSize()
        {
            return size;
        }

        public string getSize_string()
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

        public void kill()
        {
            locX = decider.Next(-100, 100);
            locY = decider.Next(-100, 100);

            leftRight = 0;
            upDown = -1;
            moveSpeed = 1;

            size = 40;
            score = 0;
        }

        public int getScore()
        {
            return score;
        }

        public string getScoreString()
        {
            return score.ToString();
        }

        public void gainPoints(int points)
        {
            score += points;
        }

        public void setScore(int newScore)
        {
            score = newScore;
        }
    }
}
