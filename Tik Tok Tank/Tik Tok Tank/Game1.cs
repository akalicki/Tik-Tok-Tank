using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Tik_Tok_Tank
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Declarations

        // Main declarations: graphics component and sprite-drawing class
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Texture declarations
        Texture2D spriteSheet;
        Texture2D menuBackground;

        // font declarations
        SpriteFont menuItemFont;

        // Game states
        enum GameStates { TitleMenu, OptionsMenu, ControlsMenu, GameModesMenu, PauseMenu, GameOverMenu, Standard, Duel, Adventure, Coop };
        GameStates gameState = GameStates.TitleMenu;

        // Menu declarations and timers
        Menus.Menu menu = new Menus.TitleMenu();
        GameStates prevState; // used for pausing game
        float menuMinTimer = 15f;
        float menuTimer = 15f;
        float gameOverMinTimer = 180f;
        float gameOverTimer = 0f;

        // player controls
        Player.Controls player1Controls = Player.Controls.Computer;
        Player.Controls player2Controls = Player.Controls.Xbox1;

        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // set window size
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 600;
            this.graphics.ApplyChanges();

            // set the mouse to be visible in game window
            this.IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load textures used for game
            spriteSheet = Content.Load<Texture2D>(@"Textures\SpriteSheet");
            menuBackground = Content.Load<Texture2D>(@"Textures\MenuBackground");

            // Load fonts for game
            menuItemFont = Content.Load<SpriteFont>(@"Fonts\MenuItem");

            // Camera initialization
            Camera.ViewPortWidth = this.graphics.PreferredBackBufferWidth;
            Camera.ViewPortHeight = this.graphics.PreferredBackBufferHeight;

            // WeaponManager initialization
            WeaponManager.Texture = spriteSheet;

            // SoundManager initialization
            SoundManager.Initialize(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            GamePadState padState = GamePad.GetState(PlayerIndex.One);

            // handle menu iteration
            menuTimer++;
            if (gameState == GameStates.TitleMenu || gameState == GameStates.OptionsMenu || gameState == GameStates.ControlsMenu
                || gameState == GameStates.GameModesMenu || gameState == GameStates.PauseMenu || gameState == GameStates.GameOverMenu)
            {
                if (menuTimer >= menuMinTimer)
                {
                    if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S) || padState.ThumbSticks.Left.Y < -0.3)
                    {
                        menuTimer = 0f;
                        menu.Iterator++;
                    }
                    else if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W) || padState.ThumbSticks.Left.Y > 0.3)
                    {
                        menuTimer = 0f;
                        menu.Iterator--;
                    }
                }
            }

            switch (gameState)
            {
                case GameStates.TitleMenu:
                    if (keyState.IsKeyDown(Keys.Escape) || padState.IsButtonDown(Buttons.Back))
                    {
                        if (menuTimer >= menuMinTimer)
                        {
                            this.Exit();
                        }
                    }
                    else if (keyState.IsKeyDown(Keys.Space) || keyState.IsKeyDown(Keys.Enter) || padState.IsButtonDown(Buttons.A))
                    {
                        if (menuTimer >= menuMinTimer)
                        {
                            menuTimer = 0f;
                            switch (menu.Iterator)
                            {
                                case 0:
                                    gameState = GameStates.GameModesMenu;
                                    menu = new Menus.GameModesMenu();
                                    break;
                                case 1:
                                    gameState = GameStates.OptionsMenu;
                                    menu = new Menus.OptionsMenu();
                                    break;
                                case 2:
                                    this.Exit();
                                    break;
                            }
                        }
                    }
                    break;

                case GameStates.OptionsMenu:
                    if (keyState.IsKeyDown(Keys.Escape) || padState.IsButtonDown(Buttons.Back))
                    {
                        if (menuTimer >= menuMinTimer)
                        {
                            menuTimer = 0f;
                            gameState = GameStates.TitleMenu;
                            menu = new Menus.TitleMenu();
                        }
                    }
                    else if (keyState.IsKeyDown(Keys.Space) || keyState.IsKeyDown(Keys.Enter) || padState.IsButtonDown(Buttons.A))
                    {
                        if (menuTimer >= menuMinTimer)
                        {
                            menuTimer = 0f;
                            switch (menu.Iterator)
                            {
                                case 0:
                                    gameState = GameStates.ControlsMenu;
                                    menu = new Menus.ControlsMenu(1);
                                    break;
                                case 1:
                                    gameState = GameStates.ControlsMenu;
                                    menu = new Menus.ControlsMenu(2);
                                    break;
                                case 2:
                                    if (SoundManager.MusicPlaying)
                                    {
                                        SoundManager.PauseBackgroundMusic();
                                    }
                                    else
                                    {
                                        SoundManager.StartBackgroundMusic();
                                    }
                                    break;
                                case 3:
                                    gameState = GameStates.TitleMenu;
                                    menu = new Menus.TitleMenu();
                                    break;
                            }
                        }
                    }
                    break;

                case GameStates.ControlsMenu:
                    if (keyState.IsKeyDown(Keys.Escape) || padState.IsButtonDown(Buttons.Back))
                    {
                        if (menuTimer >= menuMinTimer)
                        {
                            menuTimer = 0f;
                            gameState = GameStates.OptionsMenu;
                            menu = new Menus.OptionsMenu();
                        }
                    }
                    else if (keyState.IsKeyDown(Keys.Space) || keyState.IsKeyDown(Keys.Enter) || padState.IsButtonDown(Buttons.A))
                    {
                        if (menuTimer >= menuMinTimer)
                        {
                            menuTimer = 0f;
                            switch (menu.Iterator)
                            {
                                case 0:
                                    if (menu.PlayerIndex == 1)
                                    {
                                        if (player2Controls == Player.Controls.Computer)
                                        {
                                            player2Controls = player1Controls;
                                        }
                                        player1Controls = Player.Controls.Computer;
                                    }
                                    else
                                    {
                                        if (player1Controls == Player.Controls.Computer)
                                        {
                                            player1Controls = player2Controls;
                                        }
                                        player2Controls = Player.Controls.Computer;
                                    }
                                    gameState = GameStates.TitleMenu;
                                    menu = new Menus.TitleMenu();
                                    break;
                                case 1:
                                    if (menu.PlayerIndex == 1)
                                    {
                                        if (player2Controls == Player.Controls.Xbox1)
                                        {
                                            player2Controls = player1Controls;
                                        }
                                        player1Controls = Player.Controls.Xbox1;
                                    }
                                    else
                                    {
                                        if (player1Controls == Player.Controls.Xbox1)
                                        {
                                            player1Controls = player2Controls;
                                        }
                                        player2Controls = Player.Controls.Xbox1;
                                    }
                                    gameState = GameStates.TitleMenu;
                                    menu = new Menus.TitleMenu();
                                    break;
                                case 2:
                                    if (menu.PlayerIndex == 1)
                                    {
                                        if (player2Controls == Player.Controls.Xbox2)
                                        {
                                            player2Controls = player1Controls;
                                        }
                                        player1Controls = Player.Controls.Xbox2;
                                    }
                                    else
                                    {
                                        if (player1Controls == Player.Controls.Computer)
                                        {
                                            player1Controls = player2Controls;
                                        }
                                        player2Controls = Player.Controls.Xbox2;
                                    }
                                    gameState = GameStates.TitleMenu;
                                    menu = new Menus.TitleMenu();
                                    break;
                                case 3:
                                    gameState = GameStates.TitleMenu;
                                    menu = new Menus.TitleMenu();
                                    break;
                            }
                        }
                    }
                    break;

                case GameStates.GameModesMenu:
                    if (keyState.IsKeyDown(Keys.Escape) || padState.IsButtonDown(Buttons.Back))
                    {
                        if (menuTimer >= menuMinTimer)
                        {
                            menuTimer = 0f;
                            gameState = GameStates.TitleMenu;
                            menu = new Menus.TitleMenu();
                        }
                    }
                    else if (keyState.IsKeyDown(Keys.Space) || keyState.IsKeyDown(Keys.Enter) || padState.IsButtonDown(Buttons.A))
                    {
                        if (menuTimer >= menuMinTimer)
                        {
                            menuTimer = 0f;
                            switch (menu.Iterator)
                            {
                                case 0:
                                    gameState = GameStates.Standard;
                                    Camera.WorldRectangle = new Rectangle(0, 0, this.graphics.PreferredBackBufferWidth, this.graphics.PreferredBackBufferHeight);
                                    Camera.Reset();
                                    TileMap.Initialize(spriteSheet, 17, 13);
                                    WeaponManager.ClearShotList();
                                    PlayerManager.ClearPlayerList();
                                    PlayerManager.AddPlayer(new Player(spriteSheet, new Vector2(120, 120), "blue", player1Controls));
                                    PlayerManager.NeededPlayers = 1;
                                    EnemyManager.Initialize(spriteSheet, 2);
                                    break;
                                case 1:
                                    gameState = GameStates.Duel;
                                    Camera.WorldRectangle = new Rectangle(0, 0, this.graphics.PreferredBackBufferWidth, this.graphics.PreferredBackBufferHeight);
                                    Camera.Reset();
                                    TileMap.Initialize(spriteSheet, 17, 13);
                                    WeaponManager.ClearShotList();
                                    PlayerManager.ClearPlayerList();
                                    PlayerManager.AddPlayer(new Player(spriteSheet, new Vector2(120, 120), "blue", player1Controls));
                                    PlayerManager.AddPlayer(new Player(spriteSheet, new Vector2(696, 504), "red", player2Controls));
                                    PlayerManager.NeededPlayers = 2;
                                    break;
                                case 2:
                                    gameState = GameStates.Coop;
                                    Camera.WorldRectangle = new Rectangle(0, 0, this.graphics.PreferredBackBufferWidth, this.graphics.PreferredBackBufferHeight);
                                    Camera.Reset();
                                    TileMap.Initialize(spriteSheet, 17, 13);
                                    WeaponManager.ClearShotList();
                                    PlayerManager.ClearPlayerList();
                                    PlayerManager.AddPlayer(new Player(spriteSheet, new Vector2(120, 120), "blue", player1Controls));
                                    PlayerManager.AddPlayer(new Player(spriteSheet, new Vector2(696, 504), "red", player2Controls));
                                    PlayerManager.NeededPlayers = 1;
                                    EnemyManager.Initialize(spriteSheet, 3);
                                    break;
                                case 3:
                                    gameState = GameStates.Adventure;
                                    Camera.WorldRectangle = new Rectangle(0, 0, 1920, 1920); // camera initialization
                                    Camera.Reset();
                                    TileMap.Initialize(spriteSheet, 40, 40); // TileMap initialization
                                    WeaponManager.ClearShotList();
                                    PlayerManager.ClearPlayerList();
                                    PlayerManager.AddPlayer(new Player(spriteSheet, new Vector2(120, 120), "blue", player1Controls));
                                    PlayerManager.NeededPlayers = 1;
                                    EnemyManager.Initialize(spriteSheet, 5);
                                    break;
                                case 4:
                                    gameState = GameStates.TitleMenu;
                                    menu = new Menus.TitleMenu();
                                    break;
                            }
                        }
                    }
                    break;

                case GameStates.PauseMenu:
                    if (keyState.IsKeyDown(Keys.Escape) || keyState.IsKeyDown(Keys.P) || padState.IsButtonDown(Buttons.Back))
                    {
                        if (menuTimer >= menuMinTimer)
                        {
                            menuTimer = 0f;
                            gameState = prevState;
                        }
                    }
                    else if (keyState.IsKeyDown(Keys.Space) || keyState.IsKeyDown(Keys.Enter) || padState.IsButtonDown(Buttons.A))
                    {
                        if (menuTimer >= menuMinTimer)
                        {
                            menuTimer = 0f;
                            switch (menu.Iterator)
                            {
                                case 0:
                                    gameState = prevState;
                                    break;
                                case 1:
                                    gameState = GameStates.TitleMenu;
                                    menu = new Menus.TitleMenu();
                                    break;
                                case 2:
                                    this.Exit();
                                    break;
                            }
                        }
                    }
                    break;

                case GameStates.GameOverMenu:
                    if (keyState.IsKeyDown(Keys.Escape) || padState.IsButtonDown(Buttons.Back))
                    {
                        if (menuTimer >= menuMinTimer)
                        {
                            menuTimer = 0f;
                            gameState = GameStates.TitleMenu;
                            menu = new Menus.TitleMenu();
                        }
                    }
                    else if (keyState.IsKeyDown(Keys.Space) || keyState.IsKeyDown(Keys.Enter) || padState.IsButtonDown(Buttons.A))
                    {
                        if (menuTimer >= menuMinTimer)
                        {
                            menuTimer = 0f;
                            switch (menu.Iterator)
                            {
                                case 0:
                                    gameState = GameStates.TitleMenu;
                                    menu = new Menus.TitleMenu();
                                    break;
                                case 1:
                                    this.Exit();
                                    break;
                            }
                        }
                    }
                    break;

                case GameStates.Standard:
                case GameStates.Duel:
                case GameStates.Adventure:
                case GameStates.Coop:
                    EnemyManager.Update(gameTime);
                    PlayerManager.Update(gameTime);
                    WeaponManager.Update(gameTime);

                    if (PlayerManager.GameFinished || ((gameState != GameStates.Duel) && !EnemyManager.EnemiesLeft))
                    {
                        gameOverTimer++;
                        if (gameOverTimer > gameOverMinTimer)
                        {
                            gameOverTimer = 0f;
                            string gameOverTitle;
                            if (gameState == GameStates.Duel)
                            {
                                if (PlayerManager.NumPlayers == 0)
                                    gameOverTitle = "Tie!";
                                else if (PlayerManager.GetPlayer(0).TankColor.Equals("blue"))
                                    gameOverTitle = "Blue Wins!";
                                else
                                    gameOverTitle = "Red Wins!";

                            }
                            else if (!EnemyManager.EnemiesLeft)
                            {
                                gameOverTitle = "You Won!";
                            }
                            else
                            {
                                gameOverTitle = "You Lost!";
                            }
                            menuTimer = 0f;
                            gameState = GameStates.GameOverMenu;
                            menu = new Menus.GameOverMenu(gameOverTitle);
                        }
                    }
                    else
                    {
                        if (keyState.IsKeyDown(Keys.P) || keyState.IsKeyDown(Keys.Escape) || padState.IsButtonDown(Buttons.Back) || padState.IsButtonDown(Buttons.Start))
                        {
                            if (menuTimer >= menuMinTimer)
                            {
                                menuTimer = 0f;
                                prevState = gameState;
                                gameState = GameStates.PauseMenu;
                                menu = new Menus.PauseMenu();
                            }
                        }
                    }
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            switch (gameState)
            {
                case GameStates.TitleMenu:
                case GameStates.OptionsMenu:
                case GameStates.ControlsMenu:
                case GameStates.GameModesMenu:
                case GameStates.PauseMenu:
                case GameStates.GameOverMenu:
                    menu.DrawMenu(spriteBatch, menuBackground, this.graphics.PreferredBackBufferWidth, this.graphics.PreferredBackBufferHeight, menuItemFont);
                    break;

                case GameStates.Standard:
                case GameStates.Duel:
                case GameStates.Adventure:
                case GameStates.Coop:
                    TileMap.Draw(spriteBatch);
                    EnemyManager.Draw(spriteBatch);
                    PlayerManager.Draw(spriteBatch);
                    WeaponManager.Draw(spriteBatch);

                    /*// Temporary Code Begin
                    Vector2 mouseLocation = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                    mouseLocation += Camera.Position;
                    mouseLocation.X = MathHelper.Clamp(mouseLocation.X, Camera.Position.X, Camera.Position.X + Camera.ViewPortWidth);
                    mouseLocation.Y = MathHelper.Clamp(mouseLocation.Y, Camera.Position.Y, Camera.Position.Y + Camera.ViewPortHeight);

                    if (PlayerManager.NumPlayers > 0)
                    {
                        List<Vector2> path = Astar.PathFinder.FindPath(TileMap.GetSquareAtPixel(mouseLocation), TileMap.GetSquareAtPixel(PlayerManager.GetPlayer(0).BaseSprite.WorldLocation));
                        if (path != null)
                        {
                            foreach (Vector2 node in path)
                            {
                                spriteBatch.Draw(spriteSheet, TileMap.SquareScreenRectangle((int)node.X, (int)node.Y), new Rectangle(0, 0, 48, 48), new Color(128, 0, 0, 80));
                            }
                        }
                    }
                    // Temporary Code End*/

                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}