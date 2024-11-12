using System.IO;

namespace Xiyu.Application
{
    public static class ApplicationData
    {
        public static string MainDataDirectory => Path.Combine(UnityEngine.Application.persistentDataPath);

        public static string LoggerPath => Path.Combine(MainDataDirectory, "logs");

        public static string PromptPath => Path.Combine(MainDataDirectory, "Prompt");

        public static string BodyInfoSettingsPath => Path.Combine(MainDataDirectory, "body informations");


        public static string LiveRoomInfoPath => Path.Combine(MainDataDirectory, "live room informations");

        public static string EmotionsPath => Path.Combine(MainDataDirectory, "emotions");

        public static string SpeechHistoryPath => Path.Combine(MainDataDirectory, "speechs");
    }
}