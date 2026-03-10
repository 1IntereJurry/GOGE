using System.Text.Json;
using System.Reflection;

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

            string? json = null;

            if (File.Exists(path))
            {
                try { json = File.ReadAllText(path); }
                catch { json = null; }
            }

            if (json == null)
            {
                // try embedded resource (useful for single-file publish or installer packaging)
                try
                {
                    var asm = Assembly.GetExecutingAssembly();
                    var resourceSuffix = $"locales.{fileName}".ToLowerInvariant();
                    var res = asm.GetManifestResourceNames()
                                 .FirstOrDefault(n => n.ToLowerInvariant().EndsWith(resourceSuffix));
                    if (res != null)
                    {
                        using var stream = asm.GetManifestResourceStream(res);
                        if (stream != null)
                        {
                            using var reader = new StreamReader(stream);
                            json = reader.ReadToEnd();
                        }
                    }
                }
                catch
                {
                    json = null;
                }
            }

            if (string.IsNullOrEmpty(json))
                return;

            try
            {
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