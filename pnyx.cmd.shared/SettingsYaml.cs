using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using pnyx.net.errors;
using pnyx.net.fluent;
using pnyx.net.util;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Settings = pnyx.net.fluent.Settings;

namespace pnyx.cmd.shared
{
    public class SettingsYaml
    {
        public const String SETTINGS_FILE_NAME = "pnyx_settings.yaml";
        public const String LIB_DIRECTORY = "lib";
        
        public static bool parseSetting(String path = null, bool verboseSettings = false)
        {
            if (path == null)
                path = findSettings(verboseSettings);

            if (!File.Exists(path))
            {
                if (verboseSettings)
                    Console.WriteLine("- Could not find file: {0}", path);    
                
                return false;
            }

            if (verboseSettings)
                Console.WriteLine("- Found settings file: {0}", path);    

            try
            {
                using (StreamReader reader = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
                {
                    SettingsYaml sy = new SettingsYaml();
                    Settings settings = sy.deserializeSettings(reader);

                    if (settings == null)
                    {
                        if (verboseSettings)
                            Console.WriteLine("- Settings file '{0}' contained no settings and was ignored", path);
                        
                        return false;
                    }
                    
                    SettingsHome.settingsFactory = new SettingsHome(settings);
                }

                if (verboseSettings)
                    Console.WriteLine("- Finished reading settings from: {0}", path);
                
                return true;
            }            
            catch (Exception e)
            {
                if (e.InnerException != null)
                    e = e.InnerException;
                
                Console.Error.WriteLine("Error: Exception while reading '{0}' settings file: {1}", path, e.Message);
                Console.Error.WriteLine(e.StackTrace);
                return false;
            }
        }

        private static String findSettings(bool verboseSettings)
        {
            if (verboseSettings)
                Console.WriteLine("- Starting search for '{0}' file", SETTINGS_FILE_NAME);    
        
            String directory = AppContext.BaseDirectory;
            if (verboseSettings)
                Console.WriteLine("- Directory of application: {0}", directory);    
                        
            String path = Path.Combine(directory, SETTINGS_FILE_NAME);
            if (File.Exists(path))
                return path;
            
            DirectoryInfo di = new DirectoryInfo(directory);
            if (TextUtil.isEqualsIgnoreCase(di.Name, LIB_DIRECTORY))
            {
                di = di.Parent;
                path = Path.Combine(di.FullName, SETTINGS_FILE_NAME);
                
                if (verboseSettings)
                    Console.WriteLine("- Checking parent directory: {0}", di.FullName);                 
            }

            return path;
        }
        
        // NOTE: If source file is completely commented out, then result is NULL
        public Settings deserializeSettings(TextReader source)
        {
            IDeserializer deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeConverter(new EncodingTypeConverter())
                .Build();

            Settings result = deserializer.Deserialize<Settings>(source);
            if (result == null)
                return null;
            
            result.defaultNewline = validateNewline(result.defaultNewline);
            
            if (result.bufferLines <= 0)
                throw new InvalidArgumentException("BufferLines must be a positive values '{0}'", result.bufferLines);

            return result;
        }

        public void serializeSettings(TextWriter destination, Settings settings)
        {
            ISerializer serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeConverter(new EncodingTypeConverter())
                .Build();

            serializer.Serialize(destination, settings);                       
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