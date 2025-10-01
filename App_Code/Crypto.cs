using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;


public class Crypto
{
    private static readonly TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
    private static readonly MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
    private static readonly string key = "sg75b79-nj48dh02";

    public static byte[] MD5Hash(string value)
    {
        return MD5.ComputeHash(Encoding.ASCII.GetBytes(value));
    }

    public static string Encrypt(string stringToEncrypt)
    {
        DES.Key = MD5Hash(key);
        DES.Mode = CipherMode.ECB;
        byte[] buffer = Encoding.ASCII.GetBytes(stringToEncrypt);
        return Convert.ToBase64String(DES.CreateEncryptor().TransformFinalBlock(buffer, 0, buffer.Length));
    }

    public static string Decrypt(string encryptedString)
    {
        try
        {
            DES.Key = MD5Hash(key);
            DES.Mode = CipherMode.ECB;
            byte[] buffer = Convert.FromBase64String(encryptedString);
            return Encoding.ASCII.GetString(DES.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length));
        }
        catch (Exception ex)
        {
            return DBNull.Value.ToString();
        }
    }
}