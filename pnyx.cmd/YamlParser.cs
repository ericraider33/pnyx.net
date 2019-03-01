using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using pnyx.net.errors;
using pnyx.net.fluent;
using pnyx.net.util;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace pnyx.cmd
{
    // Supports 1.1 of the YAML specification.
    // https://yaml-multiline.info/
    // https://github.com/aaubry/YamlDotNet            
    // https://www.nuget.org/packages/YamlDotNet/     
    public class YamlParser
    {
        private readonly MethodInfo[] methods;

        public YamlParser()
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
            p.setSettings(stdIoDefault: true);              // forces STD-IN/OUT as defaults                         
            
            if (document.RootNode.NodeType != YamlNodeType.Sequence)
                throw new InvalidArgumentException("Expected a YAML sequence as the document root, but found: {0}", document.RootNode.NodeType.ToString());                
                
            YamlSequenceNode topLevel = (YamlSequenceNode)document.RootNode;
            parseBlock(p, topLevel);
            
            return p;
        }

        public void parseBlock(Pnyx p, YamlSequenceNode block)
        {
            foreach (YamlNode node in block)
            {
                if (node.NodeType != YamlNodeType.Mapping)
                    throw new InvalidArgumentException("Expected YAML mapping for command/value pair, but found: {0}", node.NodeType.ToString());

                YamlMappingNode commandNode = (YamlMappingNode) node;
                if (commandNode.Children.Count != 1)
                    throw new InvalidArgumentException("Expected YAML mapping with 1 command/value pair, but found: {0}", commandNode.Children.Count);

                KeyValuePair<YamlNode,YamlNode> commandPair = commandNode.Children.First();
                if (commandPair.Key.NodeType != YamlNodeType.Scalar)
                    throw new InvalidArgumentException("Expected YAML scalar for commandName, but found: {0}", commandPair.Key.NodeType.ToString());

                YamlScalarNode commandName = (YamlScalarNode) commandPair.Key;
                switch (commandPair.Value.NodeType)
                {
                    case YamlNodeType.Scalar: parseScalarNode(p, commandName, (YamlScalarNode)commandPair.Value); break;
                    case YamlNodeType.Sequence: parseSequenceNode(p, commandName, (YamlSequenceNode)commandPair.Value); break;
                    case YamlNodeType.Mapping: parseMappingNode(p, commandName, (YamlMappingNode)commandPair.Value); break;
                    default: throw new InvalidArgumentException("YAML node isn't currently supported '{0}' for command values", commandPair.Value.NodeType.ToString());                
                }
            }
        }

        protected void parseScalarNode(Pnyx p, YamlScalarNode name, YamlScalarNode value)
        {
            List<YamlScalarNode> parameterNodes = new List<YamlScalarNode>();
            if (value.Value != "")
                parameterNodes.Add(value);

            executeMethod(p, name.Value, parameterNodes);
        }
        
        protected void parseSequenceNode(Pnyx p, YamlScalarNode name, YamlSequenceNode values)
        {
            List<YamlScalarNode> parameterNodes = convertSequenceToList(values);            
            executeMethod(p, name.Value, parameterNodes);
        }

        protected void executeMethod(Pnyx p, String methodName, List<YamlScalarNode> parameterNodes)
        {
            List<MethodInfo> methodMatches = methods.Where(m => m.Name == methodName).ToList();            
            MethodInfo method = methodMatches.FirstOrDefault(m => m.GetParameters().Length == parameterNodes.Count);
            if (method == null)
                method = methodMatches.OrderByDescending(m => m.GetParameters().Length).FirstOrDefault();           // finds longest number of paramets

            if (method == null)
                throw new InvalidArgumentException("Pnyx method can not be found: {0}", methodName);

            ParameterInfo[] methodParameters = method.GetParameters();
            ParameterInfo multiParameter = findParameterArray(methodParameters);                        
            if (parameterNodes.Count > methodParameters.Length && multiParameter == null)
                throw new InvalidArgumentException("Too many parameters {0} specified for Pnyx method '{1}', which only has {2} parameters", parameterNodes.Count, methodName, methodParameters.Length);

            // Checks for minimum size
            int requiredParameters = methodParameters.Count(pi => !pi.HasDefaultValue);
            if (parameterNodes.Count < requiredParameters + (multiParameter != null ? -1 : 0))
                throw new InvalidArgumentException("Too few parameters {0} specified for Pnyx method '{1}', which only has {2} required parameters", parameterNodes.Count, methodName, requiredParameters);
            
            // Builds parameter list with defaults
            Object[] parameters = new Object[methodParameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo current = methodParameters[i];

                if (current == multiParameter)
                    parameters[i] = processMultiParameters(multiParameter, i == 0 ? parameterNodes : parameterNodes.Skip(i));
                else if (i < parameterNodes.Count)
                    parameters[i] = processScalarParameter(current, parameterNodes[i]);
                else
                    parameters[i] = current.DefaultValue;
            }
            
            try
            {
                // Runs method
                method.Invoke(p, parameters);
            }
            catch (TargetInvocationException err)
            {
                throw err.InnerException;
            }
        }

        protected void parseMappingNode(Pnyx p, YamlScalarNode name, YamlMappingNode values)
        {
            String methodName = name.Value;
            
            // Converts to dictionary
            Dictionary<String, YamlNode> parameterNodes = new Dictionary<String, YamlNode>();            
            foreach (var pairs in values.Children)
            {
                String parameterName = ((YamlScalarNode) pairs.Key).Value;
                if (parameterNodes.ContainsKey(parameterName))
                    throw new InvalidArgumentException("Parameters can only have 1 value: {0}", parameterName);
                
                parameterNodes.Add(parameterName, pairs.Value);
            }
                                     
            // Finds matching method 
            List<MethodInfo> methodMatches = methods.Where(m => m.Name == methodName).ToList();            
            MethodInfo method = methodMatches.FirstOrDefault(m => m.GetParameters().Length == parameterNodes.Count);
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
                if (!parameterNodes.ContainsKey(pi.Name) && !pi.HasDefaultValue)
                    throw new InvalidArgumentException("Pnyx method '{0}' is missing required parameter '{1}'", methodName, pi.Name);

                if (parameterNodes.ContainsKey(pi.Name))
                {
                    YamlNode valueNode = parameterNodes[pi.Name];
                    switch (valueNode.NodeType)
                    {
                        case YamlNodeType.Scalar: 
                            parameters[i] = processScalarParameter(pi, (YamlScalarNode)valueNode); 
                            break;

                        case YamlNodeType.Sequence:
                        {
                            YamlSequenceNode sequenceNode = (YamlSequenceNode)valueNode;
                            if (pi.ParameterType.IsArray)
                            {
                                parameters[i] = processArray(pi, sequenceNode);
                            }
                            else if (pi.ParameterType != typeof(Action<Pnyx>))
                            {
                                throw new InvalidArgumentException("Parameter '{0}' does not support block / dictionary", pi.Name);
                            }
                            else
                            {                                
                                // Builds action for populating sub-pnyx yaml block
                                BlockYaml block = new BlockYaml(this, sequenceNode);
                                Action<Pnyx> action = block.action;
                                parameters[i] = action;
                            }
                            break;
                        }
                            
                        default: 
                            throw new InvalidArgumentException("YAML node isn't currently supported: {0}", valueNode.NodeType.ToString());                
                    }
                    
                    parameterNodes.Remove(pi.Name);
                }
                else
                    parameters[i] = pi.DefaultValue;
            }

            if (parameterNodes.Count > 0)
            {
                String unknownParameters = String.Join(",", parameterNodes.Keys);
                String availableParameters = String.Join(",", methodParameters.Select(pi => pi.Name));
                throw new InvalidArgumentException("Unknown named parameters '{0}' for Pnyx method '{1}', which has parameters '{2}'", unknownParameters, methodName, availableParameters);                        
            }
                        
            try
            {
                // Runs named parameters
                method.Invoke(p, parameters);
            }
            catch (TargetInvocationException err)
            {
                throw err.InnerException;
            }
        }

        private Object processScalarParameter(ParameterInfo parameterInfo, YamlScalarNode scalarNode)
        {
            String scalarValue = scalarNode.Value;
            switch (parameterInfo.ParameterType.Name)
            {
                case "Int32": return Int32.Parse(scalarValue);
                case "Boolean": return TextUtil.parseBool(scalarValue);
                case "String": return scalarValue;
                case "Encoding": return EncodingTypeConverter.parseText(scalarValue);
                default:
                    throw new InvalidArgumentException("Type conversion hasn't been built yet for: {0}", parameterInfo.ParameterType.FullName);            
            }
        }

        private ParameterInfo findParameterArray(ParameterInfo[] methodParameters)
        {
            if (methodParameters.Length == 0)
                return null;

            ParameterInfo last = methodParameters[methodParameters.Length - 1];
            if (Attribute.IsDefined(last, typeof(ParamArrayAttribute)))
                return last;

            return null;
        }

        private Object processMultiParameters(ParameterInfo multiParameter, IEnumerable<YamlScalarNode> values)
        {                
            switch (multiParameter.ParameterType.Name)
            {
                case "Int32[]": return values.Select(ysn => Int32.Parse(ysn.Value)).ToArray();
                case "Boolean[]": return values.Select(ysn => TextUtil.parseBool(ysn.Value)).ToArray();
                case "String[]": return values.Select(ysn => ysn.Value).ToArray();
                case "Object[]":
                {
                    List<Object> result = new List<Object>();
                    foreach (YamlScalarNode scalarNode in values)
                    {
                        if (scalarNode.Style == ScalarStyle.DoubleQuoted || scalarNode.Style == ScalarStyle.SingleQuoted)
                            result.Add(scalarNode.Value);
                        else
                            result.Add(convertToObject(scalarNode.Value));
                    }

                    return result.ToArray();
                }
                default:
                    throw new InvalidArgumentException("No multi-param conversion exists for: {0}", multiParameter.ParameterType.FullName);            
            }
        }

        private Object convertToObject(String value)
        {
            if (value.Length == 0)
                return value;

            if (TextUtil.isDecimal(value))
            {
                if (TextUtil.isInteger(value))
                    return Int32.Parse(value);
                return Double.Parse(value);
            }

            bool? boolValue = TextUtil.parseBoolNullable(value);
            if (boolValue.HasValue)
                return boolValue.Value;

            return value;
        }
        
        private Object processArray(ParameterInfo arrayParam, YamlSequenceNode sequenceNode)
        {
            List<YamlScalarNode> parameterList = convertSequenceToList(sequenceNode);
            return processMultiParameters(arrayParam, parameterList);
        }

        private List<YamlScalarNode> convertSequenceToList(YamlSequenceNode sequenceNode)
        {
            List<YamlScalarNode> parameterList = new List<YamlScalarNode>();
            foreach (YamlNode node in sequenceNode)
            {
                if (node.NodeType != YamlNodeType.Scalar)
                    throw new InvalidArgumentException("YAML node isn't currently supported: {0}", node.NodeType.ToString());

                parameterList.Add((YamlScalarNode) node);
            }

            return parameterList;
        }        
    }
}