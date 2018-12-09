using pnyx.net.fluent;
using YamlDotNet.RepresentationModel;

namespace pnyx.cmd
{
    public class BlockYaml
    {
        private PnyxYaml pnyxYaml;
        private YamlSequenceNode block;

        public BlockYaml(PnyxYaml pnyxYaml, YamlSequenceNode block)
        {
            this.pnyxYaml = pnyxYaml;
            this.block = block;
        }

        public void action(Pnyx pnyx)
        {
            pnyxYaml.parseBlock(pnyx, block);
        }
    }
}