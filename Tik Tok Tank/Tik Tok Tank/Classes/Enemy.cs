using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tik_Tok_Tank
{
    class Enemy
    {
        #region Declarations

        public Sprite EnemyBase;
        public Sprite EnemyTurret;

        public float EnemySpeed = 60f;
        public Vector2 currentTargetSquare;

        Vector2 turretOffset = new Vector2(-6, 0);
        Vector2 turretEndOffset = new Vector2(60, 0);
        Vector2 turretOrigin;

        private float shotTimer = 0f;
        private static float shotMinTimer = 2f;

        public bool Destroyed = false;

        private int playerToTrack;
        private Player playerTrack;

        #endregion

        public Enemy(
            Texture2D texture,
            Rectangle baseFrame,
            Rectangle turretFrame,
            Vector2 worldLocation,
            int PlayerTrack)
        {
            EnemyBase = new Sprite(
                texture,
                baseFrame,
                new Vector2(baseFrame.Width / 2, baseFrame.Height / 2),
                worldLocation,
                Vector2.UnitX,
                0.0f,
                true);
            EnemyBase.Scale = 0.75f;
            EnemyBase.Speed = EnemySpeed;
            EnemyBase.AnimateWhenStopped = false; // prevent from animating when stopped
            for (int x = 1; x < 6; x++) // add all animation frames for base
            {
                EnemyBase.AddFrame(
                    new Rectangle(baseFrame.X + (baseFrame.Width * x),
                        baseFrame.Y,
                        baseFrame.Width, baseFrame.Height));
            }

            turretOrigin = new Vector2(18, baseFrame.Height / 2);
            EnemyTurret = new Sprite(
                texture,
                turretFrame,
                turretOrigin,
                worldLocation,
                Vector2.UnitX,
                0.0f);
            EnemyTurret.Scale = 0.75f;
            EnemyTurret.Animate = false;

            playerToTrack = PlayerTrack;
        }

        #region Public Methods

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            shotTimer += elapsed;

            playerToTrack = (int)MathHelper.Clamp(playerToTrack, 0, PlayerManager.NumPlayers - 1);
            if (playerToTrack < PlayerManager.NumPlayers)
            {
                playerTrack = PlayerManager.GetPlayer(playerToTrack);
                if (!Destroyed)
                {
                    if (Vector2.Distance(EnemyBase.WorldLocation, playerTrack.BaseSprite.WorldLocation) > 60 &&
                        Vector2.Distance(EnemyBase.WorldLocation, playerTrack.BaseSprite.WorldLocation) < 700)
                    {
                        EnemyBase.Heading = determineMoveDirection();
                        EnemyBase.RotateTo(EnemyBase.Heading);
                        EnemyBase.Update(gameTime);
                    }

                    Vector2 newTurretPosition = Vector2.Transform(turretOffset, EnemyBase.TransformationMatrix);
                    EnemyTurret.WorldLocation = newTurretPosition;

                    Vector2 directionToPlayer = playerTrack.BaseSprite.WorldLocation - EnemyBase.WorldLocation;
                    EnemyTurret.Heading = directionToPlayer;
                    EnemyTurret.RotateTo(directionToPlayer);

                    Vector2 shotLocation = Vector2.Transform(turretEndOffset, EnemyTurret.TransformationMatrix);
                    if (!TileMap.IsWallTileByPixel(shotLocation))
                    {
                        FireWeapon(shotLocation, EnemyTurret.Heading, WeaponManager.WeaponSpeed);
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Destroyed)
            {
                EnemyBase.Draw(spriteBatch);
                EnemyTurret.Draw(spriteBatch);
            }
        }

        #endregion

        #region AI Methods

        private Vector2 determineMoveDirection()
        {
            if (reachedTargetSquare())
            {
                currentTargetSquare = getNewTargetSquare();
            }

            Vector2 squareCenter = TileMap.GetSquareCenter(currentTargetSquare);
            return squareCenter - EnemyBase.WorldLocation;
        }

        private bool reachedTargetSquare()
        {
            return (Vector2.Distance(EnemyBase.WorldLocation, TileMap.GetSquareCenter(currentTargetSquare)) <= 2);
        }

        private Vector2 getNewTargetSquare()
        {
            List<Vector2> path = Astar.PathFinder.FindPath(
                TileMap.GetSquareAtPixel(EnemyBase.WorldLocation), TileMap.GetSquareAtPixel(playerTrack.BaseSprite.WorldLocation));

            if (path.Count > 1)
            {
                return new Vector2(path[1].X, path[1].Y);
            }
            else
            {
                return TileMap.GetSquareAtPixel(playerTrack.BaseSprite.WorldLocation);
            }
        }

        private void FireWeapon(Vector2 worldLocation, Vector2 heading, float speed)
        {
            float actualDistance = Vector2.Distance(EnemyBase.WorldLocation, playerTrack.BaseSprite.WorldLocation);
            if (actualDistance < 600)
            {
                List<Vector2> pathToPlayer = Astar.PathFinder.FindPath(
                    TileMap.GetSquareAtPixel(EnemyBase.WorldLocation), TileMap.GetSquareAtPixel(playerTrack.BaseSprite.WorldLocation));

                float roughPathDistance = (pathToPlayer.Count * TileMap.TileWidth);
                if (CanFireWeapon() && (roughPathDistance <= actualDistance + 30))
                {
                    shotTimer = 0f;
                    WeaponManager.AddShot(worldLocation, heading, speed, 0);
                }
            }
        }

        private bool CanFireWeapon()
        {
            return (shotTimer > shotMinTimer);
        }

        #endregion
    }
}
