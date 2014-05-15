// Andrew Franowicz 29297832
// Jason Heckard  84851006
// Nathan Stengel 28874701

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security.Cryptography;


//      FOUND THIS EXAMPLE OF HOW TO USE THE CRYPTOGRAPHY LIBRARY FOR C#
//      EXAMPLE FROM http://www.codeproject.com/Articles/463390/Password-Encryption-using-MD-Hash-Algorithm-in-Cs

    public static class Encryptor
    {
        public static string encryptString(string token)
        {
            MD5 md5_encryptor = new MD5CryptoServiceProvider();

            // takes the bytes of token, computes the hash
            md5_encryptor.ComputeHash(ASCIIEncoding.ASCII.GetBytes(token) );

            // store the hashed result as an array of bytes
            byte[] hashedToken = md5_encryptor.Hash;

            StringBuilder encryptedToken = new StringBuilder();

            for (int i = 0; i < hashedToken.Length; i++)
            {
                //change it into 2 hexadecimal digits
                //for each byte
                encryptedToken.Append( hashedToken[i].ToString("x2") );
            }

            return encryptedToken.ToString();
        }
}
