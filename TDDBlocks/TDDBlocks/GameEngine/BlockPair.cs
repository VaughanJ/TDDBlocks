using System;

namespace TDDBlocks.GameEngine
{
    public class BlockPair
    {
        public delegate void BothBlocksStoppedHandler(object sender, BothBlocksStoppedHandlerArgs args);
 
        public enum HorizontalDirectionEnum
        {
            None,
            Left,
            Right
        }

        public enum OrentationEnum
        {
            Right = 0,
            Below = 1,
            Left = 2,
            Above = 3
        }

        public enum RotationEnum
        {
            None,
            Clockwise,
            AntiClockwise
        }

        public enum VerticalSpeedEnum
        {
            Normal,
            DroppingFast
        }

        public BlockPair(Block.BlockTypeEnum blockType1, Block.BlockTypeEnum blockType2)
        {
            Block1 = new Block(blockType1);
            Block2 = new Block(blockType2);

            BlockPairDefaultConfig();
        }

        public BlockPair(int percentageChanceOfPowerBlock1, int percentageChanceOfPowerBlock2)
        {
            Block1 = new Block(percentageChanceOfPowerBlock1);
            Block2 = new Block(percentageChanceOfPowerBlock2);

            BlockPairDefaultConfig();
        }

        public OrentationEnum Orientation { get; set; }

        public RotationEnum RotationDirection { get; set; }

        public HorizontalDirectionEnum HorizontalDirection { get; set; }

        public VerticalSpeedEnum VerticalSpeed { get; set; }

        public Block Block1 { get; private set; }

        public Block Block2 { get; private set; }

        public void StatusChanged(object sender, StatusChangedHandlerArgs args)
        {
            if (args.NewStatus == Block.BlockStatusEnum.Stopped)
            {
                if ((Block1.Status == Block.BlockStatusEnum.Stopped) && (Block2.Status == Block.BlockStatusEnum.Stopped))
                {
                    if (BothBlocksStopped != null)
                    {
                        BothBlocksStopped(this, new BothBlocksStoppedHandlerArgs(Block1, Block2));
                    }
                }
            }
        }

        private void BlockPairDefaultConfig()
        {
            RotationDirection = RotationEnum.None;
            HorizontalDirection = HorizontalDirectionEnum.None;
            VerticalSpeed = VerticalSpeedEnum.Normal;

            Block1.StatusChange += StatusChanged;
            Block2.StatusChange += StatusChanged;
        }

        public event BothBlocksStoppedHandler BothBlocksStopped;

        protected void OnStatusChange(object sender, BothBlocksStoppedHandlerArgs args)
        {
            if (BothBlocksStopped != null)
            {
                BothBlocksStopped(this, args);
            }
        }
    }

    public class BothBlocksStoppedHandlerArgs : EventArgs
    {
        public BothBlocksStoppedHandlerArgs(Block block1, Block block2)
        {
            Block1 = block1;
            Block2 = block2;
        }

        private Block Block1 { get; set; }
        private Block Block2 { get; set; }
    }
}