﻿// Andrew Franowicz 29297832
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
            Console.WriteLine("1");
            //TCPServer.dB.createNewDatabase();
            Console.WriteLine("2");
            TCPServer.dB.connectToDatabase();
            Console.WriteLine("3");
            TCPServer.dB.createTable();
            Console.WriteLine("4");

            //TCPServer.dB.fillTable();

            TCPServer.dB.attemptToLogin("Farble", "deblarg");
            TCPServer.dB.attemptToLogin("glory", "power");
            TCPServer.dB.attemptToLogin("Farble", "deblarg");

            TCPServer.dB.attemptToLogin("Myself", "6000");
            TCPServer.dB.attemptToLogin("Myself", "asdf");

            TCPServer.dB.printUsers();

            server.gameLoop();
        }

        public ServerDatabase()
        {
            //TCPServer.dB = new LoginDatabase();

            server = new TCPServer();
        }
        
    }
}
