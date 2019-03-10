using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using ModSettings;
using UnityEngine;

namespace JsonModSettings
{

    public abstract class JsonModSettingsBase<T> : ModSettingsBase
        where T : ModSettingsBase, new()
    {
        public static T Instance;

        protected override void OnConfirm()
        {
            JsonModSettingsLoader.Save(Instance);
        }
    }

    internal static class JsonModSettingsLoader
    {
        private static readonly string MOD_NAME = Assembly.GetExecutingAssembly().GetName().Name;
        private static readonly string MODS_FOLDER_PATH = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string SETTINGS_PATH = Path.Combine(MODS_FOLDER_PATH, MOD_NAME + ".json");

        public static T Load<T>() where T : ModSettingsBase, new()
        {
            var settings = LoadOrCreateSettings<T>();
            settings.AddToModSettings(AddSpacesToModName(MOD_NAME));

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            Log("Version " + version + " loaded!");
            return settings;
        }

        public static void Save<T>(T settings)
        {
            try
            {
                string json = JsonUtility.ToJson(settings, prettyPrint: true);
                File.WriteAllText(SETTINGS_PATH, json, System.Text.Encoding.UTF8);
                Log("Config file saved to " + SETTINGS_PATH);
            }
            catch (Exception ex)
            {
                LogError("Error while trying to write config file:");
                Debug.LogException(ex);
            }
        }

        private static string AddSpacesToModName(string modName)
        {
            // Makes "MyModName" -> "My Mod Name"
            return Regex.Replace(modName, "(\\B[A-Z])", " $1");
        }

        private static T LoadOrCreateSettings<T>() where T : new()
        {
            if (!File.Exists(SETTINGS_PATH))
            {
                Log("Settings file did not exist, using default settings.");
                return new T();
            }

            try
            {
                string json = File.ReadAllText(SETTINGS_PATH, System.Text.Encoding.UTF8);
                return JsonUtility.FromJson<T>(json);
            }
            catch (Exception ex)
            {
                LogError("Error while trying to read config file:");
                Debug.LogException(ex);

                // Re-throw to make error show up in main menu
                throw new IOException("Error while trying to read config file", ex);
            }
        }

        internal static void Log(string message)
        {
            Debug.Log("[" + MOD_NAME + "] " + message);
        }

        internal static void LogError(string message)
        {
            Debug.LogError("[" + MOD_NAME + "] " + message);
        }
    }
}
