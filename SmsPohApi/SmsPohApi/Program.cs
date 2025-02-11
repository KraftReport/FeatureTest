using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SmsPohApi
{
    public class Program
    {
        /*static void Main(string[] args)
        {
            var apiUri = "https://v3.smspoh.com/api/rest/send";

            var apiKey = "JATv4QuGdTkE4Wqx";

            var apiSecret = "RLyJi9a_Df74Il_N";

            var byteData = System.Text.Encoding.UTF8.GetBytes($"{apiKey}:{apiSecret}");

            var base64Token = Convert.ToBase64String(byteData);

            var payload = new
            {
                from = "TABLE",
                to = "09971147172",
                message = "see entire picture effortlessly"
            };

            var jsonPayload = JsonSerializer.Serialize(payload);

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {base64Token}");

                    var response = httpClient.PostAsync(apiUri, content).Result;

                    var responseText = response.Content.ReadAsStringAsync().Result;

                    Console.WriteLine(responseText);

                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            } 
        }*/

        static void Main(string[] args)
        {
            var apiUri = "https://v3.smspoh.com/api/rest/send";

            var apiKey = "JATv4QuGdTkE4Wqx";

            var apiSecret = "RLyJi9a_Df74Il_N";

            var byteData = System.Text.Encoding.UTF8.GetBytes($"{apiKey}:{apiSecret}");

            var base64Token = Convert.ToBase64String(byteData);

            var encryptedData = EncryptMessage("inner peace", "password");

            Console.WriteLine(Convert.ToBase64String(encryptedData.Item2));

            Console.WriteLine(DecryptMessage(encryptedData.Item3, encryptedData.Item2, "password"));

            //var payload = new
            //{
            //    from = "TABLE",
            //    to = "09971147172",
            //    message = encryptedData.Item2,
            //    encrypt = 1,
            //    encryptKey = encryptedData.Item1,
            //};

            //var jsonPayload = JsonSerializer.Serialize(payload);

            //var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            //using (var httpClient = new HttpClient())
            //{
            //    try
            //    {
            //        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {base64Token}");

            //        var response = httpClient.PostAsync(apiUri, content).Result;

            //        var responseText = response.Content.ReadAsStringAsync().Result;

            //        Console.WriteLine(responseText);

            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.Message);
            //    }
            //}

        }

        public static byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];
            using(var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public static (string, byte[], byte[]) EncryptMessage(string rawData,string password)
        {
            var salt = GenerateSalt();
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000))
            {
                var key = pbkdf2.GetBytes(32);
                using(var aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.GenerateIV();
                    aes.Mode = CipherMode.CBC;

                    using(var encryptor = aes.CreateEncryptor())
                    {
                        var encrypted = encryptor.TransformFinalBlock(Encoding.UTF8.GetBytes(rawData), 0, rawData.Length);
                        var result = new byte[aes.IV.Length + encrypted.Length];
                        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
                        Buffer.BlockCopy(encrypted,0,result,aes.IV.Length,encrypted.Length);

                        return (Convert.ToBase64String(key).Substring(0,10),result,salt);
                    }
                }
            }
        }
 

        public static string DecryptMessage(byte[] salt, byte[] encryptedMessage, string password)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000))
            {
                byte[] key = pbkdf2.GetBytes(32);

                using (var aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.Mode = CipherMode.CBC;

                    // Extract IV
                    byte[] iv = new byte[16];
                    byte[] cipherText = new byte[encryptedMessage.Length - iv.Length];

                    Buffer.BlockCopy(encryptedMessage, 0, iv, 0, iv.Length);
                    Buffer.BlockCopy(encryptedMessage, iv.Length, cipherText, 0, cipherText.Length);

                    aes.IV = iv;

                    using (var decryptor = aes.CreateDecryptor())
                    {
                        byte[] decrypted = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
                        return Encoding.UTF8.GetString(decrypted);
                    }
                }
            }
        }
    }
}
