using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System;

public static class SecurePlayerPrefs
{
    private static readonly string EncryptionKey = "key";
    public static void SetString(string key, string value)
    {
        string encryptedValue = Encrypt(value);
        PlayerPrefs.SetString(key, encryptedValue);
    }
    public static string GetString(string key)
    {
        string encryptedValue = PlayerPrefs.GetString(key);
        return Decrypt(encryptedValue);
    }
    public static void SetInt(string key, int value)
    {
        string valueString = value.ToString();
        SetString(key, valueString);
    }
    public static int GetInt(string key)
    {
        string valueString = GetString(key);
        if (int.TryParse(valueString, out int value))
        {
            return value;
        }
        return 0;
    }

    private static string Encrypt(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32)); 
            aes.IV = new byte[16];

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return Convert.ToBase64String(encryptedBytes);
        }
    }

    private static string Decrypt(string encryptedText)
    {
        try
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32));
                aes.IV = new byte[16];

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
        catch
        {
            return "";
        }
    }
}
