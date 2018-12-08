using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using pnyx.net.errors;
using pnyx.net.fluent;
using YamlDotNet.RepresentationModel;

namespace pnyx.cmd
{
    // Supports 1.1 of the YAML specification.
    public class PnyxYaml
    {
        private MethodInfo[] methods;

        public PnyxYaml()
        {
            methods = typeof(Pnyx).GetMethods(BindingFlags.Instance | BindingFlags.Public);            
        }            
        
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
                switch (pairs.Value.NodeType)
                {
                    case YamlNodeType.Scalar: parseScalarNode(p, (YamlScalarNode)pairs.Key, (YamlScalarNode)pairs.Value); break;
                    case YamlNodeType.Sequence: parseSequenceNode(p, (YamlScalarNode)pairs.Key, (YamlSequenceNode)pairs.Value); break;
                    case YamlNodeType.Mapping: parseMappingNode(p, (YamlScalarNode)pairs.Key, (YamlMappingNode)pairs.Value); break;
                    default: throw new InvalidArgumentException("YAML node isn't currently support: {0}", pairs.Value.NodeType.ToString());                
                }
            }
            
            return p;
        }

        protected void parseScalarNode(Pnyx p, YamlScalarNode name, YamlScalarNode value)
        {
            List<Object> parameterList = new List<Object>();
            if (value.Value != "")
                parameterList.Add(value.Value);

            executeMethod(p, name.Value, parameterList);
        }
        
        protected void parseSequenceNode(Pnyx p, YamlScalarNode name, YamlSequenceNode values)
        {
            List<Object> parameterList = new List<Object>();
            foreach (YamlNode node in values)
            {
                if (node.NodeType != YamlNodeType.Scalar)
                    throw new InvalidArgumentException("YAML node isn't currently support: {0}", node.NodeType.ToString());
                
                parameterList.Add(((YamlScalarNode)node).Value);
            }

            executeMethod(p, name.Value, parameterList);
        }

        protected void executeMethod(Pnyx p, String methodName, List<Object> parameterList)
        {
            object[] parameters = parameterList.ToArray();
            
            List<MethodInfo> methodMatches = methods.Where(m => m.Name == methodName).ToList();
            
            MethodInfo method = methodMatches.FirstOrDefault(m => m.GetParameters().Length == parameters.Length);
            if (method != null)
            {
                method.Invoke(p, parameters);
                return;
            }

            method = methodMatches.OrderByDescending(m => m.GetParameters().Length).FirstOrDefault();
            if (method == null)
                throw new InvalidArgumentException("Pnyx method can not be found: {0}", methodName);

            ParameterInfo[] methodParameters = method.GetParameters();
            if (parameterList.Count > methodParameters.Length)
                throw new InvalidArgumentException("Too many parameters {0} specified for Pnyx method '{1}', which only has {2} parameters", parameterList.Count, methodName, methodParameters.Length);

            // Checks for minimum size
            int requiredParameters = methodParameters.Count(pi => !pi.HasDefaultValue);
            if (parameterList.Count < requiredParameters)
                throw new InvalidArgumentException("Too few parameters {0} specified for Pnyx method '{1}', which only has {2} required parameters", parameterList.Count, methodName, requiredParameters);
            
            // Builds parameter list with defaults
            parameters = new object[methodParameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                if (i < parameterList.Count)
                    parameters[i] = parameterList[i];
                else 
                    parameters[i] = methodParameters[i].DefaultValue;
            }
            
            // Runs with default values
            method.Invoke(p, parameters);
        }

        protected void parseMappingNode(Pnyx p, YamlScalarNode name, YamlMappingNode values)
        {
            Dictionary<String, Object> parameters = new Dictionary<string, object>();            
            foreach (var pairs in values.Children)
            {
                String parameterName = ((YamlScalarNode) pairs.Key).Value;
                if (parameters.ContainsKey(parameterName))
                    throw new InvalidArgumentException("Parameters can only have 1 value: {0}", parameterName);
                
                switch (pairs.Value.NodeType)
                {
                    case YamlNodeType.Scalar:
                        parameters.Add(parameterName, ((YamlScalarNode)pairs.Value).Value);
                        break;
                    
                    default: 
                        throw new InvalidArgumentException("YAML node isn't currently support: {0}", pairs.Value.NodeType.ToString());                
                }
            }
            
            executeMethod(p, name.Value, parameters);            
        }

        protected void executeMethod(Pnyx p, String methodName, Dictionary<String, Object> parameterDictionary)
        {
            List<MethodInfo> methodMatches = methods.Where(m => m.Name == methodName).ToList();
            
            MethodInfo method = methodMatches.FirstOrDefault(m => m.GetParameters().Length == parameterDictionary.Count);
            if (method == null)
            {
                method = methodMatches.OrderByDescending(m => m.GetParameters().Length).FirstOrDefault();
                if (method == null)
                    throw new InvalidArgumentException("Pnyx method can not be found: {0}", methodName);
            }

            // Builds parameter list with defaults
            ParameterInfo[] methodParameters = method.GetParameters();            
            Object[] parameters = new Object[methodParameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo pi = methodParameters[i];
                if (!parameterDictionary.ContainsKey(pi.Name) && !pi.HasDefaultValue)
                    throw new InvalidArgumentException("Pnyx method '{0}' is missing required parameter '{1}'", methodName, pi.Name);

                if (parameterDictionary.ContainsKey(pi.Name))
                {
                    parameters[i] = parameterDictionary[pi.Name];
                    parameterDictionary.Remove(pi.Name);
                }
                else
                    parameters[i] = methodParameters[i].DefaultValue;
            }

            if (parameterDictionary.Count > 0)
            {
                String unknownParameters = String.Join(",", parameterDictionary.Keys);
                String availableParameters = String.Join(",", methodParameters.Select(pi => pi.Name));
                throw new InvalidArgumentException("Unknown named parameters '{0}' for Pnyx method '{1}', which has parameters '{2}'", unknownParameters, methodName, availableParameters);                        
            }
            
            // Runs named parameters
            method.Invoke(p, parameters);            
        }
    }
}