using System.Collections.Generic;
using Version1.Nirvana;

namespace Version1.Data
{
    public sealed class ChromosomeBlock
    {
        public readonly Chromosome Chromosome;
        public readonly List<Block> CommonBlocks;
        public readonly List<Block> RareBlocks;

        public ChromosomeBlock(Chromosome chromosome, List<Block> commonBlocks, List<Block> rareBlocks)
        {
            Chromosome   = chromosome;
            CommonBlocks = commonBlocks;
            RareBlocks   = rareBlocks;
        }

        public void Write(ExtendedBinaryWriter writer)
        {
            

            writer.WriteOpt(RareBlocks.Count);
            foreach (Block block in RareBlocks) block.Write(writer);
        }
    }
}