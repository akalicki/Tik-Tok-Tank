using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tik_Tok_Tank
{
    /// <summary>
    /// Handles drawing and updating the tank
    /// </summary>
    class Player
    {
        #region Declarations

        // control scheme
        public enum Controls { Computer, Xbox1, Xbox2 };
        Controls playerControls;

        // sprites representing base, gun turret, and crosshair
        public Sprite BaseSprite;
        public Sprite TurretSprite;
        public Sprite CrosshairSprite;

        // initial frame of each sprite, number of base animation frames
        private static int baseFrameCount = 6;
        private static Rectangle blueBaseInitialFrame = new Rectangle(0, 192, 48, 48);
        private static Rectangle redBaseInitialFrame = new Rectangle(0, 240, 48, 48);

        private static Rectangle blueTurretFrame = new Rectangle(0, 144, 48, 48);
        private static Rectangle redTurretFrame = new Rectangle(48, 144, 48, 48);

        private static Rectangle blueCrossFrame = new Rectangle(0, 480, 48, 48);
        private static Rectangle redCrossFrame = new Rectangle(48, 480, 48, 48);

        // origin of tank at center, turret at rotation point, crosshair
        private static int baseWidth = blueBaseInitialFrame.Width;
        private static int baseHeight = blueBaseInitialFrame.Height;

        private static Vector2 baseOrigin = new Vector2(baseWidth / 2, baseHeight / 2);
        private static Vector2 turretOrigin = new Vector2(18, baseHeight / 2);
        private static Vector2 crosshairOrigin = new Vector2(baseWidth / 2, baseHeight / 2);

        // offset of turret origin from tank origin
        public static Vector2 turretOffset = new Vector2(-6, 0);

        // offset of turret end from turret origin
        public static Vector2 turretEndOffset = new Vector2(35, 0);

        // rectangle defining area in which camera will keep player
        public static Rectangle scrollArea = new Rectangle(175, 150, 450, 300);

        // list containing pixel locations of tank for collisions with wall tiles
        private List<Vector2> basePixelLocations = new List<Vector2>();

        // movement constants
        private static float playerBackMaxSpeed = 100f;
        private static float playerForwardMaxSpeed = 150f;

        private static float playerAcceleration = 10f;
        private static float friction = 5f;

        private static float forwardRotateSpeed = .05f;
        private static float backwardRotateSpeed = .03f;
        private static float staticRotateSpeed = .04f;

        private static float crosshairSpeed = 8f;

        private float shotTimer = 0f;
        private static float shotMinTimer = .2f;

        public string TankColor;

        #endregion

        #region Constructor

        public Player(
            Texture2D texture,
            Vector2 worldLocation,
            string tankColor,
            Controls controls)
        {
            TankColor = tankColor;
            if (tankColor == "blue")
            {
                // create new sprite for tank base
                BaseSprite = new Sprite(
                    texture,
                    blueBaseInitialFrame,
                    baseOrigin,
                    worldLocation,
                    Vector2.UnitX,
                    0.0f,
                    true);
                BaseSprite.Scale = 0.75f;
                BaseSprite.AnimateWhenStopped = false; // prevent from animating when stopped
                for (int x = 1; x < baseFrameCount; x++) // add all animation frames for base
                {
                    BaseSprite.AddFrame(
                        new Rectangle(blueBaseInitialFrame.X + (baseWidth * x),
                            blueBaseInitialFrame.Y,
                            baseWidth, baseHeight));
                }

                // create new sprite for turret
                TurretSprite = new Sprite(
                    texture,
                    blueTurretFrame,
                    turretOrigin,
                    worldLocation + turretOffset,
                    Vector2.UnitX,
                    0.0f);
                TurretSprite.Scale = 0.75f;
                TurretSprite.Animate = false; // prevent it from animating

                // create new sprite for crosshair
                CrosshairSprite = new Sprite(
                    texture,
                    blueCrossFrame,
                    crosshairOrigin,
                    new Vector2(400, 400),
                    Vector2.UnitX,
                    0.0f);
                CrosshairSprite.Animate = false; // prevent it from animating
            }

            else if (tankColor == "red")
            {
                // create new sprite for tank base
                BaseSprite = new Sprite(
                    texture,
                    redBaseInitialFrame,
                    baseOrigin,
                    worldLocation,
                    Vector2.UnitX,
                    0.0f,
                    true);
                BaseSprite.Scale = 0.75f;
                BaseSprite.AnimateWhenStopped = false; // prevent from animating when stopped
                for (int x = 1; x < baseFrameCount; x++) // add all animation frames for base
                {
                    BaseSprite.AddFrame(
                        new Rectangle(redBaseInitialFrame.X + (baseWidth * x),
                            redBaseInitialFrame.Y,
                            baseWidth, baseHeight));
                }

                // create new sprite for turret
                TurretSprite = new Sprite(
                    texture,
                    redTurretFrame,
                    turretOrigin,
                    worldLocation + turretOffset,
                    Vector2.UnitX,
                    0.0f);
                TurretSprite.Scale = 0.75f;
                TurretSprite.Animate = false; // prevent it from animating

                // create new sprite for crosshair
                CrosshairSprite = new Sprite(
                    texture,
                    redCrossFrame,
                    crosshairOrigin,
                    new Vector2(400, 400),
                    Vector2.UnitX,
                    0.0f);
                CrosshairSprite.Animate = false; // prevent it from animating
            }

            playerControls = controls;
        }

        #endregion

        #region Input Handling

        /// <summary>
        /// Handle all movement input for tank from keyboard and gamepad.
        /// </summary>
        /// <param name="gameTime">Used to calculate how much time has passed.</param>
        /// <param name="keyState">Keyboard state for current update cycle</param>
        private void handleMovementInput(GameTime gameTime)
        {
            if (playerControls == Controls.Computer)
            {
                KeyboardState keyState = Keyboard.GetState();
                handleKeyboardRotation(keyState);
                handleKeyboardThrust(gameTime, keyState);
            }
            else if (playerControls == Controls.Xbox1)
            {
                GamePadState padState = GamePad.GetState(PlayerIndex.One);
                if (padState.IsConnected)
                {
                    handleGamePadRotation(padState);
                    handleGamePadThrust(gameTime, padState);
                }
            }
            else
            {
                GamePadState padState = GamePad.GetState(PlayerIndex.Two);
                if (padState.IsConnected)
                {
                    handleGamePadRotation(padState);
                    handleGamePadThrust(gameTime, padState);
                }
            }
        }
        
        /// <summary>
        /// Handle all turret input for tank from keyboard and gamepad.
        /// </summary>
        /// <param name="gameTime">Used to calculate how much time has passed.</param>
        /// <param name="mouseState">Mouse state for current update cycle</param>
        private void handleTurretInput(GameTime gameTime)
        {
            Vector2 newTurretPosition = Vector2.Transform(turretOffset, BaseSprite.TransformationMatrix);
            TurretSprite.WorldLocation = newTurretPosition;

            if (playerControls == Controls.Computer)
            {
                MouseState mouseState = Mouse.GetState();
                handleMouseCrosshairLocation(mouseState);
                handleTurretRotation();
                handleMouseWeapon(mouseState);
            }
            else if (playerControls == Controls.Xbox1)
            {
                GamePadState padState = GamePad.GetState(PlayerIndex.One);
                if (padState.IsConnected)
                {
                    handleGamePadCrosshairLocation(padState);
                    handleTurretRotation();
                    handleGamePadWeapon(padState);
                }
            }
            else
            {
                GamePadState padState = GamePad.GetState(PlayerIndex.Two);
                if (padState.IsConnected)
                {
                    handleGamePadCrosshairLocation(padState);
                    handleTurretRotation();
                    handleGamePadWeapon(padState);
                }
            }
       }

        /// <summary>
        /// Change turret's rotation to follow the mouse
        /// </summary>
        /// <param name="mouseState">State of mouse for current update cycle</param>
        private void handleTurretRotation()
        {
            Vector2 pointerDirection = CrosshairSprite.ScreenLocation - BaseSprite.ScreenLocation; // vector pointing from tank to crosshair

            TurretSprite.Heading = pointerDirection;
            TurretSprite.RotateTo(TurretSprite.Heading); // make turret rotate to face mouse
        }

        #region Computer Input Handling

        /// <summary>
        /// Rotate the tank if A or D keys are pressed, preventing it from rotating into walls.
        /// </summary>
        /// <param name="keyState">Keyboard state for current update cycle</param>
        private void handleKeyboardRotation(KeyboardState keyState)
        {
            // choose the correct rotate speed based on tank's current movement
            float rotateSpeed = staticRotateSpeed;
            if (BaseSprite.Speed > 0)
            {
                rotateSpeed = forwardRotateSpeed;
            }
            else if (BaseSprite.Speed < 0)
            {
                rotateSpeed = backwardRotateSpeed;
            }

            // calculate the heading of the tank after rotation
            Vector2 newHeading = BaseSprite.Heading;
            if (keyState.IsKeyDown(Keys.A))
            {
                newHeading += (BaseSprite.Normal * rotateSpeed);
            }
            if (keyState.IsKeyDown(Keys.D))
            {
                newHeading -= (BaseSprite.Normal * rotateSpeed);
            }

            // calculate new rotation of tank, and rotate it if there isn't a wall in the way
            float newRotation = (float)Math.Atan2(newHeading.Y, newHeading.X);
            if (BaseSprite.onWallTile(BaseSprite.Scale, newRotation, BaseSprite.WorldLocation) == new Vector2(-1, -1))
            {
                BaseSprite.Heading = newHeading;
                BaseSprite.RotateTo(BaseSprite.Heading);
            }
        }

        /// <summary>
        /// Handles forward/backward movement of tank, preventing it from running into walls.
        /// </summary>
        /// <param name="keyState">State of keyboard for current update cycle.</param>
        /// <param name="gameTime">Used to calculate how much time has passed, so how far to move.</param>
        private void handleKeyboardThrust(GameTime gameTime, KeyboardState keyState)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float appliedFriction = -Math.Sign(BaseSprite.Speed) * friction; // make sure friction points the right way
            float motorAcceleration = 0.0f;

            // get the correct acceleration based on which direction tank is moving
            if (keyState.IsKeyDown(Keys.W))
            {
                motorAcceleration += playerAcceleration;
            }

            if (keyState.IsKeyDown(Keys.S))
            {
                motorAcceleration -= playerAcceleration;
            }

            // if tank would hit a wall in this update cycle, set it's speed to negative 2/3 of current so bounces
            float newSpeed = BaseSprite.Speed + (motorAcceleration + appliedFriction);
            Vector2 newVelocity = newSpeed * BaseSprite.Heading;
            Vector2 newWorldLocation = BaseSprite.WorldLocation + (newVelocity * elapsed);
            if (BaseSprite.onWallTile(BaseSprite.Scale, BaseSprite.Rotation, newWorldLocation) != new Vector2(-1, -1))
            {
                BaseSprite.Speed = -(BaseSprite.Speed * 2 / 3);
            }
            // otherwise, update speed with new acceleration
            else
            {
                // if friction is causing it to jump between positive and negative speeds instead of go to zero
                if ((motorAcceleration == 0) && (Math.Sign(BaseSprite.Speed) != Math.Sign(newSpeed)))
                {
                    BaseSprite.Speed = 0f;
                }
                else
                {
                    BaseSprite.Speed = newSpeed;
                }
            }

            // make sure tank doesn't exceed speed limitations
            BaseSprite.Speed = MathHelper.Clamp(BaseSprite.Speed, -playerBackMaxSpeed, playerForwardMaxSpeed);
        }

        /// <summary>
        /// Set crosshair to mouse location, keeping it within the window
        /// </summary>
        /// <param name="mouseState">State of the mouse for the current update cycle</param>
        private void handleMouseCrosshairLocation(MouseState mouseState)
        {
            Vector2 pointerScreenLocation = new Vector2(mouseState.X, mouseState.Y);
            Vector2 pointerWorldLocation = Camera.TransformToWorld(pointerScreenLocation);

            pointerWorldLocation.X = MathHelper.Clamp(pointerWorldLocation.X, Camera.ViewPort.Left, Camera.ViewPort.Right);
            pointerWorldLocation.Y = MathHelper.Clamp(pointerWorldLocation.Y, Camera.ViewPort.Top, Camera.ViewPort.Bottom);

            CrosshairSprite.WorldLocation = pointerWorldLocation;
        }

        /// <summary>
        /// Fire a shot if mouse button down and timer high enough
        /// </summary>
        /// <param name="mouseState">Mouse state for current update cycle</param>
        private void handleMouseWeapon(MouseState mouseState)
        {
            Vector2 shotLocation = Vector2.Transform(turretEndOffset, TurretSprite.TransformationMatrix);

            if (CanFireWeapon && !TileMap.IsWallTileByPixel(shotLocation) && (mouseState.LeftButton == ButtonState.Pressed))
            {
                FireWeapon(shotLocation, TurretSprite.Heading, WeaponManager.WeaponSpeed);
            }
        }

        #endregion

        #region Gamepad Input Handling

        /// <summary>
        /// Rotate the tank if left control stick pressed, preventing it from rotating into walls.
        /// </summary>
        /// <param name="padState">Gamepad state for current update cycle</param>
        private void handleGamePadRotation(GamePadState padState)
        {
            // choose the correct rotate speed based on tank's current movement
            float rotateSpeed = staticRotateSpeed;
            if (BaseSprite.Speed > 0)
            {
                rotateSpeed = forwardRotateSpeed;
            }
            else if (BaseSprite.Speed < 0)
            {
                rotateSpeed = backwardRotateSpeed;
            }

            // calculate the heading of the tank after rotation, how fast it should rotate
            Vector2 newHeading = BaseSprite.Heading;
            newHeading -= (BaseSprite.Normal * rotateSpeed * padState.ThumbSticks.Left.X);

            // calculate new rotation of tank, and rotate it if there isn't a wall in the way
            float newRotation = (float)Math.Atan2(newHeading.Y, newHeading.X);
            if (BaseSprite.onWallTile(BaseSprite.Scale, newRotation, BaseSprite.WorldLocation) == new Vector2(-1, -1))
            {
                BaseSprite.Heading = newHeading;
                BaseSprite.RotateTo(BaseSprite.Heading);
            }

        }

        /// <summary>
        /// Handles forward/backward movement of tank, preventing it from running into walls.
        /// </summary>
        /// <param name="padState">State of gamepad for current update cycle.</param>
        /// <param name="gameTime">Used to calculate how much time has passed, so how far to move.</param>
        private void handleGamePadThrust(GameTime gameTime, GamePadState padState)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float appliedFriction = -Math.Sign(BaseSprite.Speed) * friction; // make sure friction points the right way
            float motorAcceleration = 0.0f;

            // get the correct acceleration based on which direction tank is moving, how fast it should move
            motorAcceleration += (playerAcceleration * padState.ThumbSticks.Left.Y);

            // if tank would hit a wall in this update cycle, set it's speed to negative 2/3 of current so bounces
            float newSpeed = BaseSprite.Speed + (motorAcceleration + appliedFriction);
            Vector2 newVelocity = newSpeed * BaseSprite.Heading;
            Vector2 newWorldLocation = BaseSprite.WorldLocation + (newVelocity * elapsed);
            if (BaseSprite.onWallTile(BaseSprite.Scale, BaseSprite.Rotation, newWorldLocation) != new Vector2(-1, -1))
            {
                BaseSprite.Speed = -(BaseSprite.Speed * 2 / 3);
            }
            // otherwise, update speed with new acceleration
            else
            {
                // if friction is causing it to jump between positive and negative speeds instead of go to zero
                if ((motorAcceleration == 0) && (Math.Sign(BaseSprite.Speed) != Math.Sign(newSpeed)))
                {
                    BaseSprite.Speed = 0f;
                }
                else
                {
                    BaseSprite.Speed = newSpeed;
                }
            }

            // make sure tank doesn't exceed speed limitations
            BaseSprite.Speed = MathHelper.Clamp(BaseSprite.Speed, -playerBackMaxSpeed, playerForwardMaxSpeed);
        }

        /// <summary>
        /// Move crosshair based on gamepad joystick info, keeping it within window
        /// </summary>
        /// <param name="padState">State of the gamePad for the current update cycle</param>
        private void handleGamePadCrosshairLocation(GamePadState padState)
        {
            Vector2 rightThumVector = new Vector2(padState.ThumbSticks.Right.X, -padState.ThumbSticks.Right.Y);
            Vector2 newCrosshairLocation = CrosshairSprite.WorldLocation + (rightThumVector * crosshairSpeed);
            newCrosshairLocation.X = MathHelper.Clamp(newCrosshairLocation.X, Camera.ViewPort.Left, Camera.ViewPort.Right);
            newCrosshairLocation.Y = MathHelper.Clamp(newCrosshairLocation.Y, Camera.ViewPort.Top, Camera.ViewPort.Bottom);

            CrosshairSprite.WorldLocation = newCrosshairLocation;
        }

        /// <summary>
        /// Fire a shot if gamepad trigger down and timer high enough
        /// </summary>
        /// <param name="padState">Gamepad state for current update cycle</param>
        private void handleGamePadWeapon(GamePadState padState)
        {
            Vector2 shotLocation = Vector2.Transform(turretEndOffset, TurretSprite.TransformationMatrix);

            if (CanFireWeapon && !TileMap.IsWallTileByPixel(shotLocation) && padState.IsButtonDown(Buttons.RightTrigger))
            {
                FireWeapon(shotLocation, TurretSprite.Heading, WeaponManager.WeaponSpeed);
            }
        }

        #endregion

        #endregion

        #region Movement Limitations

        /// <summary>
        /// Prevent base tank sprite from leaving the world boundaries.
        /// </summary>
        private void clampToWorld()
        {
            float currentX = BaseSprite.WorldLocation.X;
            float currentY = BaseSprite.WorldLocation.Y;

            currentX = MathHelper.Clamp(
                currentX, BaseSprite.FrameWidth / 2, Camera.WorldRectangle.Right - (BaseSprite.FrameWidth / 2));
            currentY = MathHelper.Clamp(
                currentY, BaseSprite.FrameHeight / 2, Camera.WorldRectangle.Bottom - (BaseSprite.FrameHeight / 2));

            BaseSprite.WorldLocation = new Vector2(currentX, currentY);
        }

        /// <summary>
        /// Try to keep tank within scrollArea rectangle by moving camera when it leaves.
        /// </summary>
        /// <param name="gameTime">Used to calculate how far to move the camera each update cycle.</param>
        private void repositionCamera(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 moveAmount = BaseSprite.Velocity * elapsed;

            if (((BaseSprite.ScreenRectangle.X < scrollArea.X) && (moveAmount.X < 0)) ||
                ((BaseSprite.ScreenRectangle.Right > scrollArea.Right) && (moveAmount.X > 0)))
            {
                Camera.Move(new Vector2(moveAmount.X, 0));
            }

            if (((BaseSprite.ScreenRectangle.Y < scrollArea.Y) && (moveAmount.Y < 0)) ||
                ((BaseSprite.ScreenRectangle.Bottom > scrollArea.Bottom) && (moveAmount.Y > 0)))
            {
                Camera.Move(new Vector2(0, moveAmount.Y));
            }
        }

        #endregion

        #region Weapon Firing

        public bool CanFireWeapon
        {
            get
            {
                return (shotTimer > shotMinTimer);
            }
        }

        public void FireWeapon(Vector2 worldLocation, Vector2 heading, float speed)
        {
            shotTimer = 0f;
            WeaponManager.AddShot(worldLocation, heading, speed, 0);
        }

        #endregion

        #region Update and Draw

        /// <summary>
        /// Updates base sprite and sets turret sprite to hub location
        /// </summary>
        /// <param name="gameTime">Used to calculate elapsed time since last update cycle</param>
        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            shotTimer += elapsed ;

            handleMovementInput(gameTime);
            repositionCamera(gameTime);

            BaseSprite.Update(gameTime);
            clampToWorld();

            handleTurretInput(gameTime);
        }

        /// <summary>
        /// Draws the base sprite and turret sprite
        /// </summary>
        /// <param name="spriteBatch">Allows game to draw tank and turret</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            BaseSprite.Draw(spriteBatch);
            TurretSprite.Draw(spriteBatch);
        }

        /// <summary>
        /// Draws the crosshair sprite
        /// </summary>
        /// <param name="spriteBatch">Allows game to draw crosshair</param>
        public void DrawCrosshair(SpriteBatch spriteBatch)
        {
            CrosshairSprite.Draw(spriteBatch);
        }

        #endregion
    }
}
