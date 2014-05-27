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
    class PlayerSocket
    {
        // local 127 0 0 1
        // wireless desktop 128 195 11 143
        // const string SERVER = "127.0.0.1";
        const int SERVER_PORT = 4300;


        public NetworkStream psnws;
        public int clientID;
        public Thread psThread = null;
        // protected static bool threadState = false;
        public Queue<string> updates;
        public Socket pSock;

        public PlayerSocket(NetworkStream newStream, Socket newSocket, int newID)
        {
            psnws = newStream;
            pSock = newSocket;
            clientID = newID;

            updates = new Queue<string>();
        }
    }
}
