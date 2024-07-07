using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Xiyu.AIChat;

namespace ASceneTest
{
    public class InputTest : MonoBehaviour
    {
        [SerializeField] private string authorization;

        private IEnumerator Start()
        {
            var token = string.Empty;
            yield return AccessToken.GetTokenAsync("wm1NliR3oVY2HfA0eiJyCGk6", "7adBRwr4vYFJqJXZphAYkV5Li8tWFBdc", t => token = t.AccessToKen);


            var postUrl = $"https://aip.baidubce.com/rpc/2.0/ai_custom/v1/wenxinworkshop/embeddings/embedding-v1?access_token={token}";


            using var request = new UnityWebRequest(postUrl, "POST");

            var jsonData = JsonConvert.SerializeObject(new JObject
            {
                ["input"] = new JArray
                {
                    "我很喜欢这部电影", "我很讨厌这部电影"
                }
            });
            byte[] data = System.Text.Encoding.UTF8.GetBytes(jsonData);

            request.uploadHandler = new UploadHandlerRaw(data);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            var list = new List<float[]>();
            if (request.responseCode == 200)
            {
                var msg = request.downloadHandler.text;
                list.AddRange(JObject.Parse(msg)["data"].Values<JObject>().Select(dataItem => dataItem["embedding"].Values<float>().ToArray()));
            }

            var result = CalculateCosineSimilarity(list[0], list[1]);
            _ = result;            
            double CalculateCosineSimilarity(float[] vectorA, float[] vectorB)  
            {  
                // 确保两个向量长度相同  
                if (vectorA.Length != vectorB.Length)  
                {  
                    throw new ArgumentException("Vectors must have the same length");  
                }  
  
                float dotProduct = 0.0f;  
                float magnitudeA = 0.0f;  
                float magnitudeB = 0.0f;  
  
                // 计算点积  
                for (int i = 0; i < vectorA.Length; i++)  
                {  
                    dotProduct += vectorA[i] * vectorB[i];  
                    magnitudeA += Mathf.Pow(vectorA[i], 2);  
                    magnitudeB += Mathf.Pow(vectorB[i], 2);  
                }  
  
                // 计算模长  
                double magA = Mathf.Sqrt(magnitudeA);  
                double magB = Mathf.Sqrt(magnitudeB);  
  
                // 避免除以零的错误  
                if (magA == 0 || magB == 0)  
                {  
                    return 0;  
                }  
  
                // 计算余弦相似度  
                double cosineSimilarity = dotProduct / (magA * magB);  
                return cosineSimilarity;  
            }
            
        }
    }
}