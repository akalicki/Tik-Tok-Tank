using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tik_Tok_Tank
{
    static class PlayerManager
    {
        private static List<Player> playerList = new List<Player>();
        public static int NeededPlayers = 1; // number of remaining players needed to continue playing

        /// <summary>
        /// Returns whether the game is over (less players playing than number needed to keep playing)
        /// </summary>
        public static bool GameFinished
        {
            get { return (NumPlayers < NeededPlayers); }
        }

        /// <summary>
        /// Gets the number of players in the list of players
        /// </summary>
        public static int NumPlayers
        {
            get { return playerList.Count; }
        }

        /// <summary>
        /// Adds a player to the list
        /// </summary>
        /// <param name="player">Player to add</param>
        public static void AddPlayer(Player player)
        {
            playerList.Add(player);
        }

        /// <summary>
        /// Gets a player at the specified index
        /// </summary>
        /// <param name="index">Index of player to get</param>
        /// <returns>Player at index</returns>
        public static Player GetPlayer(int index)
        {
            return playerList[index];
        }

        /// <summary>
        /// Removes the player at the specified index
        /// </summary>
        /// <param name="index">Index of player to remove</param>
        public static void RemovePlayer(int index)
        {
            playerList.RemoveAt(index);
        }

        /// <summary>
        /// Bounce players off each other if they will come in contact at next frame (make more sophisticated later)
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values for current update cyrcle</param>
        private static void bouncePlayers(GameTime gameTime)
        {
            // TODO: make this work based on pixel collisions
            if (NumPlayers == 2)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                Vector2 newPos1 = playerList[0].BaseSprite.WorldLocation + (playerList[0].BaseSprite.Velocity * elapsed);
                Vector2 newPos2 = playerList[1].BaseSprite.WorldLocation + (playerList[1].BaseSprite.Velocity * elapsed);

                if (Vector2.Distance(newPos1, newPos2) < 45)
                {
                    foreach (Player player in playerList)
                    {
                        player.BaseSprite.Speed *= -1;
                    }
                }
            }
        }

        /// <summary>
        /// Clears the list of players - should be called when initializing each game
        /// </summary>
        public static void ClearPlayerList()
        {
            playerList.Clear();
        }

        /// <summary>
        /// Updates each player in the player list
        /// </summary>
        /// <param name="gameTime">TIming values for update cycle</param>
        public static void Update(GameTime gameTime)
        {
            bouncePlayers(gameTime);
            foreach (Player player in playerList)
            {
                player.Update(gameTime);
            }
        }

        /// <summary>
        /// Draws each player in the player list and associated crosshairs
        /// </summary>
        /// <param name="spriteBatch">Used to draw tank, turret, and crosshair to screen</param>
        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (Player player in playerList)
            {
                player.Draw(spriteBatch);
            }

            foreach (Player player in playerList)
            {
                player.DrawCrosshair(spriteBatch);
            }
        }
    }
}
