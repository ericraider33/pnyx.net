using pnyx.net.fluent;
using YamlDotNet.RepresentationModel;

namespace pnyx.cmd
{
    public class BlockYaml
    {
        private YamlParser yamlParser;
        private YamlSequenceNode block;

        public BlockYaml(YamlParser yamlParser, YamlSequenceNode block)
        {
            this.yamlParser = yamlParser;
            this.block = block;
        }

        public void action(Pnyx pnyx)
        {
            yamlParser.parseBlock(pnyx, block);
        }
    }
}