using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tik_Tok_Tank
{
    static class EnemyManager
    {
        #region Declarations

        public static List<Enemy> Enemies = new List<Enemy>();

        private static List<Rectangle> spriteRectangles = new List<Rectangle>();
        
        public static Texture2D enemyTexture;
        //private static int minDistanceFromPlayer = 100;

        public const int SpriteWidth = 48;
        public const int SpriteHeight = 48;

        public const int BaseRectangleStart = 0;
        public const int BaseRectangleEnd = 2;
        public const int TurretRectangleStart = 3;
        public const int TurretRectangleEnd = 5;

        private static Random rand = new Random();

        #endregion

        #region Initialization

        public static void Initialize(Texture2D texture, int numEnemies)
        {
            enemyTexture = texture;
            spriteRectangles.Clear(); // clear current rectangle list

            // add enemy bases to rectangle list
            spriteRectangles.Add(new Rectangle(0, 288, SpriteWidth, SpriteHeight));
            spriteRectangles.Add(new Rectangle(0, 336, SpriteWidth, SpriteHeight));
            spriteRectangles.Add(new Rectangle(0, 384, SpriteWidth, SpriteHeight));
            // add enemy turrets to rectangle list
            spriteRectangles.Add(new Rectangle(96, 144, SpriteWidth, SpriteHeight));
            spriteRectangles.Add(new Rectangle(144, 144, SpriteWidth, SpriteHeight));
            spriteRectangles.Add(new Rectangle(192, 144, SpriteWidth, SpriteHeight));

            Enemies.Clear();
            for (int i = 0; i < numEnemies; i++)
            {
                AddEnemy();
            }
        }

        #endregion

        #region Enemy Management

        public static void AddEnemy()
        {
            int enemyBaseIndex = rand.Next(BaseRectangleStart, BaseRectangleEnd + 1);
            Rectangle enemyBaseInitialFrame = spriteRectangles[enemyBaseIndex];
            Rectangle enemyTurretFrame = spriteRectangles[enemyBaseIndex + 3];
            
            int playerToTrack = rand.Next(0, PlayerManager.NumPlayers);

            if (playerToTrack < PlayerManager.NumPlayers)
            {
                Vector2 squareLocation = FindEmptySpotForEnemy(PlayerManager.GetPlayer(playerToTrack));

                if (squareLocation != new Vector2(-1, -1))
                {
                    Enemy newEnemy = new Enemy(
                        enemyTexture,
                        enemyBaseInitialFrame,
                        enemyTurretFrame,
                        TileMap.GetSquareCenter(squareLocation),
                        playerToTrack);
                    newEnemy.currentTargetSquare = squareLocation;
                    Enemies.Add(newEnemy);
                }
            }
        }

        public static Vector2 FindEmptySpotForEnemy(Player playerTrack)
        {
            int tries = 0;
            int maxTries = 10;

            int startX = rand.Next(2, TileMap.MapWidth - 2);
            int startY = rand.Next(2, TileMap.MapHeight - 2);

            List<Vector2> path = Astar.PathFinder.FindPath(new Vector2(startX, startY), TileMap.GetSquareAtPixel(playerTrack.BaseSprite.WorldLocation));
            while ((path == null) && (tries < maxTries))
            {
                startX = rand.Next(2, TileMap.MapWidth - 2);
                startY = rand.Next(2, TileMap.MapHeight - 2);
                path = Astar.PathFinder.FindPath(new Vector2(startX, startY), TileMap.GetSquareAtPixel(playerTrack.BaseSprite.WorldLocation));
                tries++;
            }

            if (path == null)
            {
                return new Vector2(-1, -1);
            }
            else
            {
                return new Vector2(startX, startY);
            }
        }

        public static bool EnemiesLeft
        {
            get { return (Enemies.Count > 0); }
        }

        #endregion

        #region Update and Draw

        public static void Update(GameTime gameTime)
        {
            for (int i = Enemies.Count - 1; i >= 0; i--)
            {
                Enemies[i].Update(gameTime);
                if (Enemies[i].Destroyed)
                {
                    Enemies.RemoveAt(i);
                }
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (Enemy enemy in Enemies)
            {
                enemy.Draw(spriteBatch);
            }
        }

        #endregion
    }
}
