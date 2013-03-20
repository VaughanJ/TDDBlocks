using NUnit.Framework;
using TDDBlocks.GameEngine;

namespace TDDBlocks.UnitTests.GameEngine
{
    [TestFixture]
    public class BlockPairTested
    {
        private bool _bothBlocksStoppedTest;

        public void BothBlocksStopped(object sender, BothBlocksStoppedHandlerArgs args)
        {
            _bothBlocksStoppedTest = true;
        }

        [Test]
        public void BothBlocksInPairStopped()
        {
            var currentBlockPair = new BlockPair(Block.BlockTypeEnum.Green, Block.BlockTypeEnum.Green);

            currentBlockPair.BothBlocksStopped += BothBlocksStopped;

            currentBlockPair.Block1.Status = Block.BlockStatusEnum.Stopped;
            currentBlockPair.Block2.Status = Block.BlockStatusEnum.Stopped;

            Assert.AreEqual(_bothBlocksStoppedTest, true);
        }

        [Test]
        public void CreateAPairOfGreenBlocks()
        {
            var currentBlockPair = new BlockPair(Block.BlockTypeEnum.Green, Block.BlockTypeEnum.Green);

            Assert.AreEqual(currentBlockPair.Block1.Type, Block.BlockTypeEnum.Green);
            Assert.AreEqual(currentBlockPair.Block2.Type, Block.BlockTypeEnum.Green);
        }

        [Test]
        public void CreateAPairOfGreenBlocksDefaultNoRotation()
        {
            var currentBlockPair = new BlockPair(Block.BlockTypeEnum.Green, Block.BlockTypeEnum.Green);

            Assert.AreEqual(currentBlockPair.RotationDirection, BlockPair.RotationEnum.None);
        }
    }
}