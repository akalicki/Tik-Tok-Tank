using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tik_Tok_Tank
{
    /// <summary>
    /// Handle particle and explosion animation
    /// </summary>
    class Particle : Sprite
    {
        #region Declarations

        // particle movement
        private Vector2 acceleration;
        private float maxSpeed;

        // duration properties for particle
        private int initialDuration;
        private int remainingDuration;
        
        // color change of particle
        private Color initialColor;
        private Color finalColor;

        #endregion

        #region Properties

        /// <summary>
        /// Elapsed time since particle was created.
        /// </summary>
        public int ElapsedDuration
        {
            get
            {
                return (initialDuration - remainingDuration);
            }
        }

        /// <summary>
        /// Float representing percentage of duration completed.
        /// </summary>
        public float DurationProgress
        {
            get
            {
                return ((float)ElapsedDuration / (float)initialDuration);
            }
        }

        /// <summary>
        /// Whether or not the particle's duration has run out.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return (remainingDuration > 0);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Sprite that can expire and change color over a given period of time
        /// </summary>
        /// <param name="texture">Spritesheet containing particle texture</param>
        /// <param name="initialFrame">Rectangle representing first frame of particle sprite on spritesheet</param>
        /// <param name="location">Location to draw the particle in world coordinates</param>
        /// <param name="heading">Which direction the particle is heading</param>
        /// <param name="speed">The initial speed of the particle</param>
        /// <param name="acceleration">Direction and amount the particle is accelerating</param>
        /// <param name="maxSpeed">Fastest the particle can travel</param>
        /// <param name="duration">How long the particle should last before dying</param>
        /// <param name="initialColor">Color the particle should start as</param>
        /// <param name="finalColor">Color the particle should end as</param>
        public Particle(
            Texture2D texture,
            Rectangle initialFrame,
            Vector2 worldLocation,
            Vector2 heading,
            float speed,
            Vector2 acceleration,
            float maxSpeed,
            int duration,
            Color initialColor,
            Color finalColor,
            bool pixelCollidable = false)
            : base(texture, initialFrame, new Vector2(initialFrame.Width / 2, initialFrame.Height / 2), worldLocation, heading, speed, pixelCollidable)
        {
            initialDuration = duration;
            remainingDuration = duration;
            this.acceleration = acceleration;
            this.initialColor = initialColor;
            this.finalColor = finalColor;
            this.maxSpeed = maxSpeed;
        }

        #endregion

        #region Update and Draw

        /// <summary>
        /// If particle hasn't expired, update its velocity based on acceleration and then update sprite
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (remainingDuration <= 0)
            {
                Expired = true;
            }

            if (!Expired)
            {
                Vector2 newVelocity = Velocity + acceleration;
                Heading = newVelocity;
                Speed = MathHelper.Min(newVelocity.Length(), maxSpeed);

                TintColor = Color.Lerp(
                    initialColor,
                    finalColor,
                    DurationProgress);

                remainingDuration--;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draw the particle to the screen.
        /// </summary>
        /// <param name="spriteBatch">Used to draw the particle</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        #endregion
    }
}
