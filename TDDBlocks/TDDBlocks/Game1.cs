using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TDDBlocks.GameEngine;
using TDDBlocks.Logging;
using System;

namespace TDDBlocks
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        readonly GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        // Textures
        private Texture2D _background;
        private Texture2D _blueBlock;
        private Texture2D _greenBlock;
        private Texture2D _orangeBlock;
        private Texture2D _purpleBlock;
        private Texture2D _redBlock;
        private SpriteFont _gameFont;

        //Game Objects
        private readonly Arena _arena = new Arena();
 
        //Game variables
        private enum GameState
        {
            Playing,
            Paused 
        }

        private GameState _currentState = GameState.Playing;
        private int _score;
        private Point _arenaStartCoordinates = new Point(195, 630);
        private Point _nextBlocksCoordinates = new Point(675, 91);
        private Point _scoreDisplayPosition = new Point(718, 185);
        private const float GameTimePassedBeforeBlockDropNormal = 0.6F;
        private const float GameTimePassedBeforeBlockDropFast = 0.1F;
        private const float HorizontalMovesAllowedPerDrop = 8;
        private const float RotatesAllowedPerDrop = 4;
        private float _autoFallTimer;
        private float _allowMoveTimer;
        private float _allowRotateTimer;
        private short _frameCount;
        private short _frameRate;
        private TimeSpan _fpsTimer = TimeSpan.Zero;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _graphics.PreferredBackBufferHeight = 675;
            _graphics.PreferredBackBufferWidth = 900;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //Initialize the graphics, the game and the text objects
            InitializeGraphics();
            InitializeGame();
            InitializeText();
        }

        private void InitializeGraphics()
        {
            _background = Content.Load<Texture2D>("Images/Background");
            _blueBlock = Content.Load<Texture2D>("Images/Blue");
            _greenBlock = Content.Load<Texture2D>("Images/Green");
            _orangeBlock = Content.Load<Texture2D>("Images/Orange");
            _purpleBlock = Content.Load<Texture2D>("Images/Purple");
            _redBlock = Content.Load<Texture2D>("Images/Red");
        }

        //Initialize the Game
        private void InitializeGame()
        {
            Log.Instance.Trace("InitializeGame.");

            //Initialize the game data
           _score = 0;

            //Clear the arena of blocks
            _arena.Clear();

            //Set the arena up with a pair of falling blocks
            _arena.PutNextBlocksAtArenaStart();

            // No intro screen yet, game just goes straight into playing
            _currentState = GameState.Playing;
        }

        //Initialize the fonts to be used
        private void InitializeText()
        {
            _gameFont = Content.Load<SpriteFont>("GameFont");
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardInput = Keyboard.GetState();
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Allows the game to exit 
            if (keyboardInput.IsKeyDown(Keys.Escape)) Exit();

            //Update the game objects
            UpdateGame(elapsed, keyboardInput);

            CalculateFramesPerSecond(gameTime);

            base.Update(gameTime);
        }

        private short CalculateFramesPerSecond(GameTime gameTime)
        {
            _fpsTimer += gameTime.ElapsedGameTime;

            if (_fpsTimer > TimeSpan.FromSeconds(1))
            {
                _fpsTimer -= TimeSpan.FromSeconds(1);
                _frameRate = _frameCount;
                Log.Instance.Trace("Frames = " + _frameRate.ToString(CultureInfo.InvariantCulture) + ".");
                _frameCount = 0;
            }

            return _frameRate;
        }

        private void UpdateGame(float elapsed, KeyboardState keyboardInput)
        {
            if (_currentState == GameState.Playing)
            {
                UpdateMainGameObjects(elapsed, keyboardInput);
            }
        }

        private void UpdateMainGameObjects(float elapsed, KeyboardState keyboardInput)
        {
            // Deal with keyboard input
            if (keyboardInput.GetPressedKeys().Length > 0)
            {
                MoveBlockHorizontally(elapsed, keyboardInput);
                RotateBlocks(elapsed, keyboardInput);
                AllowBlockQuickDrop(keyboardInput);
            }

            // Attempt to drop blocks by one space
            DropBlockPair(elapsed);
        }

        private void AllowBlockQuickDrop(KeyboardState keyboardInput)
        {
            Log.Instance.Trace("AllowBlockQuickDrop.");

            if (keyboardInput.IsKeyDown(Keys.Down))
            {
                _arena.QuickDrop();
            }
        }

        private void RotateBlocks(float elapsed, KeyboardState keyboardInput)
        {
            _allowRotateTimer += elapsed;

            float gameTimeBeforeDrop = (_arena.CurrentBlockPair.VerticalSpeed == BlockPair.VerticalSpeedEnum.Normal ? GameTimePassedBeforeBlockDropNormal : GameTimePassedBeforeBlockDropFast);

            if (_allowRotateTimer >= (gameTimeBeforeDrop / RotatesAllowedPerDrop))
            {
                if (keyboardInput.IsKeyDown(Keys.Left))
                {
                    _arena.RotateBlocks(BlockPair.RotationEnum.Clockwise);
                }
                if (keyboardInput.IsKeyDown(Keys.Right))
                {
                    _arena.RotateBlocks(BlockPair.RotationEnum.AntiClockwise);
                }
                _allowRotateTimer = 0;
            }
        }

        private void MoveBlockHorizontally(float elapsed, KeyboardState keyboardInput)
        {
            _allowMoveTimer += elapsed;

            float gameTimeBeforeDrop = (_arena.CurrentBlockPair.VerticalSpeed == BlockPair.VerticalSpeedEnum.Normal ? GameTimePassedBeforeBlockDropNormal : GameTimePassedBeforeBlockDropFast);

            if (_arena.CurrentBlockPair.Block1.Status == Block.BlockStatusEnum.Dropping &&
                _arena.CurrentBlockPair.Block2.Status == Block.BlockStatusEnum.Dropping)
            {
                if (_allowMoveTimer >= (gameTimeBeforeDrop / HorizontalMovesAllowedPerDrop))
                {
                    if (keyboardInput.IsKeyDown(Keys.Z)) _arena.MoveBlocksLeft();
                    if (keyboardInput.IsKeyDown(Keys.X)) _arena.MoveBlocksRight();

                    if (_arena.CurrentBlockPair.HorizontalDirection == BlockPair.HorizontalDirectionEnum.Left)
                    {
                        _arena.MoveBlocksLeft();
                        _arena.CurrentBlockPair.HorizontalDirection = BlockPair.HorizontalDirectionEnum.None;
                    }

                    if (_arena.CurrentBlockPair.HorizontalDirection == BlockPair.HorizontalDirectionEnum.Right)
                    {
                        _arena.MoveBlocksRight();
                        _arena.CurrentBlockPair.HorizontalDirection = BlockPair.HorizontalDirectionEnum.None;
                    }
                    _allowMoveTimer = 0;
                }
            }
        }

        private void DropBlockPair(float elapsed)
        {
            bool dropBlock1First = false;

            _autoFallTimer += elapsed;

            float gameTimeBeforeDrop = (_arena.CurrentBlockPair.VerticalSpeed == BlockPair.VerticalSpeedEnum.Normal ? GameTimePassedBeforeBlockDropNormal : GameTimePassedBeforeBlockDropFast);

            if (_autoFallTimer >= gameTimeBeforeDrop)
            {
                // determine which block needs to drop first based on who is lowest
                if (_arena.CurrentBlockPair.Block1.ArenaYPosition < _arena.CurrentBlockPair.Block2.ArenaYPosition) dropBlock1First = true;

                if (dropBlock1First)
                {
                    if (_arena.CanBlockDrop(_arena.CurrentBlockPair.Block1))
                    {
                        _arena.DropBlock(_arena.CurrentBlockPair.Block1);
                    }
                    else
                    {
                        _arena.CurrentBlockPair.Block1.Status = Block.BlockStatusEnum.Stopped; // block 2 = detached ?
                        _arena.CurrentBlockPair.VerticalSpeed = BlockPair.VerticalSpeedEnum.DroppingFast;
                    }

                    if (_arena.CanBlockDrop(_arena.CurrentBlockPair.Block2))
                    {
                        _arena.DropBlock(_arena.CurrentBlockPair.Block2);
                    }
                    else
                    {
                        _arena.CurrentBlockPair.Block2.Status = Block.BlockStatusEnum.Stopped;
                        _arena.CurrentBlockPair.VerticalSpeed = BlockPair.VerticalSpeedEnum.DroppingFast;
                    }

                }
                else
                {
                    if (_arena.CanBlockDrop(_arena.CurrentBlockPair.Block2))
                    {
                        _arena.DropBlock(_arena.CurrentBlockPair.Block2);
                    }
                    else
                    {
                        _arena.CurrentBlockPair.Block2.Status = Block.BlockStatusEnum.Stopped;
                        _arena.CurrentBlockPair.VerticalSpeed = BlockPair.VerticalSpeedEnum.DroppingFast;
                    }
                    
                    if (_arena.CanBlockDrop(_arena.CurrentBlockPair.Block1))
                    {
                        _arena.DropBlock(_arena.CurrentBlockPair.Block1);
                    }
                    else
                    {
                        _arena.CurrentBlockPair.Block1.Status = Block.BlockStatusEnum.Stopped; // block 2 = detached ?
                        _arena.CurrentBlockPair.VerticalSpeed = BlockPair.VerticalSpeedEnum.DroppingFast;
                    }

                }

                _autoFallTimer = 0;

                // When both blocks have settled then we need to check for linked blocks.
                if (_arena.CurrentBlockPair.Block1.Status == Block.BlockStatusEnum.Stopped && _arena.CurrentBlockPair.Block2.Status == Block.BlockStatusEnum.Stopped)
                {
                    if ((_arena.CurrentBlockPair.Block1.ArenaYPosition == _arena.HeightIncludingStartLine) | (_arena.CurrentBlockPair.Block2.ArenaYPosition == _arena.HeightIncludingStartLine))
                    {
                        // Game over, but reset the game for the time being
                        InitializeGame();
                    }
                    else
                    {
                         _score += _arena.PostBlockSettleChecks();
                        _arena.PutNextBlocksAtArenaStart();   
                    }
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            _frameCount++;

            _graphics.GraphicsDevice.Clear(Color.Black);
            
            _spriteBatch.Begin();
            if (_currentState == GameState.Playing)
            {
                _spriteBatch.Draw(_background, new Rectangle(0, 0, 850, 675), Color.White);
                DrawArena();
                DrawNextBlocks();
                DrawText();
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawText()
        {
            _spriteBatch.DrawString(_gameFont, _score.ToString(CultureInfo.InvariantCulture), new Vector2(_scoreDisplayPosition.X - ((int)_gameFont.MeasureString(_score.ToString(CultureInfo.InvariantCulture)).X / 2), _scoreDisplayPosition.Y), Color.Black);
        }

        private void DrawNextBlocks()
        {
            BlockPair nextBlocks = _arena.ViewNextBlocks();

            _spriteBatch.Draw(GetImage(nextBlocks.Block1.Type), new Rectangle(_nextBlocksCoordinates.X, _nextBlocksCoordinates.Y, _arena.GridSquareWidth, _arena.GridSquareHeight), Color.White);
            _spriteBatch.Draw(GetImage(nextBlocks.Block2.Type), new Rectangle(_nextBlocksCoordinates.X + 45, _nextBlocksCoordinates.Y, _arena.GridSquareWidth, _arena.GridSquareHeight), Color.White);
        }

        private void DrawArena()
        {

            for (int drawYLoop = 1; drawYLoop <= _arena.HeightIncludingStartLine; drawYLoop++)
            {
                for (int drawXLoop = 1; drawXLoop <= _arena.Width; drawXLoop++)
                {
                    Block blockToDraw = _arena.GetBlockAtPosition(drawXLoop, drawYLoop);

                    if (blockToDraw != null)
                    {
                        _spriteBatch.Draw(GetImage(blockToDraw.Type), new Rectangle(_arenaStartCoordinates.X + ((drawXLoop - 1) * _arena.GridSquareWidth), _arenaStartCoordinates.Y - (drawYLoop * _arena.GridSquareHeight),
                        _arena.GridSquareWidth, _arena.GridSquareHeight),Color.White);
                    }
                }
            }
        }

        private Texture2D GetImage(Block.BlockTypeEnum type)
        {
            Texture2D returnTexture = null;

            switch (type)
            {
                case Block.BlockTypeEnum.Red:
                    returnTexture = _redBlock;
                    break;
                case Block.BlockTypeEnum.Green:
                    returnTexture = _greenBlock;
                    break;
                case Block.BlockTypeEnum.Blue:
                    returnTexture = _blueBlock;
                    break;
                case Block.BlockTypeEnum.Orange:
                    returnTexture = _orangeBlock;
                    break;
                case Block.BlockTypeEnum.Purple:
                    returnTexture = _purpleBlock;
                    break;
               // case Block.BlockTypeEnum.BigExplosion:
               //     break;
               // case Block.BlockTypeEnum.DelayedExplosion:
               //     break;
               // case Block.BlockTypeEnum.Rock:
               //     break;
               // case Block.BlockTypeEnum.InIce:
               //     break;
               // case Block.BlockTypeEnum.MorphingBlock:
               //     break;
            }

            return returnTexture;
        }
    }
}
