using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Xiyu.AIChat.LargeLanguageModel.Service;

namespace Xiyu.AIChat.LargeLanguageModel.Images.Stable_Diffusion_XL
{
    public class Service : LargeLanguageModel<RequestBody, ResponseResult>
    {
    }

    [Serializable]
    public class ResponseResult : ServiceResponse<Sprite[]>
    {
        [SerializeField] private List<ImageData> data;

        [JsonIgnore] public override string Result { get; set; }

        public List<ImageData> Data
        {
            get => data;
            set => data = value;
        }

        public override IEnumerator SendResultCoroutine(Action<Sprite[]> onComplete)
        {
            var targetDirectory = $"{Environment.CurrentDirectory}/Images/{DateTime.Now:yy_MM_dd}";

            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            var result = new List<Sprite>();
            foreach (var imageBytes in Data.Select(base64Ima => Convert.FromBase64String(base64Ima.B64Image)))
            {
                var tex = new Texture2D(2, 2);

                if (tex.LoadImage(imageBytes))
                {
                    tex.Apply();

                    var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(.5F, .5F));
                    result.Add(sprite);
                    yield return new WaitForEndOfFrame();
                }
                else
                {
                    Debug.LogError($"无法加载图片! (length:{imageBytes.Length})");
                }
            }

            onComplete?.Invoke(result.ToArray());
        }
    }

    [Serializable]
    public class ImageData
    {
        [SerializeField] private string b64Image;
        [SerializeField] private int index;

        public string B64Image
        {
            get => b64Image;
            set => b64Image = value;
        }

        public int Index
        {
            get => index;
            set => index = value;
        }
    }
}