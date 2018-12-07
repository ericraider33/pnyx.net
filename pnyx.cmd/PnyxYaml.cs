using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using pnyx.net.errors;
using pnyx.net.fluent;
using YamlDotNet.RepresentationModel;

namespace pnyx.cmd
{
    // Supports 1.1 of the YAML specification.
    public class PnyxYaml
    {
        public List<Pnyx> parseYaml(TextReader source)
        {
            YamlStream yaml = new YamlStream();
            yaml.Load(source);
            
            List<Pnyx> result = new List<Pnyx>();
            foreach (YamlDocument document in yaml.Documents)
            {
                Pnyx pnyx = parseDocument(document);
                result.Add(pnyx);
            }

            return result;
        }

        protected Pnyx parseDocument(YamlDocument document)
        {
            Pnyx p = new Pnyx();
            
            YamlMappingNode topLevel = (YamlMappingNode)document.RootNode;
            foreach (var pairs in topLevel.Children)
            {                
                if (pairs.Value.NodeType == YamlNodeType.Scalar)
                    parseScalarNode(p, (YamlScalarNode)pairs.Key, (YamlScalarNode)pairs.Value);
                else
                    throw new InvalidArgumentException("YAML node isn't currently support: {0}", pairs.Value.NodeType.ToString());                
            }
            
            return p;
        }

        protected void parseScalarNode(Pnyx p, YamlScalarNode name, YamlScalarNode value)
        {
            List<Object> parameterList = new List<Object>();
            if (value.Value != "")
                parameterList.Add(value.Value);

            object[] parameters = parameterList.ToArray(); 
            MethodInfo method = p.GetType().GetMethod(name.Value, BindingFlags.Instance | BindingFlags.Public);
            method.Invoke(p, parameters);            
        }
                
    }
}