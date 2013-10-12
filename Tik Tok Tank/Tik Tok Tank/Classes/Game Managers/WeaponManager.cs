using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tik_Tok_Tank
{
    /// <summary>
    /// Handles all shots fired in the game
    /// </summary>
    static class WeaponManager
    {
        #region Declarations

        public static List<Particle> Shots = new List<Particle>();
        public static Texture2D Texture;
        public static Rectangle shotRectangle = new Rectangle(0, 432, 48, 48);
        public static float WeaponSpeed = 180f;

        private static float collisionCheckDistance = 40f;

        public static Random rand = new Random(); // temporary

        #endregion

        #region General Methods

        public static void ClearShotList()
        {
            Shots.Clear();
        }

        #endregion

        #region Effects Management Methods

        /// <summary>
        /// Add a shot to the list of updatable particles
        /// </summary>
        /// <param name="worldLocation">Location of shot to create</param>
        /// <param name="heading">Heading of shot to create</param>
        /// <param name="speed">Speed of shot to create</param>
        /// <param name="frame">Frame of shot row in stylesheet to add</param>
        public static void AddShot(
            Vector2 worldLocation,
            Vector2 heading,
            float speed,
            int frame)
        {
            Particle shot = new Particle(
                Texture,
                shotRectangle,
                worldLocation,
                heading,
                speed,
                Vector2.Zero,
                400f,
                300,
                Color.White,
                Color.White,
                true);

            shot.AddFrame(new Rectangle(
                shotRectangle.X + shotRectangle.Width,
                shotRectangle.Y,
                shotRectangle.Width, shotRectangle.Height));

            shot.Animate = false;
            shot.CurrentFrame = frame;
            shot.RotateTo(heading);
            shot.circleCollideRadius = 4;

            Shots.Add(shot);
        }

        #endregion

        #region Collision Detection

        /// <summary>
        /// Rebounds a given shot off a wall if it will hit it in the update
        /// </summary>
        /// <param name="gameTime">Timing values for current update cycle</param>
        /// <param name="shot">Particle to check against walls</param>
        private static void checkShotWallCollision(GameTime gameTime, Particle shot)
        {
            if (shot.onWallTile() != new Vector2(-1, -1)) // remove shot if on wall tile
            {
                shot.Expired = true;
            }
            
            if (shot.Expired)
            {
                return;
            }

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 newWorldLocation = shot.WorldLocation + (shot.Velocity * elapsed);

            // reflect shot off wall if it is about to get there
            Vector2 futureWallTile = shot.onWallTile(shot.Scale, shot.Rotation, newWorldLocation);
            Vector2 shotSquare = TileMap.GetSquareAtPixel(shot.WorldLocation);

            if (futureWallTile != new Vector2(-1, -1))
            {
                if (TileMap.IsBreakWallTile(futureWallTile))
                {
                    shot.Expired = true;
                    TileMap.SetTileAtSquare((int)futureWallTile.X, (int)futureWallTile.Y, TileMap.currentFloorTile);
                }
                else
                {
                    if ((futureWallTile.X < shotSquare.X) || (futureWallTile.X > shotSquare.X))
                    {
                        shot.Heading = new Vector2(-shot.Heading.X, shot.Heading.Y);
                    }
                    else if ((futureWallTile.Y < shotSquare.Y) || (futureWallTile.Y > shotSquare.Y))
                    {
                        shot.Heading = new Vector2(shot.Heading.X, -shot.Heading.Y);
                    }
                    shot.Speed = (shot.Speed * 2 / 3);
                }
            }
        }

        /// <summary>
        /// Checks to see if a shot would hit either player.  If so, removes player.
        /// </summary>
        /// <param name="shot">Shot to check collision of</param>
        private static void checkShotPlayerCollisions(Particle shot)
        {
            if (shot.Expired)
            {
                return;
            }

            for (int i = PlayerManager.NumPlayers - 1; i >= 0; i--)
            {
                Player player = PlayerManager.GetPlayer(i);

                if (Vector2.Distance(shot.WorldLocation, player.BaseSprite.WorldLocation) < collisionCheckDistance)
                {
                    foreach (Vector2 tankPixel in player.BaseSprite.pixelLocations)
                    {
                        Vector2 pixelScreenLocation = Vector2.Transform(tankPixel, player.BaseSprite.TransformationMatrix);
                        if (shot.spriteCircleCollidesWith(pixelScreenLocation))
                        {
                            shot.Expired = true;
                            //player.BaseSprite.TintColor = new Color(rand.Next(255), rand.Next(255), rand.Next(255));
                            PlayerManager.RemovePlayer(i);
                            break;
                        }
                    }
                }
            }
        }

        private static void checkShotEnemyCollisions(Particle shot)
        {
            if (shot.Expired)
            {
                return;
            }

            foreach (Enemy enemy in EnemyManager.Enemies)
            {
                if (Vector2.Distance(shot.WorldLocation, enemy.EnemyBase.WorldLocation) < collisionCheckDistance)
                {
                    foreach (Vector2 tankPixel in enemy.EnemyBase.pixelLocations)
                    {
                        Vector2 pixelScreenLocation = Vector2.Transform(tankPixel, enemy.EnemyBase.TransformationMatrix);
                        if (shot.spriteCircleCollidesWith(pixelScreenLocation))
                        {
                            shot.Expired = true;
                            enemy.Destroyed = true;
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #region Update and Draw

        /// <summary>
        /// Run the update method of each non-expired shot
        /// </summary>
        /// <param name="gameTime">Timing values for update cycle</param>
        public static void Update(GameTime gameTime)
        {
            for(int i = Shots.Count - 1; i >= 0; i--)
            {
                Shots[i].Update(gameTime);
                checkShotWallCollision(gameTime, Shots[i]);
                checkShotPlayerCollisions(Shots[i]);
                checkShotEnemyCollisions(Shots[i]);

                if (Shots[i].Expired)
                {
                    Shots.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Draw every non-expired shot to the screen
        /// </summary>
        /// <param name="spriteBatch">Allows computer to draw particle to screen</param>
        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (Particle sprite in Shots)
            {
                sprite.Draw(spriteBatch);
            }
        }

        #endregion
    }
}
