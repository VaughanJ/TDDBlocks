using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Xna.Framework;
using TDDBlocks.Logging;

namespace TDDBlocks.GameEngine
{
    public class Arena
    {
        private const int ArrayOffset = 1;
        private const int BlockBelow = 1;
        private const int ArenaFloor = 1;
        private const short LinkedBlocksRequired = 4;
        private const int LeftArenaWall = 1;
        private const int ScoreMultiplier = 100;

        private readonly Point _block1StartPosition;
        private readonly Point _block2StartPosition;
        private readonly Block[,] _blockArena = new Block[0,0];
        private readonly BlockPairQueue _blockPairQueue;
        private readonly int _gridSquareHeight;
        private readonly int _gridSquareWidth;
        private readonly int _height;
        private readonly int _heightIncludingStartLine;
        private readonly int _width;
        private BlockPair _currentBlockPair;

        public Arena()
        {
            // Set default arena width and height
            _width = 8;
            _height = 13;
            _heightIncludingStartLine = (_height + 1);

            // Set default block start positions 
            _block1StartPosition.X = 4;
            _block1StartPosition.Y = _heightIncludingStartLine;

            _block2StartPosition.X = 5;
            _block2StartPosition.Y = _heightIncludingStartLine;

            // Create the arena size
            _blockArena = new Block[_width,_heightIncludingStartLine];

            _gridSquareWidth = 45;
            _gridSquareHeight = 45;

            _blockPairQueue = new BlockPairQueue(2, true, 0, 0);
        }

        public BlockPair CurrentBlockPair
        {
            get { return _currentBlockPair; }
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public int HeightIncludingStartLine
        {
            get { return _heightIncludingStartLine; }
        }

        public Point Block1StartPosition
        {
            get { return _block1StartPosition; }
        }

        public Point Block2StartPosition
        {
            get { return _block2StartPosition; }
        }

        public int GridSquareHeight
        {
            get { return _gridSquareHeight; }
        }

        public int GridSquareWidth
        {
            get { return _gridSquareWidth; }
        }

        public void Clear()
        {
            Log.Instance.Trace("Clear Arena.");

            for (int height = 1; height <= _heightIncludingStartLine; height++)
            {
                for (int width = 1; width <= _width; width++)
                {
                    _blockArena[width - ArrayOffset, height - ArrayOffset] = null;
                }
            }
        }

        public bool CanBlockDrop(Block blockToTest)
        {
            bool canDrop =
                !((blockToTest.ArenaYPosition == ArenaFloor) ||
                  ((_blockArena[
                      blockToTest.ArenaXPosition - ArrayOffset, blockToTest.ArenaYPosition - ArrayOffset - BlockBelow] !=
                    null)));

            Log.Instance.Trace(string.Format("CanBlockDrop [BlockNo] [{0},{1}] = {2}", blockToTest.ArenaXPosition.ToString(CultureInfo.InvariantCulture), blockToTest.ArenaYPosition.ToString(CultureInfo.InvariantCulture), canDrop.ToString()));

            return canDrop;
        }

        public void AddBlock(Block blockToAdd, Block.BlockStatusEnum newBlockStatus)
        {
            AddBlockBase(blockToAdd);

            blockToAdd.Status = newBlockStatus;
        }

        private void AddBlock(Block blockToAdd)
        {
            AddBlockBase(blockToAdd);
        }

        private void RemoveBlock(Block blockToRemove)
        {
            if (
                _blockArena[(blockToRemove.ArenaXPosition - ArrayOffset), (blockToRemove.ArenaYPosition - ArrayOffset)] ==
                null)
            {
                throw new Exception("There is no block to remove at this location.");
            }

            _blockArena[blockToRemove.ArenaXPosition - ArrayOffset, blockToRemove.ArenaYPosition - ArrayOffset] = null;
        }

        public void DropBlock(Block blockToDrop)
        {
            if (CanBlockDrop(blockToDrop))
            {
                RemoveBlock(blockToDrop);
                blockToDrop.ArenaYPosition -= 1;
                AddBlock(blockToDrop);
            }
        }

        private void AddBlockBase(Block blockToAdd)
        {
            if (_blockArena[(blockToAdd.ArenaXPosition - ArrayOffset), (blockToAdd.ArenaYPosition - ArrayOffset)] !=
                null)
            {
                Debugger.Break();
                Log.Instance.Error("There is already a block present at this location.");
                throw new Exception("There is already a block present at this location.");
            }

            Log.Instance.Trace("AddBlockBase [BlockNo] [" + blockToAdd.ArenaXPosition.ToString(CultureInfo.InvariantCulture) + "," +
                               blockToAdd.ArenaYPosition.ToString(CultureInfo.InvariantCulture) + "]");

            _blockArena[blockToAdd.ArenaXPosition - ArrayOffset, blockToAdd.ArenaYPosition - ArrayOffset] = blockToAdd;
        }

        public void MarkSuspendedBlocksAsDropping()
        {
            for (int yloop = 0; yloop < _height - 1; yloop++)
            {
                for (int xloop = 0; xloop < _width - 1; xloop++)
                {
                    if (_blockArena[xloop, yloop] != null)
                    {
                        if (CanBlockDrop(_blockArena[xloop, yloop]))
                        {
                            Log.Instance.Trace("MarkSuspendedBlocksAsDropping [" +
                                               _blockArena[xloop, yloop].ArenaXPosition.ToString(CultureInfo.InvariantCulture) + "," +
                                               _blockArena[xloop, yloop].ArenaYPosition.ToString(CultureInfo.InvariantCulture) + "]");
                            _blockArena[xloop, yloop].Status = Block.BlockStatusEnum.Dropping;
                        }
                    }
                }
            }
        }

        public short ChangeStatusOfLinkedBlocks(Block.BlockTypeEnum checkingForBlockType,
                                                Block.BlockStatusEnum changeToBlockStatus, Block testBlock,
                                                short currentConnectionCount)
        {
            if (testBlock != null)
            {
                if (testBlock.Type == checkingForBlockType)
                {
                    if (testBlock.Status != changeToBlockStatus)
                    {
                        testBlock.Status = changeToBlockStatus;
                        currentConnectionCount++;

                        if (testBlock.ArenaXPosition < _blockArena.GetLength(0))
                        {
                            currentConnectionCount = ChangeStatusOfLinkedBlocks(checkingForBlockType,
                                                                                changeToBlockStatus,
                                                                                _blockArena[
                                                                                    testBlock.ArenaXPosition -
                                                                                    ArrayOffset + 1,
                                                                                    testBlock.ArenaYPosition -
                                                                                    ArrayOffset],
                                                                                currentConnectionCount);
                        }

                        if (testBlock.ArenaXPosition > 1)
                        {
                            currentConnectionCount = ChangeStatusOfLinkedBlocks(checkingForBlockType,
                                                                                changeToBlockStatus,
                                                                                _blockArena[
                                                                                    testBlock.ArenaXPosition -
                                                                                    ArrayOffset - 1,
                                                                                    testBlock.ArenaYPosition -
                                                                                    ArrayOffset],
                                                                                currentConnectionCount);
                        }

                        if (testBlock.ArenaYPosition < _blockArena.GetLength(1))
                        {
                            currentConnectionCount = ChangeStatusOfLinkedBlocks(checkingForBlockType,
                                                                                changeToBlockStatus,
                                                                                _blockArena[
                                                                                    testBlock.ArenaXPosition -
                                                                                    ArrayOffset,
                                                                                    testBlock.ArenaYPosition -
                                                                                    ArrayOffset + 1],
                                                                                currentConnectionCount);
                        }

                        if (testBlock.ArenaYPosition > 1)
                        {
                            currentConnectionCount = ChangeStatusOfLinkedBlocks(checkingForBlockType,
                                                                                changeToBlockStatus,
                                                                                _blockArena[
                                                                                    testBlock.ArenaXPosition -
                                                                                    ArrayOffset,
                                                                                    testBlock.ArenaYPosition -
                                                                                    ArrayOffset - 1],
                                                                                currentConnectionCount);
                        }
                    }
                }
            }

            return currentConnectionCount;
        }

        public Block GetBlockAtPosition(int arenaXPosition, int arenaYPosition)
        {
            Block returnBlock = null;

            if (((arenaXPosition - ArrayOffset) >= 0) &&
                ((arenaXPosition - ArrayOffset) <= _blockArena.GetUpperBound(0))
                && ((arenaYPosition - ArrayOffset) >= 0) &&
                ((arenaYPosition - ArrayOffset) <= _blockArena.GetUpperBound(1)))
            {
                returnBlock = _blockArena[arenaXPosition - ArrayOffset, arenaYPosition - ArrayOffset];
            }

            return returnBlock;
        }

        private bool RemoveExplodingBlocks()
        {
            bool blocksRemoved = false;

            Log.Instance.Trace("RemoveExplodingBlocks.");

            for (int checkYLoop = 1; checkYLoop <= HeightIncludingStartLine; checkYLoop++)
            {
                for (int checkXLoop = 1; checkXLoop <= Width; checkXLoop++)
                {
                    Block blockToCheck = GetBlockAtPosition(checkXLoop, checkYLoop);

                    if ((blockToCheck != null) && (blockToCheck.Status == Block.BlockStatusEnum.Exploding))
                    {
                        RemoveBlock(blockToCheck);
                        blocksRemoved = true;
                    }
                }
            }

            return blocksRemoved;
        }

        private int CheckForLinkedBlocks()
        {
            int linkedBlocksScore = 0;
            int seperatelyLinked = 0;

            for (int checkYLoop = 1; checkYLoop <= HeightIncludingStartLine; checkYLoop++)
            {
                for (int checkXLoop = 1; checkXLoop <= Width; checkXLoop++)
                {
                    Block blockToCheck = GetBlockAtPosition(checkXLoop, checkYLoop);

                    if ((blockToCheck != null) && (blockToCheck.Status != Block.BlockStatusEnum.Exploding))
                    {
                        short linkedBlocks = ChangeStatusOfLinkedBlocks(blockToCheck.Type,
                                                                               Block.BlockStatusEnum.MarkedForDeletion,
                                                                               blockToCheck, 0);

                        // If we have met the required linked blocks target then marked as exploding, otherwise mark as stopped.
                        ChangeStatusOfLinkedBlocks(blockToCheck.Type, linkedBlocks >= LinkedBlocksRequired ? Block.BlockStatusEnum.Exploding : Block.BlockStatusEnum.Stopped, blockToCheck, 0);

                        if (linkedBlocks >= LinkedBlocksRequired)
                        {
                            seperatelyLinked += 1;
                            linkedBlocksScore += (linkedBlocks * seperatelyLinked);
                        }
                    }
                }
            }
            return linkedBlocksScore;
        }

        private bool DropSuspendedBlocks()
        {
            bool checkForSuspenedBlocks;
            bool blockDropped = false;

            do
            {
                checkForSuspenedBlocks = false;
                // assume we wont need to check for any more blocks after the first check

                for (int checkYLoop = 1; checkYLoop <= HeightIncludingStartLine; checkYLoop++)
                {
                    for (int checkXLoop = 1; checkXLoop <= Width; checkXLoop++)
                    {
                        Block blockToCheck = GetBlockAtPosition(checkXLoop, checkYLoop);

                        if (blockToCheck != null)
                        {
                            if (CanBlockDrop(blockToCheck))
                            {
                                DropBlock(blockToCheck);
                                checkForSuspenedBlocks = true;
                                // As blocks have moved, check that there are no more blocks that need to drop
                                blockDropped = true;
                            }
                            else
                            {
                                blockToCheck.Status = Block.BlockStatusEnum.Stopped;
                            }
                        }
                    }
                }
            } while (checkForSuspenedBlocks);

            Log.Instance.Trace("DropSuspendedBlocks = " + blockDropped.ToString() + ".");

            return blockDropped;
        }

        public void PutNextBlocksAtArenaStart()
        {
            Log.Instance.Trace("PutNextBlocksAtArenaStart");

            _currentBlockPair = _blockPairQueue.NextBlocks();

            _currentBlockPair.Block1.ArenaXPosition = Block1StartPosition.X;
            _currentBlockPair.Block1.ArenaYPosition = Block1StartPosition.Y;
            _currentBlockPair.Block2.ArenaXPosition = Block2StartPosition.X;
            _currentBlockPair.Block2.ArenaYPosition = Block2StartPosition.Y;

            AddBlock(_currentBlockPair.Block1, Block.BlockStatusEnum.Dropping);
            AddBlock(_currentBlockPair.Block2, Block.BlockStatusEnum.Dropping);
        }

        public BlockPair ViewNextBlocks()
        {
            return _blockPairQueue.ViewNextBlocks();
        }

        public int PostBlockSettleChecks()
        {
            int thisRoundsScore = 0;
            bool blockDropped;
            int chains = 0;

            Log.Instance.Trace("PostBlockSettleChecks.");

            do
            {
                blockDropped = false;
                thisRoundsScore += CheckForLinkedBlocks();

                if (RemoveExplodingBlocks())
                {
                    MarkSuspendedBlocksAsDropping();
                    blockDropped = DropSuspendedBlocks();

                    chains += 1;
                    thisRoundsScore *= chains;
                }

            } while (blockDropped);

            return thisRoundsScore * ScoreMultiplier;
        }

        public bool MoveBlocksLeft()
        {
            bool movedLeft;

            if (CanPairMoveLeft())
            {
                RemoveBlock(_currentBlockPair.Block1);
                RemoveBlock(_currentBlockPair.Block2);
                _currentBlockPair.Block1.ArenaXPosition -= 1;
                AddBlock(_currentBlockPair.Block1);
                _currentBlockPair.Block2.ArenaXPosition -= 1;
                AddBlock(_currentBlockPair.Block2);

                Log.Instance.Trace("MoveBlocksLeft.");

                movedLeft = true;
            }
            else
            {
                movedLeft = false;
            }

            return movedLeft;
        }

        public bool MoveBlocksRight()
        {
            bool movedRight;

            if (CanPairMoveRight())
            {
                RemoveBlock(_currentBlockPair.Block2);
                RemoveBlock(_currentBlockPair.Block1);
                _currentBlockPair.Block2.ArenaXPosition += 1;
                AddBlock(_currentBlockPair.Block2);
                _currentBlockPair.Block1.ArenaXPosition += 1;
                AddBlock(_currentBlockPair.Block1);

                Log.Instance.Trace("MoveBlocksRight.");

                movedRight = true;
            }
            else
            {
                movedRight = false;
            }

            return movedRight;
        }

        public void RotateBlocks(BlockPair.RotationEnum rotateDirection)
        {
            if (CanRotate(rotateDirection))
            {
                Log.Instance.Trace("RotateBlocks " + _currentBlockPair.RotationDirection.ToString() + '.');

                if (_currentBlockPair.RotationDirection == BlockPair.RotationEnum.Clockwise)
                {
                    _currentBlockPair.Orientation += 1;
                    if ((int)_currentBlockPair.Orientation > (int)BlockPair.OrentationEnum.Above)
                    {
                        _currentBlockPair.Orientation = BlockPair.OrentationEnum.Right;
                    }
                }
                else
                {
                    _currentBlockPair.Orientation -= 1;
                    if ((int)_currentBlockPair.Orientation < (int)BlockPair.OrentationEnum.Right)
                    {
                        _currentBlockPair.Orientation = BlockPair.OrentationEnum.Above;
                    }
                }

                RemoveBlock(_currentBlockPair.Block2);

                switch (_currentBlockPair.Orientation)
                {
                    case BlockPair.OrentationEnum.Right:
                        _currentBlockPair.Block2.ArenaXPosition = _currentBlockPair.Block1.ArenaXPosition + 1;
                        _currentBlockPair.Block2.ArenaYPosition = _currentBlockPair.Block1.ArenaYPosition;
                        break;
                    case BlockPair.OrentationEnum.Below:
                        _currentBlockPair.Block2.ArenaXPosition = _currentBlockPair.Block1.ArenaXPosition;
                        _currentBlockPair.Block2.ArenaYPosition = _currentBlockPair.Block1.ArenaYPosition - 1;
                        break;
                    case BlockPair.OrentationEnum.Left:
                        _currentBlockPair.Block2.ArenaXPosition = _currentBlockPair.Block1.ArenaXPosition - 1;
                        _currentBlockPair.Block2.ArenaYPosition = _currentBlockPair.Block1.ArenaYPosition;
                        break;
                    case BlockPair.OrentationEnum.Above:
                        _currentBlockPair.Block2.ArenaXPosition = _currentBlockPair.Block1.ArenaXPosition;
                        _currentBlockPair.Block2.ArenaYPosition = _currentBlockPair.Block1.ArenaYPosition + 1;
                        break;
                }

                AddBlock(_currentBlockPair.Block2);
            }
        }

        private bool CanRotate(BlockPair.RotationEnum rotateDirection)
        {
            int testArenaPositionX = 0;
            int testArenaPositionY = 0;
            bool canRotate = false;
            BlockPair.OrentationEnum newOrientation = _currentBlockPair.Orientation;

            if (((_currentBlockPair.Block2.ArenaXPosition > LeftArenaWall) &&
                 (rotateDirection == BlockPair.RotationEnum.Clockwise)) ||
                ((_currentBlockPair.Block2.ArenaXPosition > LeftArenaWall) &&
                 (rotateDirection == BlockPair.RotationEnum.Clockwise)) ||
                ((_currentBlockPair.Block2.ArenaXPosition > LeftArenaWall) &&
                 (rotateDirection == BlockPair.RotationEnum.Clockwise)) ||
                ((_currentBlockPair.Block2.ArenaXPosition < Width) &&
                 (rotateDirection == BlockPair.RotationEnum.AntiClockwise)))
            {
                canRotate = true;
            }

            // if we have passed the arena wall test above, test arena block collision
            if (canRotate)
            {
                if (_currentBlockPair.RotationDirection == BlockPair.RotationEnum.Clockwise)
                {
                    newOrientation += 1;
                    if ((int)newOrientation > (int)BlockPair.OrentationEnum.Above)
                    {
                        newOrientation = BlockPair.OrentationEnum.Right;
                    }
                }
                else
                {
                    newOrientation -= 1;
                    if ((int)_currentBlockPair.Orientation < (int)BlockPair.OrentationEnum.Right)
                    {
                        newOrientation = BlockPair.OrentationEnum.Above;
                    }
                }

                switch (newOrientation)
                {
                    case BlockPair.OrentationEnum.Right:
                        testArenaPositionX = _currentBlockPair.Block1.ArenaXPosition + 1;
                        testArenaPositionY = _currentBlockPair.Block1.ArenaYPosition;
                        break;
                    case BlockPair.OrentationEnum.Below:
                        testArenaPositionX = _currentBlockPair.Block1.ArenaXPosition;
                        testArenaPositionY = _currentBlockPair.Block1.ArenaYPosition - 1;
                        break;
                    case BlockPair.OrentationEnum.Left:
                        testArenaPositionX = _currentBlockPair.Block1.ArenaXPosition - 1;
                        testArenaPositionY = _currentBlockPair.Block1.ArenaYPosition;
                        break;
                    case BlockPair.OrentationEnum.Above:
                        testArenaPositionX = _currentBlockPair.Block1.ArenaXPosition;
                        testArenaPositionY = _currentBlockPair.Block1.ArenaYPosition + 1;
                        break;
                }

                // Check the block we are moving to is empty
                canRotate = GetBlockAtPosition(testArenaPositionX, testArenaPositionY) == null;
            }

            return canRotate;
        }

        public void QuickDrop()
        {
            Log.Instance.Trace("QuickDrop.");

            _currentBlockPair.VerticalSpeed = BlockPair.VerticalSpeedEnum.DroppingFast;
        }

        private bool CanPairMoveRight()
        {
            bool moveRight = false;

            if (_currentBlockPair.Block1.ArenaXPosition < Width && _currentBlockPair.Block2.ArenaXPosition < Width)
            {
                switch (_currentBlockPair.Orientation)
                {
                    case BlockPair.OrentationEnum.Right:
                        moveRight = GetBlockAtPosition(_currentBlockPair.Block2.ArenaXPosition + 1,
                                                       _currentBlockPair.Block2.ArenaYPosition) == null;
                        break;
                    case BlockPair.OrentationEnum.Below:
                    case BlockPair.OrentationEnum.Above:
                        moveRight = (GetBlockAtPosition(_currentBlockPair.Block1.ArenaXPosition + 1,
                                                        _currentBlockPair.Block1.ArenaYPosition) == null &&
                                     GetBlockAtPosition(_currentBlockPair.Block2.ArenaXPosition + 1,
                                                        _currentBlockPair.Block2.ArenaYPosition) == null);
                        break;
                    case BlockPair.OrentationEnum.Left:
                        moveRight = GetBlockAtPosition(_currentBlockPair.Block1.ArenaXPosition + 1,
                                                       _currentBlockPair.Block1.ArenaYPosition) == null;
                        break;
                }
            }

            return moveRight;
        }

        private bool CanPairMoveLeft()
        {
            bool moveLeft = false;

            if ((_currentBlockPair.Block1.ArenaXPosition > LeftArenaWall) &&
                (_currentBlockPair.Block2.ArenaXPosition > LeftArenaWall))
            {
                switch (_currentBlockPair.Orientation)
                {
                    case BlockPair.OrentationEnum.Right:
                        moveLeft = GetBlockAtPosition(_currentBlockPair.Block1.ArenaXPosition - 1,
                                                      _currentBlockPair.Block1.ArenaYPosition) == null;
                        break;
                    case BlockPair.OrentationEnum.Below:
                    case BlockPair.OrentationEnum.Above:
                        moveLeft = (GetBlockAtPosition(_currentBlockPair.Block1.ArenaXPosition - 1,
                                                       _currentBlockPair.Block1.ArenaYPosition) == null &&
                                    GetBlockAtPosition(_currentBlockPair.Block2.ArenaXPosition - 1,
                                                       _currentBlockPair.Block2.ArenaYPosition) == null);
                        break;
                    case BlockPair.OrentationEnum.Left:
                        moveLeft = GetBlockAtPosition(_currentBlockPair.Block2.ArenaXPosition - 1,
                                                      _currentBlockPair.Block2.ArenaYPosition) == null;
                        break;
                }
            }

            return moveLeft;
        }
    }
}