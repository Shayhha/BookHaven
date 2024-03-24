using System.Security.Cryptography;
using System.Text;
namespace BookHeaven.Models
{
    public class Encryption
    {

        /// <summary>
        /// Function to generate SHA-256 hash for strings
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string toSHA256(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] hash = sha256Hash.ComputeHash(bytes);
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Function for writing users aes key in txt file using userId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="encryptionKey"></param>
        public static void writeKeyToFile(int userId, byte[] encryptionKey)
        {
            int secretIndex = (userId << 3) ^ userId; //this represents the secret index of our user in txt file
            string encryptedSecretIndex = toSHA256(secretIndex.ToString()); //we encrypt the secretIndex in sha256 for extra diffusion
            string encryptionKeyString = Convert.ToBase64String(encryptionKey); //convert byte array to string
            Console.WriteLine(BitConverter.ToString(encryptionKey).Replace("-", " "));
            //open or create the text file and append to the end
            using (StreamWriter writer = new StreamWriter("appData.txt", true))
            {
                writer.WriteLine(encryptedSecretIndex + ":" + encryptionKeyString); //write the secretIndex and encryptionKey to the file
            }
        }

        /// <summary>
        /// Function for retrieving user's aes key from txt file using its userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Returns aes key as byte array</returns>
        public static byte[]? getKeyFromFile(int userId)
        {
            int secretIndex = (userId << 3) ^ userId; //this represents the secret index of our user in txt file
            string encryptedSecretIndex = toSHA256(secretIndex.ToString()); //get the sha256 representation of the index

            //open the text file and read line by line
            using (StreamReader reader = new StreamReader("appData.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(':'); //split the string 
                    if (parts.Length == 2 && parts[0] == encryptedSecretIndex) //if true we found our secret index in txt file
                    {
                        //convert string back to byte array and return
                        Console.WriteLine(BitConverter.ToString(Convert.FromBase64String(parts[1])).Replace("-", " ")); //print key
                        return Convert.FromBase64String(parts[1]); //return aes key for user
                    }
                }
            }
            //if no matching secretIndex is found, return null
            return null;
        }

        /// <summary>
        /// Function for generating aes key for AES-256 encryption
        /// </summary>
        /// <returns>Returns aes key byte array</returns>
        public static byte[] generateAESKey()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] key = new byte[32]; //generate random 256 bit AES
                rng.GetBytes(key); //generate the key
                return key;
            }
        }

        /// <summary>
        /// Function for encrypting a given string with aes key using AES-256
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key"></param>
        /// <returns>Returns encrypted string in hex</returns>
        public static string encryptAES(string plainText, byte[] key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.Key = key;
                aes.Mode = CipherMode.ECB;

                ICryptoTransform encryptor = aes.CreateEncryptor();

                byte[] encryptedBytes;
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encryptedBytes = msEncrypt.ToArray();
                    }
                }

                return BitConverter.ToString(encryptedBytes).Replace("-", "");
            }
        }

        /// <summary>
        /// Function for decrypting a given hex string and aes key using AES-256
        /// </summary>
        /// <param name="encryptedHex"></param>
        /// <param name="key"></param>
        /// <returns>Returns decrypted string</returns>
        public static string? decryptAES(string encryptedHex, byte[] key)
        {
            try
            {
                byte[] encryptedBytes = hexToByteArray(encryptedHex);

                using (Aes aes = Aes.Create())
                {
                    aes.KeySize = 256;
                    aes.Key = key;
                    aes.Mode = CipherMode.ECB;

                    ICryptoTransform decryptor = aes.CreateDecryptor();

                    using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Decryption failed: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Function for converting hex string to byte array 
        /// </summary>
        /// <param name="hex"></param>
        /// <returns>Returns hex string represented as byte array </returns>
        private static byte[] hexToByteArray(string hex)
        {
            int length = hex.Length;
            byte[] byteArray = new byte[length / 2]; //each byte in the array represents two characters in the hex string

            for (int i = 0; i < length; i += 2)
            {
                string byteString = hex.Substring(i, 2); //extract two characters at a time
                byte b = Convert.ToByte(byteString, 16); //convert the extracted string to a byte in base 16 (hexadecimal)
                byteArray[i / 2] = b; //assign the byte to the corresponding position in the byte array
            }

            return byteArray;
        }
    }
}
