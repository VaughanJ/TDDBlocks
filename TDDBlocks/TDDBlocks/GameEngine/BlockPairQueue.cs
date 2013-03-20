using System.Collections.Generic;

namespace TDDBlocks.GameEngine
{
    internal class BlockPairQueue
    {
        private readonly bool _addNewBlock;
        private readonly Queue<BlockPair> _blocksQueue = new Queue<BlockPair>();
        private readonly int _percentageChanceOfBlock1BeingAPowerBlock;
        private readonly int _percentageChanceOfBlock2BeingAPowerBlock;

        public BlockPairQueue(int queueLength, bool whenRemoveBlocksAddNewBlocksToQueue,
                              int percentageChanceOfBlock1BeingAPowerBlock, int percentageChanceOfBlock2BeingAPowerBlock)
        {
            _addNewBlock = whenRemoveBlocksAddNewBlocksToQueue;
            _percentageChanceOfBlock1BeingAPowerBlock = percentageChanceOfBlock1BeingAPowerBlock;
            _percentageChanceOfBlock2BeingAPowerBlock = percentageChanceOfBlock2BeingAPowerBlock;

            for (var bq = 0; bq < queueLength; bq++)
            {
                BlockPair nextBlocks = new BlockPair(percentageChanceOfBlock1BeingAPowerBlock,
                                               percentageChanceOfBlock2BeingAPowerBlock);

                _blocksQueue.Enqueue(nextBlocks);
            }
        }

        public BlockPair NextBlocks()
        {
            BlockPair returnedBlocks = _blocksQueue.Dequeue();

            if (_addNewBlock)
            {
                BlockPair nextBlocks = new BlockPair(_percentageChanceOfBlock1BeingAPowerBlock,
                                               _percentageChanceOfBlock2BeingAPowerBlock);

                _blocksQueue.Enqueue(nextBlocks);
            }

            return returnedBlocks;
        }

        public BlockPair ViewNextBlocks()
        {
            return _blocksQueue.Peek();
        }
    }
}