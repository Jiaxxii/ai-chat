using System.IO;

namespace Xiyu.Application
{
    public static class ApplicationData
    {
        public static string MainDataDirectory => Path.Combine(UnityEngine.Application.persistentDataPath);

        public static string LoggerConfigSettingFilePath => Path.Combine(LoggerPath, "configseting.json");
        public static string LoggerPath => Path.Combine(MainDataDirectory, "logs");

        public static string PromptPath => Path.Combine(MainDataDirectory, "Prompt");
    }
}