using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Tik_Tok_Tank
{
    /// <summary>
    /// Manages the sound for the game.
    /// </summary>
    static class SoundManager
    {
        #region Declarations

        private static Song backgroundMusic;

        #endregion

        /// <summary>
        /// Loads all sound effects and starts background music playing
        /// </summary>
        /// <param name="content"></param>
        public static void Initialize(ContentManager content)
        {
            try
            {
                backgroundMusic = content.Load<Song>(@"Sounds\backgroundMusic");

                StartBackgroundMusic();
            }
            catch
            {
                Debug.Write("SoundManager Initialization Failed");
            }
        }

        private static MediaState playerState
        {
            get { return MediaPlayer.State; }
        }

        public static bool MusicPlaying
        {
            get
            {
                return (playerState == MediaState.Playing);
            }
        }

        /// <summary>
        /// Sets the media player to repeat, then starts background music playing
        /// </summary>
        public static void StartBackgroundMusic()
        {
            try
            {
                MediaPlayer.IsRepeating = true;

                if (playerState == MediaState.Stopped)
                {
                    MediaPlayer.Play(backgroundMusic);
                }
                else if (playerState == MediaState.Paused)
                {
                    MediaPlayer.Resume();
                }
            }
            catch
            {
                Debug.Write("StartBackgroundMusic Failed");
            }
        }

        public static void PauseBackgroundMusic()
        {
            try
            {
                MediaPlayer.Pause();
            }
            catch
            {
                Debug.Write("StopBackgroundMusic Failed");
            }
        }
    }
}
