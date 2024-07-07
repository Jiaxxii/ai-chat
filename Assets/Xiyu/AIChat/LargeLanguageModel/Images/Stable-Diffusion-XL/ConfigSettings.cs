using System;
using Newtonsoft.Json;
using UnityEngine;
using Xiyu.AIChat.LargeLanguageModel.Service;

namespace Xiyu.AIChat.LargeLanguageModel.Images.Stable_Diffusion_XL
{
    public class ConfigSettings : ConfigSetting<RequestBody>
    {
    }


    [Serializable]
    public class RequestBody
    {
        [SerializeField] [TextArea(5, 10)] private string prompt;
        [SerializeField] [TextArea(5, 10)] private string negativePrompt;

        [SerializeField] [Range(1, 4)] private int imageCount = 1;

        [SerializeField] [Range(10, 50)] private int steps = 20;

        [SerializeField] private ImageSize size = ImageSize.Size768X768;

        [SerializeField] private Style style = Stable_Diffusion_XL.Style.漫画;


        public string Prompt
        {
            get => prompt;
            set => prompt = value;
        }

        public string NegativePrompt
        {
            get => negativePrompt;
            set => negativePrompt = value;
        }

        [JsonProperty(PropertyName = "n")]
        public int ImageCount
        {
            get => imageCount;
            set => imageCount = value;
        }

        public int Steps
        {
            get => steps;
            set => steps = value;
        }

        public string Style
        {
            get => style switch
            {
                Stable_Diffusion_XL.Style.基础风格 => "Base",
                Stable_Diffusion_XL.Style.模型3D => "3D Model",
                Stable_Diffusion_XL.Style.模拟胶片 => "Analog Film",
                Stable_Diffusion_XL.Style.动漫 => "Anime",
                Stable_Diffusion_XL.Style.电影 => "Cinematic",
                Stable_Diffusion_XL.Style.漫画 => "Comic Book",
                Stable_Diffusion_XL.Style.工艺黏土 => "Craft Clay",
                Stable_Diffusion_XL.Style.数字艺术 => "Digital Art",
                Stable_Diffusion_XL.Style.增强 => "Enhance",
                Stable_Diffusion_XL.Style.幻想艺术 => "Fantasy Art",
                Stable_Diffusion_XL.Style.等距风格 => "Isometric",
                Stable_Diffusion_XL.Style.线条艺术 => "Line Art",
                Stable_Diffusion_XL.Style.低多边形 => "Lowpoly",
                Stable_Diffusion_XL.Style.霓虹朋克 => "Neonpunk",
                Stable_Diffusion_XL.Style.折纸 => "Origami",
                Stable_Diffusion_XL.Style.摄影 => "Photographic",
                Stable_Diffusion_XL.Style.像素艺术 => "Pixel Art", // 注意这里原代码写的是 "Pixel Ar"，可能是个笔误  
                Stable_Diffusion_XL.Style.纹理 => "Texture",
                _ => "Base"
            };
            set => style = value switch
            {
                "Base" => Stable_Diffusion_XL.Style.基础风格,
                "3D Model" => Stable_Diffusion_XL.Style.模型3D,
                "Analog Film" => Stable_Diffusion_XL.Style.模拟胶片,
                "Anime" => Stable_Diffusion_XL.Style.动漫,
                "Cinematic" => Stable_Diffusion_XL.Style.电影,
                "Comic Book" => Stable_Diffusion_XL.Style.漫画,
                "Craft Clay" => Stable_Diffusion_XL.Style.工艺黏土,
                "Digital Art" => Stable_Diffusion_XL.Style.数字艺术,
                "Enhance" => Stable_Diffusion_XL.Style.增强,
                "Fantasy Art" => Stable_Diffusion_XL.Style.幻想艺术,
                "Isometric" => Stable_Diffusion_XL.Style.等距风格,
                "Line Art" => Stable_Diffusion_XL.Style.线条艺术,
                "Lowpoly" => Stable_Diffusion_XL.Style.低多边形,
                "Neonpunk" => Stable_Diffusion_XL.Style.霓虹朋克,
                "Origami" => Stable_Diffusion_XL.Style.折纸,
                "Photographic" => Stable_Diffusion_XL.Style.摄影,
                "Pixel Art" => Stable_Diffusion_XL.Style.像素艺术, // 修正了这里的值  
                "Texture" => Stable_Diffusion_XL.Style.纹理,
                _ => Stable_Diffusion_XL.Style.基础风格 // 默认情况映射到基础风格  
            };
        }


        public string Size
        {
            get
            {
                var sizeStr = size.ToString();
                return sizeStr.Substring(4, sizeStr.Length - 4).ToLower();
            }
            set => size = Enum.Parse<ImageSize>($"Size{value.ToUpper()}");
        }
    }

    public enum ImageSize
    {
        // 适合头像
        Size768X768,
        Size1024X1024,
        Size1536X1536,
        Size2048X2048,
        //

        // 适用文章配图
        Size1024X768,
        Size2048X1536,
        //

        // 适用海报传单
        Size768X1024,
        Size1536X2048,
        // 

        // 适用电脑壁纸
        Size1024X576,
        Size2048X1152,
        // 

        // 适用海报传单
        Size576X1024,
        Size1152X2048,
        //
    }

    public enum Style
    {
        基础风格,
        模型3D,
        模拟胶片,
        动漫,
        电影,
        漫画,
        工艺黏土,
        数字艺术,
        增强,
        幻想艺术,
        等距风格,
        线条艺术,
        低多边形,
        霓虹朋克,
        折纸,
        摄影,
        像素艺术,
        纹理
    }
}