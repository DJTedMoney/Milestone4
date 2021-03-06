﻿// Andrew Franowicz 29297832
// Jason Heckard  84851006
// Nathan Stengel 28874701

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SQLite;

namespace ServerDatabase
{
    class LoginDatabase
    {
        // Holds our connection with the database
        SQLiteConnection m_dbConnection;

        // Creates an empty database file
        public void createNewDatabase()
        {
            SQLiteConnection.CreateFile("LoginDatabase.sqlite");
        }

        // Creates a connection with our database file.
        public void connectToDatabase()
        {
            m_dbConnection = new SQLiteConnection("Data Source=LoginDatabase.sqlite;Version=3;");
            m_dbConnection.Open();
        }

        // Creates a table named 'highscores' with two columns: name (a string of max 20 characters) and password (a string of max 20 characters)
        // password = pw
        public void createTable()
        {
            //SELECT name FROM sqlite_master WHERE type='table' AND name='table_name';

            string sql = "SELECT * FROM sqlite_master WHERE type='table' AND name='users'";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            if (!reader.HasRows)
            {
                Console.WriteLine("CREATING TABLE");

                sql = "create table users (name varchar(32), pw varchar(32) )";
                command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();

                Console.WriteLine("HI!");
            }
            else
            {
                Console.WriteLine("TABLE EXISTS!");
            }
        }

        // Inserts some values in the highscores table.
        // As you can see, there is quite some duplicate code here, we'll solve this in part two.
        public void fillTable()
        {
            /*
            string sql = "insert into users (name, pw) values ('Me', 3000)";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();

            sql = "insert into users (name, pw) values ('Myself', 6000)";
            command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();

            sql = "insert into users (name, pw) values ('And I', 9001)";
            command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
             */

            attemptToLogin("Me", "3000");
            attemptToLogin("Myself", "6000");
            attemptToLogin("And I", "9001");

        }

        void addElement(string userName, string newPW)
        {
            string sql = "insert into users (name, pw) values ('" + userName + "', '" + newPW + "')";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }

        // return 1 if the login attempt was a success
        // return 0 if the login attempt was a fail
        int verifyPassword(string loginName, string loginPW)
        {
            // check that newPW matches password of userName
            string sql = "select * from users where name= '" + loginName + "'";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            /*
            Console.WriteLine("      inside verify password " + loginName);
            Console.WriteLine("  verivy password values &" + reader["pw"] + "&  %" + loginPW + "% ");
            Console.WriteLine( reader["pw"].Equals(loginPW) );
            Console.WriteLine(loginPW.Equals(reader["pw"]) );
            */

            // the password is a match
            if (reader["pw"].Equals(loginPW))
            {
                Console.WriteLine("    Password is a match    " + loginName);
                // return true;

                // send TCP packet back to client setting player as logged in
                return 1;
            }

            else
            {
                Console.WriteLine("  &&  Password is NOT a match  " + loginName);
                // return false;

                // send TCP packet back to client that login failed 
                return 0;
            }
        }

        bool checkIfUserNameExists(string userName)
        {
            string sql = "select * from users where name= '" + userName + "'";
            Console.WriteLine("sql " + sql);
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

           // Console.WriteLine("test");

            // command.Connection.Open();
            bool userExists = reader.Read();

            if (userExists)
            {
                // Console.WriteLine("  found it! " + userName);
                return true;
            }

            // Console.WriteLine("   didn't find it " + userName);
            return false;
        }

        // return 1 if login attempt was a success
        // return 0 if the login attempt was a fail 
        // return 3 if the new user was created
        public int attemptToLogin(string checkName, string checkPW)
        {
            bool returningUser = checkIfUserNameExists(checkName);

            // Console.WriteLine("  calling login " + checkName);

            if (returningUser)
            {
                int verify = verifyPassword(checkName, checkPW);

                return verify;
            }

            else
            {
                addElement(checkName, checkPW);
                return 3;
            }
        }

        // Writes the highscores to the console sorted on score in descending order.
        public void printUsers()
        {

            string sql = "select * from users order by pw desc";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine("Name: " + reader["name"] + "\tPassword: " + reader["pw"]);
            }

            Console.ReadLine();
        }
    }
}
