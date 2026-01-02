using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace GOGE.Utils
{
    public enum Language
    {
        English,
        German
    }

    public static class Localization
    {
        private static readonly Dictionary<string, string> _strings = new();
        private static readonly object _lock = new();
        private static Language _current = Language.English;
        public static Language Current => _current;

        public static void Set(Language lang)
        {
            lock (_lock)
            {
                _current = lang;
                Load(lang);
            }
        }

        private static void Load(Language lang)
        {
            _strings.Clear();

            string folder = Path.Combine(AppContext.BaseDirectory, "Locales");
            string fileName = lang == Language.German ? "de.json" : "en.json";
            string path = Path.Combine(folder, fileName);

            if (!File.Exists(path))
            {
                // fallback to english
                path = Path.Combine(folder, "en.json");
                if (!File.Exists(path))
                    return;
            }

            try
            {
                string json = File.ReadAllText(path);
                var doc = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                          ?? new Dictionary<string, string>();
                foreach (var kv in doc)
                    _strings[kv.Key] = kv.Value;
            }
            catch
            {
                // ignore - keep empty dictionary
            }
        }

        public static string T(string key)
        {
            if (_strings.TryGetValue(key, out var value))
                return value;
            return $"[{key}]";
        }

        public static string TF(string key, params object[] args)
        {
            var template = T(key);
            try
            {
                return string.Format(template, args);
            }
            catch
            {
                return template;
            }
        }
    }
}