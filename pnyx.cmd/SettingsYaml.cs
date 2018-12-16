using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using pnyx.net.errors;
using pnyx.net.fluent;
using pnyx.net.util;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

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
            IDeserializer deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();

            Settings result = deserializer.Deserialize<Settings>(source);
            result.defaultNewline = validateNewline(result.defaultNewline);
            
            if (result.bufferLines <= 0)
                throw new InvalidArgumentException("BufferLines must be a positive values '{0}'", result.bufferLines);

            return result;
        }

        private readonly Regex NEWLINE_EXPRESSION = new Regex("^([\n])|([\n\r])+$");        
        private String validateNewline(String newline)
        {
            if (String.IsNullOrEmpty(newline))
                throw new InvalidArgumentException("Default newline setting must have a value");
            
            if (NEWLINE_EXPRESSION.IsMatch(newline))
                return newline;

            // Converts 'named' strings into newlines
            IDictionary<String,NewLineEnum> named = EnumUtil.toDictionary<NewLineEnum>();
            if (named.ContainsKey(newline))
            {
                NewLineEnum newlineType = named[newline];
                if (newlineType != NewLineEnum.None)
                    return StreamInformation.newlineString(newlineType);
            }
            
            throw new InvalidArgumentException("Unrecognized newline '{0}'", newline);
        }
    }
}