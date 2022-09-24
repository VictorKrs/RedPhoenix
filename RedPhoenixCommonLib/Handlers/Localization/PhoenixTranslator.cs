using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedPhoenix.Common.Handlers.Localization.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace RedPhoenix.Common.Handlers.Localization
{
    /// <summary>
    /// Translates by the key. The values ​​for each key should be 
    /// recorded in the JSON file (each language has its own) in the LanguageDir
    /// </summary>
    public static class PhoenixTranslator
    {
        #region Consts
        private const string DEFAULT_LANGUE = "RU";
        private const string LANG_FILENAME_PATTERN = "*.json";
        #endregion

        #region Private fields
        private static Dictionary<string, JObject> _dictionary = new Dictionary<string, JObject>();
        #endregion

        #region Properties
        public static string LanguageDir { get; set; } = PhoenixFunctions.ProgramLocation + "\\langs";

        internal static ObservableCollection<string> AvailableLanguages { get; private set; } = new ObservableCollection<string>();

        public static LocalizationInfo GlobalLocalizaitionInfo { get; set; } = new LocalizationInfo();
        #endregion

        #region Methods
        #region Initialization
        public static void Init()
        {
            Clear();

            if (!Directory.Exists(LanguageDir))
                throw new Exception("Languages directory not found: " + LanguageDir);

            var files = Directory.GetFiles(LanguageDir, LANG_FILENAME_PATTERN);

            if (files == null || files.Length == 0)
                throw new Exception("Not found languages files in directory: " + LanguageDir + "; Filename pattern: " + LANG_FILENAME_PATTERN);

            foreach (var langFile in files)
                LoadLanguage(langFile);

            if (AvailableLanguages.Count != 0)
                SelectLanguage();
        }

        private static void Clear()
        {
            AvailableLanguages.Clear();
            GlobalLocalizaitionInfo.Clear();
        }

        private static void LoadLanguage(string langFile)
        {
            try
            {
                var langName = Path.GetFileNameWithoutExtension(langFile).ToUpper();

                using (var fileStream = File.OpenText(langFile))
                using (var jReader = new JsonTextReader(fileStream))
                {
                    if (_dictionary.ContainsKey(langName))
                        throw new Exception("Double language: " + langName);

                    AvailableLanguages.Add(langName);

                    _dictionary.Add(langName, JObject.Load(jReader));
                }
            }
            catch (Exception) { throw; }
        }

        private static void SelectLanguage()
        {
            if (AvailableLanguages.IndexOf(DEFAULT_LANGUE) >= 0)
                GlobalLocalizaitionInfo.SelectedLanguage = DEFAULT_LANGUE;
            else
                GlobalLocalizaitionInfo.SelectedLanguage = AvailableLanguages[0];
        }
        #endregion

        #region Translate
        public static string TranslateMessage(string mKey, params string[] mValues) =>
            TranslateMessage(GlobalLocalizaitionInfo, mKey, mValues);

        public static string TranslateMessage(LocalizationInfo lInfo, string mKey, params string[] mValues)
        {
            try
            {
                var lang = lInfo.SelectedLanguage;

                if (!HasTranslating(lang, mKey))
                    return GetNotFoundedMessge(mKey, mValues);

                var message = _dictionary[lang][mKey].ToString();

                if (mValues == null || mValues.Length == 0)
                    return message;

                return string.Format(message, mValues);
            }
            catch (Exception) { throw; }
        }

        private static bool HasTranslating(string lang, string mKey)
        {
            return !string.IsNullOrEmpty(lang) && _dictionary.ContainsKey(lang) && _dictionary[lang].ContainsKey(mKey);
        }

        private static string GetNotFoundedMessge(string mKey, string[] mValues)
        {
            if (mValues == null || mValues.Length == 0)
                return mKey;

            return string.Join("; ", mKey, string.Join("; ", mValues));
        }
        #endregion
        #endregion
    }
}
