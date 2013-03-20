using NUnit.Framework;
using TDDBlocks.GameEngine;

namespace TDDBlocks.UnitTests.GameEngine
{
    [TestFixture]
    public class Test1
    {
        [Test]
        public void GetArenaSize()
        {
            var testArena = new Arena();

            Assert.AreEqual(testArena.Width, 8);
            Assert.AreEqual(testArena.Height, 13);
        }
    }

    [TestFixture]
    public class Test2
    {
        [Test]
        public void ArenaBlockStartPositions()
        {
            var testArena = new Arena();

            Assert.AreEqual(testArena.Block1StartPosition.X, 4);
            Assert.AreEqual(testArena.Block1StartPosition.Y, 14);
            Assert.AreEqual(testArena.Block2StartPosition.X, 5);
            Assert.AreEqual(testArena.Block2StartPosition.Y, 14);
        }
    }

    [TestFixture]
    public class Test3
    {
        private Arena _arena;
        private Block _block1;
        private Block _block2;
        private Block _block3;
        private Block _block4;

        [TestFixtureSetUp]
        public void Init()
        {
            _arena = new Arena();
            _block1 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 4, ArenaYPosition = 1};
            _arena.AddBlock(_block1, Block.BlockStatusEnum.Stopped);

            _block2 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 4, ArenaYPosition = 2};
            _arena.AddBlock(_block2, Block.BlockStatusEnum.Stopped);

            _block3 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 4, ArenaYPosition = 4};
            _arena.AddBlock(_block3, Block.BlockStatusEnum.Stopped);

            _block4 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 5, ArenaYPosition = 2};
            _arena.AddBlock(_block4, Block.BlockStatusEnum.Detatched);
        }


        [Test]
        public void CanBlockDropIntoSpaceBelow()
        {
            Assert.AreEqual(_arena.CanBlockDrop(_block1), false);
            Assert.AreEqual(_arena.CanBlockDrop(_block2), false);
            Assert.AreEqual(_arena.CanBlockDrop(_block3), true);
        }
    }

    [TestFixture]
    public class Test4
    {
        private Arena _arena;
        private Block _block1;
        private Block _block2;
        private Block _block3;
        private Block _block4;

        [TestFixtureSetUp]
        public void Init()
        {
            _arena = new Arena();
            _block1 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 4, ArenaYPosition = 1};
            _arena.AddBlock(_block1, Block.BlockStatusEnum.Stopped);

            _block2 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 4, ArenaYPosition = 2};
            _arena.AddBlock(_block2, Block.BlockStatusEnum.Stopped);

            _block3 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 4, ArenaYPosition = 4};
            _arena.AddBlock(_block3, Block.BlockStatusEnum.Stopped);

            _block4 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 5, ArenaYPosition = 2};
            _arena.AddBlock(_block4, Block.BlockStatusEnum.Detatched);
        }


        [Test]
        public void AutoUpdateSuspendedBlockToDroping()
        {
            _arena.MarkSuspendedBlocksAsDropping();

            Assert.AreEqual(_block1.Status == Block.BlockStatusEnum.Dropping, false);
            Assert.AreEqual(_block3.Status == Block.BlockStatusEnum.Dropping, true);
            Assert.AreEqual(_block4.Status == Block.BlockStatusEnum.Dropping, true);
        }
    }

    [TestFixture]
    public class Test5
    {
        private Arena _arena;
        private Block _block1;
        private Block _block2;
        private Block _block3;
        private Block _block4;

        private static bool _gameOver;

        [TestFixtureSetUp]
        public void Init()
        {
            _arena = new Arena();
            _block1 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 4, ArenaYPosition = 1};
            _arena.AddBlock(_block1, Block.BlockStatusEnum.Stopped);

            _block2 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 4, ArenaYPosition = 2};
            _arena.AddBlock(_block2, Block.BlockStatusEnum.Stopped);

            _block3 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 4, ArenaYPosition = 4};
            _arena.AddBlock(_block3, Block.BlockStatusEnum.Stopped);

            _block4 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 5, ArenaYPosition = 14};
            _block4.StatusChange += StatusChanged;
            _arena.AddBlock(_block4, Block.BlockStatusEnum.Stopped);
        }

        public void StatusChanged(object sender, StatusChangedHandlerArgs args)
        {
            if ((args.NewStatus == Block.BlockStatusEnum.Stopped) && (args.Y == 14))
            {
                _gameOver = true;
            }
        }

        [Test]
        public void GameOverWhenBlockOutOfArena()
        {
            Assert.AreEqual(_gameOver, true);
        }
    }

    [TestFixture]
    public class Test6
    {
        private Arena _arena;
        private Block _block1;
        private Block _block2;
        private Block _block3;
        private Block _block4;

        [TestFixtureSetUp]
        public void Init()
        {
            _arena = new Arena();
            _block1 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 4, ArenaYPosition = 1};
            _arena.AddBlock(_block1, Block.BlockStatusEnum.Stopped);

            _block2 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 4, ArenaYPosition = 2};
            _arena.AddBlock(_block2, Block.BlockStatusEnum.Stopped);

            _block3 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 4, ArenaYPosition = 3};
            _arena.AddBlock(_block3, Block.BlockStatusEnum.Stopped);

            _block4 = new Block(Block.BlockTypeEnum.Red) {ArenaXPosition = 5, ArenaYPosition = 1};
            _arena.AddBlock(_block4, Block.BlockStatusEnum.Stopped);
        }


        [Test]
        public void MarkAllConnectedBlocksOfTheSameType()
        {
            short connections = 0;

            connections = _arena.ChangeStatusOfLinkedBlocks(_block3.Type, Block.BlockStatusEnum.MarkedForDeletion,
                                                            _block3, connections);

            Assert.AreEqual(_block1.Status == Block.BlockStatusEnum.MarkedForDeletion, true);
            Assert.AreEqual(_block2.Status == Block.BlockStatusEnum.MarkedForDeletion, true);
            Assert.AreEqual(_block3.Status == Block.BlockStatusEnum.MarkedForDeletion, true);
        }
    }

    [TestFixture]
    public class Test7
    {
        private Arena _arena;
        private Block _block1;
        private Block _block2;
        private Block _block3;
        private Block _block4;
        private Block _block5;

        [TestFixtureSetUp]
        public void Init()
        {
            _arena = new Arena();
            _block1 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 1, ArenaYPosition = 1};
            _arena.AddBlock(_block1, Block.BlockStatusEnum.Stopped);

            _block2 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 1, ArenaYPosition = 2};
            _arena.AddBlock(_block2, Block.BlockStatusEnum.Stopped);

            _block3 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 1, ArenaYPosition = 3};
            _arena.AddBlock(_block3, Block.BlockStatusEnum.Stopped);

            _block4 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 1, ArenaYPosition = 4};
            _arena.AddBlock(_block4, Block.BlockStatusEnum.Stopped);

            _block5 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 1, ArenaYPosition = 5};
            _arena.AddBlock(_block5, Block.BlockStatusEnum.Stopped);
        }

        [Test]
        public void MarkAllConnectedBlocksOfTheSameTypeLeftEdge()
        {
            short connections = 0;

            connections = _arena.ChangeStatusOfLinkedBlocks(_block3.Type, Block.BlockStatusEnum.MarkedForDeletion,
                                                            _block3, connections);

            Assert.AreEqual(_block1.Status == Block.BlockStatusEnum.MarkedForDeletion, true);
            Assert.AreEqual(_block2.Status == Block.BlockStatusEnum.MarkedForDeletion, true);
            Assert.AreEqual(_block3.Status == Block.BlockStatusEnum.MarkedForDeletion, true);
            Assert.AreEqual(_block4.Status == Block.BlockStatusEnum.MarkedForDeletion, true);
            Assert.AreEqual(_block5.Status == Block.BlockStatusEnum.MarkedForDeletion, true);
        }
    }

    [TestFixture]
    public class Test8
    {
        private Arena _arena;
        private Block _block1;
        private Block _block2;
        private Block _block3;
        private Block _block4;
        private Block _block5;

        [TestFixtureSetUp]
        public void Init()
        {
            _arena = new Arena();
            _block1 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 8, ArenaYPosition = 1};
            _arena.AddBlock(_block1, Block.BlockStatusEnum.Stopped);

            _block2 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 8, ArenaYPosition = 2};
            _arena.AddBlock(_block2, Block.BlockStatusEnum.Stopped);

            _block3 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 8, ArenaYPosition = 3};
            _arena.AddBlock(_block3, Block.BlockStatusEnum.Stopped);

            _block4 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 8, ArenaYPosition = 4};
            _arena.AddBlock(_block4, Block.BlockStatusEnum.Stopped);

            _block5 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 8, ArenaYPosition = 5};
            _arena.AddBlock(_block5, Block.BlockStatusEnum.Stopped);
        }


        [Test]
        public void MarkAllConnectedBlocksOfTheSameTypeRightEdge()
        {
            short connections = 0;

            connections = _arena.ChangeStatusOfLinkedBlocks(_block3.Type, Block.BlockStatusEnum.MarkedForDeletion,
                                                            _block3, connections);

            Assert.AreEqual(_block1.Status == Block.BlockStatusEnum.MarkedForDeletion, true);
            Assert.AreEqual(_block2.Status == Block.BlockStatusEnum.MarkedForDeletion, true);
            Assert.AreEqual(_block3.Status == Block.BlockStatusEnum.MarkedForDeletion, true);
            Assert.AreEqual(_block4.Status == Block.BlockStatusEnum.MarkedForDeletion, true);
            Assert.AreEqual(_block5.Status == Block.BlockStatusEnum.MarkedForDeletion, true);
        }
    }

    [TestFixture]
    public class Test9
    {
        private Arena _arena;
        private Block _block1;
        private Block _block2;
        private Block _block3;
        private Block _block4;
        private Block _block5;

        [TestFixtureSetUp]
        public void Init()
        {
            _arena = new Arena();
            _block1 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 2, ArenaYPosition = 1};
            _arena.AddBlock(_block1, Block.BlockStatusEnum.Stopped);

            _block2 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 3, ArenaYPosition = 1};
            _arena.AddBlock(_block2, Block.BlockStatusEnum.Stopped);

            _block3 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 4, ArenaYPosition = 1};
            _arena.AddBlock(_block3, Block.BlockStatusEnum.Stopped);

            _block4 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 5, ArenaYPosition = 1};
            _arena.AddBlock(_block4, Block.BlockStatusEnum.Stopped);

            _block5 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 6, ArenaYPosition = 1};
            _arena.AddBlock(_block5, Block.BlockStatusEnum.Stopped);
        }


        [Test]
        public void MarkAllConnectedBlocksOfTheSameTypeBottomEdge()
        {
            short connections = 0;

            connections = _arena.ChangeStatusOfLinkedBlocks(_block3.Type, Block.BlockStatusEnum.MarkedForDeletion,
                                                            _block3, connections);

            Assert.AreEqual(_block1.Status == Block.BlockStatusEnum.MarkedForDeletion, true);
            Assert.AreEqual(_block2.Status == Block.BlockStatusEnum.MarkedForDeletion, true);
            Assert.AreEqual(_block3.Status == Block.BlockStatusEnum.MarkedForDeletion, true);
            Assert.AreEqual(_block4.Status == Block.BlockStatusEnum.MarkedForDeletion, true);
            Assert.AreEqual(_block5.Status == Block.BlockStatusEnum.MarkedForDeletion, true);
        }
    }

    [TestFixture]
    public class Test10
    {
        private Arena _arena;
        private Block _block1;
        private Block _block2;
        private Block _block3;
        private Block _block4;
        private Block _block5;

        [TestFixtureSetUp]
        public void Init()
        {
            _arena = new Arena();
            _block1 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 2, ArenaYPosition = 1};
            _arena.AddBlock(_block1, Block.BlockStatusEnum.Stopped);

            _block2 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 3, ArenaYPosition = 1};
            _arena.AddBlock(_block2, Block.BlockStatusEnum.Stopped);

            _block3 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 4, ArenaYPosition = 1};
            _arena.AddBlock(_block3, Block.BlockStatusEnum.Stopped);

            _block4 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 3, ArenaYPosition = 2};
            _arena.AddBlock(_block4, Block.BlockStatusEnum.Stopped);

            _block5 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 3, ArenaYPosition = 3};
            _arena.AddBlock(_block5, Block.BlockStatusEnum.Stopped);
        }


        [Test]
        public void MarkAllConnectedBlocksOfTheSameTypeLinkedAsTShape()
        {
            short connections = 0;

            connections = _arena.ChangeStatusOfLinkedBlocks(_block3.Type, Block.BlockStatusEnum.MarkedForDeletion,
                                                            _block3, connections);

            Assert.AreEqual(_block1.Status == Block.BlockStatusEnum.MarkedForDeletion, true);
            Assert.AreEqual(_block2.Status == Block.BlockStatusEnum.MarkedForDeletion, true);
            Assert.AreEqual(_block3.Status == Block.BlockStatusEnum.MarkedForDeletion, true);
            Assert.AreEqual(_block4.Status == Block.BlockStatusEnum.MarkedForDeletion, true);
            Assert.AreEqual(_block5.Status == Block.BlockStatusEnum.MarkedForDeletion, true);
        }
    }

    [TestFixture]
    public class Test11
    {
        private Arena _arena;
        private Block _block1;
        private Block _block2;
        private Block _block3;
        private Block _block4;
        private Block _block5;

        [TestFixtureSetUp]
        public void Init()
        {
            _arena = new Arena();
            _block1 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 2, ArenaYPosition = 1};
            _arena.AddBlock(_block1, Block.BlockStatusEnum.Stopped);

            _block2 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 3, ArenaYPosition = 1};
            _arena.AddBlock(_block2, Block.BlockStatusEnum.Stopped);

            _block3 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 4, ArenaYPosition = 1};
            _arena.AddBlock(_block3, Block.BlockStatusEnum.Stopped);

            _block4 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 3, ArenaYPosition = 2};
            _arena.AddBlock(_block4, Block.BlockStatusEnum.Stopped);

            _block5 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 3, ArenaYPosition = 3};
            _arena.AddBlock(_block5, Block.BlockStatusEnum.Stopped);
        }


        [Test]
        public void MarkAllConnectedBlocksOfTheSameTypeAndCheckConnections()
        {
            short connections = 0;

            connections = _arena.ChangeStatusOfLinkedBlocks(_block3.Type, Block.BlockStatusEnum.MarkedForDeletion,
                                                            _block3, connections);

            Assert.AreEqual(connections == 5, true);
        }
    }

    [TestFixture]
    public class Test12
    {
        private Arena _arena;
        private Block _block1;
        private Block _block2;
        private Block _block3;
        private Block _block4;
        private Block _block5;

        [TestFixtureSetUp]
        public void Init()
        {
            _arena = new Arena();
            _block1 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 2, ArenaYPosition = 1};
            _arena.AddBlock(_block1, Block.BlockStatusEnum.Stopped);

            _block2 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 3, ArenaYPosition = 1};
            _arena.AddBlock(_block2, Block.BlockStatusEnum.Stopped);

            _block3 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 4, ArenaYPosition = 1};
            _arena.AddBlock(_block3, Block.BlockStatusEnum.Stopped);

            _block4 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 3, ArenaYPosition = 2};
            _arena.AddBlock(_block4, Block.BlockStatusEnum.Stopped);

            _block5 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 3, ArenaYPosition = 3};
            _arena.AddBlock(_block5, Block.BlockStatusEnum.Stopped);
        }


        [Test]
        public void MarkAllMarkedForExplosionConnectedBlocks()
        {
            short connections = 0;

            connections = _arena.ChangeStatusOfLinkedBlocks(_block3.Type, Block.BlockStatusEnum.Exploding, _block3,
                                                            connections);

            Assert.AreEqual(_block1.Status == Block.BlockStatusEnum.Exploding, true);
            Assert.AreEqual(_block2.Status == Block.BlockStatusEnum.Exploding, true);
            Assert.AreEqual(_block3.Status == Block.BlockStatusEnum.Exploding, true);
            Assert.AreEqual(_block4.Status == Block.BlockStatusEnum.Exploding, true);
            Assert.AreEqual(_block5.Status == Block.BlockStatusEnum.Exploding, true);
        }
    }

    [TestFixture]
    public class Test15
    {
        private Arena _arena;
        private Block _block1;
        private Block _block2;
        private Block _block3;
        private Block _block4;
        private Block _block5;
        private Block _block6;

        [TestFixtureSetUp]
        public void Init()
        {
            _arena = new Arena();
            _block1 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 2, ArenaYPosition = 1};
            _arena.AddBlock(_block1, Block.BlockStatusEnum.Stopped);

            _block2 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 3, ArenaYPosition = 1};
            _arena.AddBlock(_block2, Block.BlockStatusEnum.Stopped);

            _block3 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 4, ArenaYPosition = 1};
            _arena.AddBlock(_block3, Block.BlockStatusEnum.Stopped);

            _block4 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 3, ArenaYPosition = 2};
            _arena.AddBlock(_block4, Block.BlockStatusEnum.Stopped);

            _block5 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 3, ArenaYPosition = 3};
            _arena.AddBlock(_block5, Block.BlockStatusEnum.Stopped);

            _block6 = new Block(Block.BlockTypeEnum.Orange) {ArenaXPosition = 4, ArenaYPosition = 2};
            _arena.AddBlock(_block6, Block.BlockStatusEnum.Stopped);
        }

        [Test]
        public void MakeSureBlockOfDifferentColourDoesntGetMarkedForDeletion()
        {
            short connections = 0;

            connections = _arena.ChangeStatusOfLinkedBlocks(_block3.Type, Block.BlockStatusEnum.MarkedForDeletion,
                                                            _block3, connections);

            Assert.AreEqual(_block6.Status == Block.BlockStatusEnum.Stopped, true);
        }
    }


    [TestFixture]
    public class Test16
    {
        private Arena _arena;
        private Block _block1;
        private Block _block2;
        private Block _block3;
        private Block _block4;
        private Block _block5;
        private Block _block6;

        [TestFixtureSetUp]
        public void Init()
        {
            _arena = new Arena();
            _block1 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 2, ArenaYPosition = 1};
            _arena.AddBlock(_block1, Block.BlockStatusEnum.Stopped);

            _block2 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 3, ArenaYPosition = 1};
            _arena.AddBlock(_block2, Block.BlockStatusEnum.Stopped);

            _block3 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 4, ArenaYPosition = 1};
            _arena.AddBlock(_block3, Block.BlockStatusEnum.Stopped);

            _block4 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 3, ArenaYPosition = 2};
            _arena.AddBlock(_block4, Block.BlockStatusEnum.Stopped);

            _block5 = new Block(Block.BlockTypeEnum.Green) {ArenaXPosition = 3, ArenaYPosition = 3};
            _arena.AddBlock(_block5, Block.BlockStatusEnum.Stopped);

            _block6 = new Block(Block.BlockTypeEnum.Orange) {ArenaXPosition = 4, ArenaYPosition = 2};
            _arena.AddBlock(_block6, Block.BlockStatusEnum.Stopped);
        }

        [Test]
        public void ClearTheArena()
        {
            _arena.Clear();

            // currently no tests !!
        }
    }
}