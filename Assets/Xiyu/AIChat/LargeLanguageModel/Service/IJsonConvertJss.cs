using System;
using Newtonsoft.Json;

namespace Xiyu.AIChat.LargeLanguageModel.Service
{
    public interface IJsonConvertJss
    {
        JsonSerializerSettings RequestBodyJss { get; }

        JsonSerializerSettings ResponseJss { get; }
    }
}