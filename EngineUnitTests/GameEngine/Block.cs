using NUnit.Framework;
using TDDBlocks.GameEngine;
using System;

namespace TDDBlocks.UnitTests.GameEngine
{
    [TestFixture]
    public class CreateAGreenBlock
    {
        [Test]
        public void Test1()
        {
            var testBlock = new Block(Block.BlockTypeEnum.Green);

            Assert.AreEqual(testBlock.Type, Block.BlockTypeEnum.Green);
        }
    }


    [TestFixture]
    public class CheckTheBlockStatusIsStopped
    {
        [Test]
        public void Test2()
        {
            var testBlock = new Block(Block.BlockTypeEnum.Green);

            //Change the block status to stopped
            testBlock.Status = Block.BlockStatusEnum.Stopped;

            Assert.AreEqual(testBlock.Status, Block.BlockStatusEnum.Stopped);
        }
    }

    [TestFixture]
    public class BlockThatIsntAPowerBlock
    {
        [Test]
        public void Test3()
        {
            var testBlock = new Block(0);

            Assert.AreNotEqual(testBlock.Type, Block.BlockTypeEnum.DelayedExplosion);
            Assert.AreNotEqual(testBlock.Type, Block.BlockTypeEnum.BigExplosion);
            Assert.AreNotEqual(testBlock.Type, Block.BlockTypeEnum.InIce);
            Assert.AreNotEqual(testBlock.Type, Block.BlockTypeEnum.MorphingBlock);
            Assert.AreNotEqual(testBlock.Type, Block.BlockTypeEnum.Rock);

        }
    }

    [TestFixture]
    public class BlockThatIsntAStandardBlock
    {
        [Test]
        public void Test4()
        {
            var testBlock = new Block(100);

            Assert.AreNotEqual(testBlock.Type, Block.BlockTypeEnum.Blue);
            Assert.AreNotEqual(testBlock.Type, Block.BlockTypeEnum.Green);
            Assert.AreNotEqual(testBlock.Type, Block.BlockTypeEnum.Orange);
            Assert.AreNotEqual(testBlock.Type, Block.BlockTypeEnum.Purple);
            Assert.AreNotEqual(testBlock.Type, Block.BlockTypeEnum.Red);

        }
    }

    [TestFixture]
    public class TwentyPercentChanceOfPowerBlock
    {
        [Test]
        public void Test5()
        {

            for (int i = 0; i < 50; i++)
            {

                var testBlock = new Block(20);

                Console.WriteLine(testBlock.Type.ToString());

            }

        }
    }

}