// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography;
using System.Text;



EncriptService nuevo=new EncriptService();
var nueva=nuevo.EncriptarPassword("er45");
Console.WriteLine("la nueva es:" + nueva);
var vieja = nuevo.DesencriptarPassword(nueva);
Console.WriteLine("la nueva es:" + vieja);



public class EncriptService 
{
    public string ConvertirSha256(string texto)
    {
        StringBuilder sb = new StringBuilder();

        using (SHA256 hash = SHA256Managed.Create())
        {
            Encoding enc = Encoding.UTF8;

            byte[] result = hash.ComputeHash(enc.GetBytes(texto));

            foreach (byte b in result)
            {
                sb.Append(b.ToString("X2"));
            }

        }
        return sb.ToString();

    }




public string EncriptarPassword(string password)
{
    using (Aes aesAlg = Aes.Create())
    {
        aesAlg.Key = Encoding.UTF8.GetBytes("A234567890123456789012345678901a"); // Clave secreta (debería ser almacenada de forma segura)
        aesAlg.IV = new byte[16]; // Vector de inicialización (puede ser generado aleatoriamente)

        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        byte[] encryptedBytes;
        using (var msEncrypt = new System.IO.MemoryStream())
        {
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                using (var swEncrypt = new System.IO.StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(password);
                }
                encryptedBytes = msEncrypt.ToArray();
            }
        }
        return Convert.ToBase64String(encryptedBytes);
    }
}

public string DesencriptarPassword(string encryptedPassword)
{
    using (Aes aesAlg = Aes.Create())
    {
        aesAlg.Key = Encoding.UTF8.GetBytes("A234567890123456789012345678901a"); // La misma clave usada para encriptar
        aesAlg.IV = new byte[16]; // El mismo vector de inicialización usado para encriptar

        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        byte[] encryptedBytes = Convert.FromBase64String(encryptedPassword);
        string decryptedPassword = null;

        using (var msDecrypt = new System.IO.MemoryStream(encryptedBytes))
        {
            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            {
                using (var srDecrypt = new System.IO.StreamReader(csDecrypt))
                {
                    decryptedPassword = srDecrypt.ReadToEnd();
                }
            }
        }
        return decryptedPassword;
    }
}
}