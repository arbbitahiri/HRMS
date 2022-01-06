using System;
using System.IO;
using System.Security.Cryptography;

namespace HRMS.Utilities.Security;

public static class CryptoSecurity
{
    private readonly static string IV = "ZCPpTydDDxl+alvCal+2xw==";
    private readonly static string Key = "evnuHp0Fn3WvnQ91SssExYX7F7hGCFyWz/5yW0TW0ag=";

    public static string Encrypt(object obj)
    {
        using Aes aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = Convert.FromBase64String(Key);
        aes.IV = Convert.FromBase64String(IV);

        try
        {
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(obj);
            }
            return Convert.ToBase64String(msEncrypt.ToArray()).Replace("+", "(").Replace("/", ")");
        }
        catch
        {
            return null;
        }
    }

    public static T Decrypt<T>(string obj)
    {
        obj = obj.Replace("(", "+").Replace(")", "/").Replace(" ", "+").Replace("%2F", "/");

        using Aes aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = Convert.FromBase64String(Key);
        aes.IV = Convert.FromBase64String(IV);

        try
        {
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var msDecrypt = new MemoryStream(Convert.FromBase64String(obj));
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            if (typeof(T) == typeof(int))
            {
                return (T)Convert.ChangeType(Convert.ToInt32(srDecrypt.ReadToEnd()), typeof(T));
            }
            else
            {
                return (T)Convert.ChangeType(srDecrypt.ReadToEnd(), typeof(T));
            }
        }
        catch
        {
            return (T)Convert.ChangeType(0, typeof(T));
        }
    }
}
