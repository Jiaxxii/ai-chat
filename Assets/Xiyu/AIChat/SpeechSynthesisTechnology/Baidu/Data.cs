using System;
using UnityEngine;

namespace Xiyu.AIChat.SpeechSynthesisTechnology.Baidu
{
    [Serializable]
    public class Config
    {
        [Tooltip("用户唯一标识，用来计算UV值。建议填写能区分用户的机器 MAC 地址或 IMEI 码，长度为60字符以内")] [SerializeField]
        private string cuId = "x3Xp6OKERfotQw690PcPPll6eXJkPFhE";


        [Tooltip("客户端类型选择，web端填写固定值1")] [SerializeField]
        private string ctp = "1";

        /// <summary>
        /// 
        /// </summary>
        [Tooltip("固定值zh。语言选择,目前只有中英文混合模式，填写固定值zh")] [SerializeField]
        private string language = "zh";

        /// <summary>
        /// 
        /// </summary>
        [Tooltip("语速，取值0-15，默认为5中语速")] [SerializeField]
        private int speed = 5;

        /// <summary>
        /// 
        /// </summary>
        [Tooltip("音调，取值0-15，默认为5中语调")] [SerializeField]
        private int pitch = 5;

        /// <summary>
        /// 
        /// </summary>
        [Tooltip("音量，基础音库取值0-9，精品音库取值0-15，默认为5中音量（取值为0时为音量最小值，并非为无声）")] [SerializeField]
        private int volume = 5;

        /// <summary>
        /// 
        /// </summary>
        [Tooltip("度小宇=1，度小美=0，度逍遥（基础）=3，度丫丫=4")] [SerializeField]
        private int soundLibrary = 3;


        [Tooltip("3为mp3格式(默认)； 4为pcm-16k；5为pcm-8k；6为wav（内容同pcm-16k）;" +
                 "注意aue=4或者6是语音识别要求的格式，但是音频内容不是语音识别要求的自然人发音，所以识别效果会受影响。")]
        [SerializeField]
        private int aue = 3;


        public string CuId => cuId;

        public string Ctp => ctp;

        public string Language => language;

        public int Speed => speed;

        public int Pitch => pitch;

        public int Volume => volume;

        public int SoundLibrary => soundLibrary;

        public int Aue => aue;
    }

    [Serializable]
    public class RequestData
    {
        /// <summary>
        /// 合成的文本，文本长度必须小于1024GBK字节。建议每次请求文本不超过120字节，约为60个汉字或者字母数字。
        /// 请注意计费统计依据：120个GBK字节以内（含120个）记为1次计费调用；每超过120个GBK字节则多记1次计费调用。
        /// </summary>
        public string Tex { get; set; }

        /// <summary>
        /// 开放平台获取到的开发者[access_token]获取 Access Token "access_token")
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 用户唯一标识，用来计算UV值。建议填写能区分用户的机器 MAC 地址或 IMEI 码，长度为60字符以内
        /// </summary>
        public string CuID { get; set; } = "x3Xp6OKERfotQw690PcPPll6eXJkPFhE";

        /// <summary>
        /// 客户端类型选择，web端填写固定值1
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string CTP { get; set; } = "1";

        /// <summary>
        /// 固定值zh。语言选择,目前只有中英文混合模式，填写固定值zh
        /// </summary>
        public string Language { get; set; } = "zh";

        /// <summary>
        /// 语速，取值0-15，默认为5中语速
        /// </summary>
        public int Speed { get; set; } = 5;

        /// <summary>
        /// 音调，取值0-15，默认为5中语调
        /// </summary>
        public int Pitch { get; set; } = 5;

        /// <summary>
        /// 音量，基础音库取值0-9，精品音库取值0-15，默认为5中音量（取值为0时为音量最小值，并非为无声）
        /// </summary>
        public int Volume { get; set; } = 5;

        /// <summary>
        /// 度小宇=1，度小美=0，度逍遥（基础）=3，度丫丫=4
        /// </summary>

        public int SoundLibrary { get; set; } = 3;

        /// <summary>
        /// 3为mp3格式(默认)； 4为pcm-16k；5为pcm-8k；6为wav（内容同pcm-16k）;
        /// 注意aue=4或者6是语音识别要求的格式，但是音频内容不是语音识别要求的自然人发音，所以识别效果会受影响。
        /// </summary>

        public int Aue { get; set; } = 3;
    }

    public enum SoundLibraryType
    {
        // ReSharper disable once IdentifierTypo
        DuXiaoYu = 1,

        // ReSharper disable once IdentifierTypo
        DuXiaoMei = 0,

        // ReSharper disable once IdentifierTypo
        DuXiaoYao = 3,

        // ReSharper disable once IdentifierTypo
        DuYaYa = 4
    }
}