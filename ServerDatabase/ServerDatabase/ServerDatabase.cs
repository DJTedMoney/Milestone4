// Andrew Franowicz 29297832
// Jason Heckard  84851006
// Nathan Stengel 28874701

using System;

using System.Data.SQLite;

namespace ServerDatabase
{

    class ServerDatabase
    {
        // static LoginDatabase dB;

        static TCPServer server;

        static ServerDatabase p = new ServerDatabase();
        
        static void Main(string[] args)
        {
            // server.dB.createNewDatabase();
            //TCPServer.dB.connectToDatabase();

            //TCPServer.dB.createTable();

            TCPServer.dB.fillTable();

            TCPServer.dB.attemptToLogin("Farble", "deblarg");
            TCPServer.dB.attemptToLogin("glory", "power");
            TCPServer.dB.attemptToLogin("Farble", "deblarg");

            TCPServer.dB.attemptToLogin("Myself", "6000");
            TCPServer.dB.attemptToLogin("Myself", "asdf");

            TCPServer.dB.printUsers();
        }

        public ServerDatabase()
        {
            //TCPServer.dB = new LoginDatabase();

            server = new TCPServer();
        }
        
    }
}
