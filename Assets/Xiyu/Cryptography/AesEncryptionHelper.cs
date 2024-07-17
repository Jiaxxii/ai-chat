using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Xiyu.Cryptography
{
    public static class AesEncryptionHelper
    {
        public static byte[] CreateKey(int size)
        {
            using var rng = RandomNumberGenerator.Create();

            var bytes = new byte [size];

            rng.GetBytes(bytes);

            return bytes;
        }

        public static string CreateKeyToBase64(int size)
        {
            var bytes = CreateKey(size);
            return Convert.ToBase64String(bytes);
        }

        public static byte[] Base64ToKey(string base64Content) => Convert.FromBase64String(base64Content);

        /// <summary>
        /// 加密数据
        /// </summary>
        /// <param name="original"></param>
        /// <param name="key"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<string> EncryptorAsync(string original, byte[] key, CancellationToken cancellationToken = default)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = new byte[16];

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var memoryStream = new MemoryStream();
            await using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                await using (var sw = new StreamWriter(cryptoStream))
                {
                    await sw.WriteAsync(original.AsMemory(), cancellationToken);
                }
            }


            return Convert.ToBase64String(memoryStream.ToArray());
        }

        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="key"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<string> DecryptAsync(string cipherText, byte[] key, CancellationToken cancellationToken = default)
        {
            try
            {
                using var aes = Aes.Create();
                aes.Key = key;
                aes.IV = new byte[16];

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using var memoryStream = new MemoryStream(Convert.FromBase64String(cipherText));
                await using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using var sr = new StreamReader(cryptoStream);
                    var sb = new StringBuilder();
                    while (await sr.ReadLineAsync() is { } line)
                    {
                        sb.Append(line);
                    }

                    return sb.ToString();
                }
            }
            catch (OperationCanceledException e)
            {
                throw new OperationCanceledException(e.Message);
            }
            catch (Exception e)
            {
                throw new CryptographicException($"解密数据时发生错误! {e.Message}");
            }
        }

        /// <summary>
        /// 加密数据
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        /// <param name="jsonSerializerSettings"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<string> SerializeObjectEncryptorAsync(object obj, byte[] key, JsonSerializerSettings jsonSerializerSettings = null,
            CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonConvert.SerializeObject(obj, jsonSerializerSettings);
            return await EncryptorAsync(jsonContent, key, cancellationToken);
        }

        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="key"></param>
        /// <param name="jsonSerializerSettings"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T> DeserializeObjectDecryptAsync<T>(string cipherText, byte[] key, JsonSerializerSettings jsonSerializerSettings = null,
            CancellationToken cancellationToken = default)
        {
            var jsonContent = await DecryptAsync(cipherText, key, cancellationToken);
            var instance = JsonConvert.DeserializeObject<T>(jsonContent, jsonSerializerSettings);

            return instance;
        }
    }
}