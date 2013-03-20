using System;

namespace TDDBlocks.GameEngine
{
    public class Block
    {
        private static readonly Random Random = new Random();

        public delegate void StatusChangedHandler(object sender, StatusChangedHandlerArgs args);

        private const short EnumStartPositionForPowerBlocks = 6;

        public enum BlockStatusEnum
        {
            NotInPlay,
            Stopped,
            Dropping,
            Detatched,
            MarkedForDeletion,
            Exploding
        }

        public enum BlockTypeEnum
        {
            Red,
            Green, 
            Blue, 
            Orange, 
            Purple, 
            BigExplosion,
            DelayedExplosion, 
            Rock, 
            InIce, 
            MorphingBlock 
        }

        private BlockStatusEnum _previousStatus;

        private BlockStatusEnum _status;

        public Block(BlockTypeEnum currentBlockType)
        {
            Type = currentBlockType;
            Status = BlockStatusEnum.NotInPlay;
        }

        public Block(int percentageChaneOfBeingPowerBlock)
        {
            Array availableBlocks = Enum.GetValues(typeof(BlockTypeEnum));

            int randomNumber = Random.Next(0, 100);

            if (randomNumber >= percentageChaneOfBeingPowerBlock)
            {
                Type = (BlockTypeEnum)availableBlocks.GetValue(Random.Next(0, EnumStartPositionForPowerBlocks - 1)); 
            }
            else
            {
                Type = (BlockTypeEnum)availableBlocks.GetValue(Random.Next(EnumStartPositionForPowerBlocks, availableBlocks.Length)); 
            }

            Status = BlockStatusEnum.NotInPlay;
        }

        public int ArenaXPosition { get; set; }

        public int ArenaYPosition { get; set; }

        public BlockTypeEnum Type { get; private set; }

        public BlockStatusEnum Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnStatusChange(this,
                               new StatusChangedHandlerArgs(_previousStatus, value, ArenaXPosition, ArenaYPosition));
                _previousStatus = value;
            }
        }

        public event StatusChangedHandler StatusChange;

        protected void OnStatusChange(object sender, StatusChangedHandlerArgs args)
        {
            if (StatusChange != null)
            {
                StatusChange(this, args);
            }
        }

    }

    public class StatusChangedHandlerArgs : EventArgs
    {
        public StatusChangedHandlerArgs(Block.BlockStatusEnum previousStatus, Block.BlockStatusEnum newStatus, int x,
                                        int y)
        {
            PreviousStatus = previousStatus;
            NewStatus = newStatus;
            X = x;
            Y = y;
        }

        private Block.BlockStatusEnum PreviousStatus { get; set; }
        public Block.BlockStatusEnum NewStatus { get; private set; }
        private int X { get; set; }
        public int Y { get; private set; }
    }
}