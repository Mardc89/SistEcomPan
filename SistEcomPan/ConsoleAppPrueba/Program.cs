// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography;
using System.Text;



EncriptService nuevo=new EncriptService();
var nueva=nuevo.ConvertirSha256("");
Console.WriteLine("la nueva es:" + nueva);



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


}