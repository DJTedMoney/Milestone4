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

namespace SQLiteTest
{
    class PlayerSocket
    {
        const string SERVER = "128.195.11.143";
        const int SERVER_PORT = 4300;

        public TcpClient client;

        public NetworkStream psnws;
        public int clientID;
        public bool startGame;
        public bool connected;
        public DateTime dateTime;
        public Thread psThread = null;
        protected static bool threadState = false;
        public Queue<string> updates;
        public Socket pSock;

        public StreamReader playerReader;
        public StreamWriter playerWriter;

        public PlayerSocket(NetworkStream newStream, Socket newSocket /*,StreamReader newSR, StreamWriter newSW*/)
        {
            psnws = newStream;
            pSock = newSocket;
            //playerReader = newSR;
            //playerWriter = newSW;

            updates = new Queue<string>();
        }
    }
}
