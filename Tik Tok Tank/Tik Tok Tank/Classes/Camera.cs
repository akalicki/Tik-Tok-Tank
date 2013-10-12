using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Tik_Tok_Tank
{
    /// <summary>
    /// Game window and movement, transformation methods between world and screen coordinates
    /// </summary>
    static class Camera
    {
        #region Declarations

        private static Vector2 position = Vector2.Zero; // position of camera's top left corner
        private static Vector2 viewPortSize = Vector2.Zero; // width and height of viewport
        private static Rectangle worldRectangle = new Rectangle(0, 0, 0, 0); // absolute size of game world

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the rectangle that defines absolute screen area
        /// </summary>
        public static Rectangle WorldRectangle
        {
            get { return worldRectangle; }
            set { worldRectangle = value; }
        }

        /// <summary>
        /// Gets or sets the width of the camera viewport
        /// </summary>
        public static int ViewPortWidth
        {
            get { return (int)viewPortSize.X; }
            set { viewPortSize.X = value; }
        }

        /// <summary>
        /// Gets or sets the height of the camera viewport
        /// </summary>
        public static int ViewPortHeight
        {
            get { return (int)viewPortSize.Y; }
            set { viewPortSize.Y = value; }
        }

        /// <summary>
        /// Position of camera in game world.  Setting the position
        /// automatically clamps it so it stays inside the world rectangle
        /// </summary>
        public static Vector2 Position
        {
            get { return position; }
            set
            {
                position = new Vector2(
                    MathHelper.Clamp(value.X, WorldRectangle.X, WorldRectangle.Width - ViewPortWidth),
                    MathHelper.Clamp(value.Y, WorldRectangle.Y, WorldRectangle.Height - ViewPortHeight));
            }
        }

        /// <summary>
        /// Gets a rectangle representing the current camera viewport
        /// </summary>
        public static Rectangle ViewPort
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, ViewPortWidth, ViewPortHeight);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resets camera position to (0, 0) - should be used when initializing game mode
        /// </summary>
        public static void Reset()
        {
            position = Vector2.Zero;
        }

        /// <summary>
        /// Move the camera by a specific amount
        /// </summary>
        /// <param name="offset">Vector representing the x and y offset (in pixels) to move the camera</param>
        public static void Move(Vector2 offset)
        {
            // Position property keeps camera onscreen, so only need to add offset
            Position += offset;
        }

        /// <summary>
        /// Checks to see if the object is within the camera bounds (onscreen)
        /// </summary>
        /// <param name="objectBounds">Rectangle representing outer bounds of object in world coordinates</param>
        /// <returns>True if onscreen, false if offscreen</returns>
        public static bool IsObjectVisible(Rectangle objectBounds)
        {
            return (objectBounds.Intersects(ViewPort));
        }

        /// <summary>
        /// Transforms absolute world coordinates into screen coordinates
        /// </summary>
        /// <param name="worldPoint">Vector representing world coordinates of object</param>
        /// <returns>Vector representing screen coordinates of object</returns>
        public static Vector2 TransformToScreen(Vector2 worldPoint)
        {
            // subtract world coordinates from top-left of camera to get screen coordinates
            return (worldPoint - Position);
        }

        /// <summary>
        /// Transforms rectangle representing world position into one representing screen position
        /// </summary>
        /// <param name="worldRectangle">Rectangle representing location in world coordinates</param>
        /// <returns>Rectangle representing location in screen coordinates</returns>
        public static Rectangle TransformToScreen(Rectangle worldRectangle)
        {
            return new Rectangle(
                worldRectangle.Left - (int)Position.X,
                worldRectangle.Top - (int)Position.Y,
                worldRectangle.Width, worldRectangle.Height);
        }

        /// <summary>
        /// Transforms screen coordinates into absolute world coordinates
        /// </summary>
        /// <param name="screenPoint">Vector representing screen coordinates of object</param>
        /// <returns>Vector representing screen coordinates of object</returns>
        public static Vector2 TransformToWorld(Vector2 screenPoint)
        {
            return (screenPoint + Position);
        }

        /// <summary>
        /// Transforms rectangle representing screen position into one representing world position
        /// </summary>
        /// <param name="screenRectangle">Rectangle representing location in screen coordinates</param>
        /// <returns>Rectangle representing location in world coordinates</returns>
        public static Rectangle TransformToWorld(Rectangle screenRectangle)
        {
            return new Rectangle(
                screenRectangle.Left + (int)Position.X,
                screenRectangle.Top + (int)Position.Y,
                screenRectangle.Width, screenRectangle.Height);
        }

        #endregion
    }
}
