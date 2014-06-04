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
	
	
		public static string Decrypt(string cipherString, bool useHashing)
		{
    		byte[] keyArray;
    		//get the byte code of the string

    		byte[] toEncryptArray = Convert.FromBase64String(cipherString);

    		System.Configuration.AppSettingsReader settingsReader = 
                                        new AppSettingsReader();
    		//Get your key from config file to open the lock!
    		string key = (string)settingsReader.GetValue("SecurityKey", 
            
        	//if hashing was used get the hash code with regards to your key
        	MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
        	keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
        	//release any resource held by the MD5CryptoServiceProvider

       		 hashmd5.Clear();
    		
    		TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
    		//set the secret key for the tripleDES algorithm
    		tdes.Key = keyArray;
    		//mode of operation. there are other 4 modes. 
    		//We choose ECB(Electronic code Book)

    		tdes.Mode = CipherMode.ECB;
    		//padding mode(if any extra byte added)
    		tdes.Padding = PaddingMode.PKCS7;

    		ICryptoTransform cTransform = tdes.CreateDecryptor();
    		byte[] resultArray = cTransform.TransformFinalBlock(
                         toEncryptArray, 0, toEncryptArray.Length);
    		//Release resources held by TripleDes Encryptor                
    		tdes.Clear();
    		//return the Clear decrypted TEXT
    		return UTF8Encoding.UTF8.GetString(resultArray);
		}
}
