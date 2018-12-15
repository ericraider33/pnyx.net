using System;
using System.IO;
using pnyx.net.fluent;
using YamlDotNet.RepresentationModel;

namespace pnyx.cmd
{
    public class SettingsYaml
    {
        public const String SETTINGS_FILE_NAME = "pnyx_settings.yaml";
        
        public static bool parseSetting(String path = null)
        {
            if (path == null)
            {
                String directory = AppContext.BaseDirectory;
                path = Path.Combine(directory, SETTINGS_FILE_NAME);
            }

            if (!File.Exists(path))
                return false;

            try
            {
                using (StreamReader reader = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
                {
                    SettingsYaml sy = new SettingsYaml();
                    Settings settings = sy.parseReader(reader);
                    SettingsHome.settingsFactory = new SettingsHome(settings);
                }

                return true;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error: Exception while reading '{0}' settings file: {1}", path, e.Message);
                Console.Error.WriteLine(e.StackTrace);
                return false;
            }
        }
        
        public Settings parseReader(TextReader source)
        {
            YamlStream yaml = new YamlStream();
            yaml.Load(source);
            throw new NotImplementedException();            
        }
    }
}