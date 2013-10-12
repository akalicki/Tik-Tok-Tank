using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tik_Tok_Tank
{
    /// <summary>
    /// Defines moving, drawing, animation, and collision functions for all sprites
    /// </summary>
    class Sprite
    {
        #region Declarations

        // spritesheet where texture is located
        public Texture2D Texture;

        // origin, location, heading and speed of sprite
        private Vector2 origin = Vector2.Zero;
        private Vector2 worldLocation = Vector2.Zero;

        private Vector2 heading = Vector2.UnitX; // direction sprite is facing
        private float speed = 0.0f; // speed of tank in direction it's facing

        // list to store animation frames, other frame animation properties
        private List<Rectangle> frames = new List<Rectangle>();
        private int currentFrame;
        private float frameTime = 0.1f; // amount of time before changing frame
        private float timeForCurrentFrame = 0.0f;

        // Color to tint the sprite (most of the time just white)
        private Color tintColor = Color.White;

        // rotation, in radians, of sprite from facing directly right
        private float rotation = 0.0f;

        // scale factor of sprite for when it's drawn
        private float scale = 1.0f;

        // information which will be used to determine whether it should be drawn or animate
        public bool Expired = false;
        public bool Animate = true;
        public bool AnimateWhenStopped = true;

        // whether sprite should populate pixelLocations list
        public bool PixelCollidable = false;

        // list containing non-transparent pixel locations of sprite for collisions
        public List<Vector2> pixelLocations;

        // circle collision radius
        public float circleCollideRadius = 0;

        #endregion

        #region Constructors

        /// <summary>
        /// Default sprite constructor
        /// </summary>
        /// <param name="origin">Origin of sprite from top left corner to use for location at and rotate around</param>
        /// <param name="worldLocation">Initial position in absolute screen coordinates</param>
        /// <param name="texture">Spritesheet where sprite image is located</param>
        /// <param name="initialFrame">Rectangle indicating position of first frame on the spritesheet</param>
        /// <param name="heading">Initial heading of sprite</param>
        /// <param name="speed">Initial speed of sprite</param>
        public Sprite(
            Texture2D texture,
            Rectangle initialFrame,
            Vector2 origin,
            Vector2 worldLocation,
            Vector2 heading,
            float speed,
            bool pixelCollidable = false)
        {
            Origin = origin;
            WorldLocation = worldLocation;

            Texture = texture;
            frames.Add(initialFrame);

            Heading = heading;
            Speed = speed;

            PixelCollidable = pixelCollidable;
            if (PixelCollidable)
            {
                pixelLocations = colorsToLocations(Texture, initialFrame);
            }
        }

        #endregion

        #region Collidable Pixel Population

        /// <summary>
        /// Turn the texture region into a 2D array of colors
        /// </summary>
        /// <param name="texture">Texture to grab the data from</param>
        /// <param name="initialFrame">Region of texture from which to grab the data</param>
        /// <returns>2D color array</returns>
        private Color[,] TextureTo2DArray(Texture2D texture, Rectangle initialFrame)
        {
            Color[] colors1D = new Color[initialFrame.Width * initialFrame.Height];
            texture.GetData(0, initialFrame, colors1D, 0, colors1D.Length);

            Color[,] colors2D = new Color[initialFrame.Width, initialFrame.Height];
            for (int x = 0; x < initialFrame.Width; x++)
            {
                for (int y = 0; y < initialFrame.Height; y++)
                {
                    colors2D[x, y] = colors1D[x + y * initialFrame.Width];
                }
            }

            return colors2D;
        }

        /// <summary>
        /// Make a list of all positions of collidable pixels relative to origin
        /// </summary>
        /// <param name="initialFrame">Region of texture where sprite is located.</param>
        /// <returns></returns>
        private List<Vector2> colorsToLocations(Texture2D texture, Rectangle initialFrame)
        {
            Color[,] colors2D = TextureTo2DArray(texture, initialFrame);
            List<Vector2> pixelLocations = new List<Vector2>();

            for (int x = 0; x < colors2D.GetLength(0); x++)
            {
                for (int y = 0; y < colors2D.GetLength(1); y++)
                {
                    if (colors2D[x, y].A > 0)
                    {
                        pixelLocations.Add(new Vector2(x - (initialFrame.Width / 2), y - (initialFrame.Height / 2)));
                    }
                }
            }

            return pixelLocations;
        }

        public Vector2 onWallTile(float scale, float rotation, Vector2 location)
        {
            foreach (Vector2 pixel in pixelLocations)
            {
                Vector2 transformedLocation = Vector2.Transform(
                    pixel, getTransformationMatrix(scale, rotation, location));
                if (TileMap.IsWallTileByPixel(transformedLocation))
                {
                    return TileMap.GetSquareAtPixel(transformedLocation) ;
                }
            }
            return new Vector2(-1, -1);
        }

        public Vector2 onWallTile()
        {
            return onWallTile(Scale, Rotation, WorldLocation);
        }

        #endregion

        #region Drawing and Animation Properties

        /// <summary>
        /// Gets width of initial frame of sprite
        /// </summary>
        public int FrameWidth
        {
            get { return frames[0].Width; }
        }

        /// <summary>
        /// Gets height of initial frame of sprite
        /// </summary>
        public int FrameHeight
        {
            get { return frames[0].Height; }
        }

        /// <summary>
        /// Gets or sets index of current frame in list of frames, constraining index to number of frames
        /// </summary>
        public int CurrentFrame
        {
            get { return currentFrame; }
            set { currentFrame = (int)MathHelper.Clamp(value, 0, frames.Count - 1); } // prevents from exceeding total frames
        }

        /// <summary>
        /// Gets or sets time per frame of animation, preventing from going below 0
        /// </summary>
        public float FrameTime
        {
            get { return frameTime; }
            set { frameTime = MathHelper.Max(0, value); } // prevents frame time from going below 0
        }

        /// <summary>
        /// Gets rectangle representing the current frame in the spritesheet
        /// </summary>
        public Rectangle SourceFrame
        {
            get { return frames[CurrentFrame]; }
        }

        /// <summary>
        /// Gets or sets the tint color of the sprite (solid white meaning no tint)
        /// </summary>
        public Color TintColor
        {
            get { return tintColor; }
            set { tintColor = value; }
        }

        /// <summary>
        /// Gets or sets the scale of the sprite
        /// </summary>
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        /// <summary>
        /// Gets or sets the sprite's rotation, keeping it constrained from 0 to 2pi
        /// </summary>
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value % MathHelper.TwoPi; }
        }

        #endregion

        #region Position Properties

        /// <summary>
        /// Gets or sets the sprite's origin relative to its top left corner.
        /// The origin is the point at which its location is calculated and which it is rotated around.
        /// </summary>
        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        /// <summary>
        /// Gets or sets the location of the origin of the sprite in world coordinates
        /// </summary>
        public Vector2 WorldLocation
        {
            get { return worldLocation; }
            set { worldLocation = value; }
        }

        /// <summary>
        /// Gets the location of the origin of the sprite in screen coordinates
        /// </summary>
        public Vector2 ScreenLocation
        {
            get
            {
                return Camera.TransformToScreen(WorldLocation);
            }
        }

        /// <summary>
        /// Gets rectangle representing sprite's area in absolute coordinates by subtracting origin offset from location.
        /// Might not work well with rotation - should check later.
        /// </summary>
        public Rectangle WorldRectangle
        {
            // TODO: change this to work with rotation (might just need to add extra space to all sides of rectangle) 
            get
            {
                return new Rectangle(
                    (int)(WorldLocation.X - Origin.X),
                    (int)(WorldLocation.Y - Origin.Y),
                    FrameWidth,
                    FrameHeight);
            }
        }

        /// <summary>
        /// Gets rectangle representing the sprite's area in screen coordinates
        /// </summary>
        public Rectangle ScreenRectangle
        {
            get
            {
                return Camera.TransformToScreen(WorldRectangle);
            }
        }

        /// <summary>
        /// Gets or sets the sprite's heading (normalized vector pointing in direction it's facing),
        /// </summary>
        public Vector2 Heading
        {
            get { return heading; }
            set
            {
                // set heading to normalized vector if not of length zero
                if (value != Vector2.Zero)
                {
                    heading = value;
                    heading.Normalize();
                }
            }
        }

        /// <summary>
        /// Gets the vector perpendicular to heading in clockwise direction
        /// </summary>
        public Vector2 Normal
        {
            get
            {
                return new Vector2(Heading.Y, -Heading.X);
            }
        }

        /// <summary>
        /// Gets or sets the sprite's speed (negative = backwards)
        /// </summary>
        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        /// <summary>
        /// Gets the sprite's velocity vector (heading * speed)
        /// </summary>
        public Vector2 Velocity
        {
            get { return (Heading * Speed); }
        }

        /// <summary>
        /// Gets a matrix representing the rotation, scale, and translation of any point on the sprite relative to its origin.
        /// </summary>
        public Matrix TransformationMatrix
        {
            get
            {
                return getTransformationMatrix(Scale, Rotation, WorldLocation);
            }
        }

        #endregion

        #region Animation Methods

        /// <summary>
        /// Add a new frame to the sprite's animation frame list
        /// </summary>
        /// <param name="frameRectangle">Rectangle defining new frame on spritesheet</param>
        public void AddFrame(Rectangle frameRectangle)
        {
            frames.Add(frameRectangle);
        }

        /// <summary>
        /// Sets the sprite's rotation angle to be facing in given direction
        /// </summary>
        /// <param name="direction">Vector representing direction of sprite to face</param>
        public void RotateTo(Vector2 direction)
        {
            Rotation = (float)Math.Atan2(direction.Y, direction.X);
        }

        /// <summary>
        /// Gets the transformation matrix around the sprite's origin if it was at a certain scale, rotation, and location
        /// </summary>
        /// <param name="scale">how much sprite would be scaled</param>
        /// <param name="rotation">how much sprite would be rotated</param>
        /// <param name="location">where sprite's origin would be located</param>
        /// <returns>transformation matrix of sprite given a scale, rotation, and location</returns>
        public Matrix getTransformationMatrix(float scale, float rotation, Vector2 location)
        {
            // assumes origin of sprite is already at place where it should be rotated, so no origin translation
            Matrix scaleMatrix = Matrix.CreateScale(scale);
            Matrix rotationMatrix = Matrix.CreateRotationZ(rotation);
            Matrix translationMatrix = Matrix.CreateTranslation(location.X, location.Y, 0);

            return (scaleMatrix * rotationMatrix * translationMatrix);
        }

        #endregion

        #region

        /// <summary>
        /// Gets whether a certain point falls within a sprite's circle collision radius
        /// </summary>
        /// <param name="pointLocation">Point to check collision with</param>
        /// <returns>True if collision, false if not</returns>
        public bool spriteCircleCollidesWith(Vector2 pointLocation)
        {
            if (Vector2.Distance(WorldLocation, pointLocation) <= circleCollideRadius)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region Update and Draw Methods

        /// <summary>
        /// Sprite update function - handles animation and location changes based on velocity
        /// </summary>
        /// <param name="gameTime">Object used to calculate amount of time since last update cycle</param>
        public virtual void Update(GameTime gameTime)
        {
            if (!Expired)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                timeForCurrentFrame += elapsed;

                if (Animate) // if sprite is supposed to animate
                {
                    if (timeForCurrentFrame >= FrameTime) // if it's time to change frames
                    {
                        if ((AnimateWhenStopped) || (Velocity != Vector2.Zero)) // if it's the right circumstances to animate
                        {
                            if ((CurrentFrame == 0) && (Speed < 0)) // going backwards and first frame
                            {
                                CurrentFrame = frames.Count - 1;
                            }

                            else // going forwards or going backwards and not first frame
                            {
                                CurrentFrame = (CurrentFrame + Math.Sign(Speed)) % (frames.Count);
                            }

                            timeForCurrentFrame = 0.0f;
                        }
                    }
                }

                WorldLocation += (Velocity * elapsed); // add to world location (d = rt)
            }
        }

        /// <summary>
        /// Sprite draw function - draws sprite to screen with location and rotation at origin point
        /// </summary>
        /// <param name="spriteBatch">Used to draw sprite to screen</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!Expired) // if sprite should be drawn
            {
                if (Camera.IsObjectVisible(WorldRectangle)) // if sprite is on the screen
                {
                    spriteBatch.Draw(
                        Texture,
                        ScreenLocation,
                        SourceFrame,
                        TintColor,
                        Rotation,
                        Origin,
                        Scale,
                        SpriteEffects.None,
                        0.0f);
                }
            }
        }

        #endregion
    }
}
