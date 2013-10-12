using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tik_Tok_Tank
{
    /// <summary>
    /// Create a random, navigable map of tiles
    /// </summary>
    static class TileMap
    {
        #region Declarations

        // tile and map dimensions constants
        public const int TileWidth = 48;
        public const int TileHeight = 48;
        public static int MapWidth;
        public static int MapHeight;

        // constants representing tile ranges in the tile rectangle list
        public const int FloorTileStart = 0;
        public const int FloorTileEnd = 3;
        public const int WallTileStart = 4;
        public const int WallTileEnd = 7;
        public const int BreakWallTileStart = 8;
        public const int BreakWallTileEnd = 11;

        public static int currentFloorTile = 0;

        // texture variable for the spritesheet that stores the tiles
        private static Texture2D texture;

        // list to store all possible tile rectangles on spritesheet
        private static List<Rectangle> tiles = new List<Rectangle>();

        // 2D array that will hold the value of each tile in the map
        private static int[,] mapSquares;

        // random number generator
        private static Random rand = new Random();

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize TileMap class, adding all tile rectangles to its internal list
        /// and setting each square to begin with the first floor tile
        /// </summary>
        /// <param name="tileTexture">Spritesheet texture that contains the tiles</param>
        public static void Initialize(Texture2D tileTexture, int mapWidth, int mapHeight)
        {
            texture = tileTexture; // set spritesheet texture

            // set map width and height
            MapWidth = mapWidth;
            MapHeight = mapHeight;
            mapSquares = new int[MapWidth, MapHeight];

            tiles.Clear(); // clear current tile rectangle list

            // add floor tile rectangles to tile list
            tiles.Add(new Rectangle(0, 0, TileWidth, TileHeight));
            tiles.Add(new Rectangle(48, 0, TileWidth, TileHeight));
            tiles.Add(new Rectangle(96, 0, TileWidth, TileHeight));
            tiles.Add(new Rectangle(144, 0, TileWidth, TileHeight));
            // add wall tiles to tile list
            tiles.Add(new Rectangle(0, 48, TileWidth, TileHeight));
            tiles.Add(new Rectangle(48, 48, TileWidth, TileHeight));
            tiles.Add(new Rectangle(96, 48, TileWidth, TileHeight));
            tiles.Add(new Rectangle(144, 48, TileWidth, TileHeight));
            // add breakable wall tiles to tile list
            tiles.Add(new Rectangle(0, 96, TileWidth, TileHeight));
            tiles.Add(new Rectangle(48, 96, TileWidth, TileHeight));
            tiles.Add(new Rectangle(96, 96, TileWidth, TileHeight));
            tiles.Add(new Rectangle(144, 96, TileWidth, TileHeight));

            GenerateRandomMap();
        }

        #endregion

        #region Information about Map Squares

        /// <summary>
        /// Gets the horizontal square index of the tile map given an x-coordinate
        /// </summary>
        /// <param name="pixelX">The x-coordinate of the square</param>
        /// <returns>Horizontal square index that contains the pixel</returns>
        public static int GetSquareByPixelX(int pixelX)
        {
            return (pixelX / TileWidth);
        }

        /// <summary>
        /// Gets the vertical square index of the tile map given a y-coordinate
        /// </summary>
        /// <param name="pixelY">The y-coordinate of the square</param>
        /// <returns>Vertical square index that contains the pixel</returns>
        public static int GetSquareByPixelY(int pixelY)
        {
            return (pixelY / TileHeight);
        }

        /// <summary>
        /// Gets a vector representing the square indices for a given location
        /// </summary>
        /// <param name="pixelLocation">Vector representing absolute world coordinates of pixel</param>
        /// <returns>Vector representing square indices in tile map</returns>
        public static Vector2 GetSquareAtPixel(Vector2 pixelLocation)
        {
            return new Vector2(
                GetSquareByPixelX((int)pixelLocation.X),
                GetSquareByPixelY((int)pixelLocation.Y));
        }

        /// <summary>
        /// Gets the center of a given square
        /// </summary>
        /// <param name="squareX">X-index of square</param>
        /// <param name="squareY">Y-index of square</param>
        /// <returns>Vector representing absolute pixel coordinates of square center</returns>
        public static Vector2 GetSquareCenter(int squareX, int squareY)
        {
            return new Vector2(
                (squareX * TileWidth) + (TileWidth / 2),
                (squareY * TileHeight) + (TileHeight / 2));
        }

        /// <summary>
        /// Gets the center of a given square
        /// </summary>
        /// <param name="square">Vector representing indices of square</param>
        /// <returns>Vector representing absolute pixel coordinates of square center</returns>
        public static Vector2 GetSquareCenter(Vector2 square)
        {
            return GetSquareCenter(
                (int)square.X,
                (int)square.Y);
        }

        /// <summary>
        /// Get absolute pixel region of square
        /// </summary>
        /// <param name="squareX">X-index of square</param>
        /// <param name="squareY">Y-index of square</param>
        /// <returns>Rectangle representing square's absolute screen region</returns>
        public static Rectangle SquareWorldRectangle(int squareX, int squareY)
        {
            return new Rectangle(
                squareX * TileWidth,
                squareY * TileHeight,
                TileWidth,
                TileHeight);
        }

        /// <summary>
        /// Get absolute pixel region of square
        /// </summary>
        /// <param name="square">Vector representing square's indices</param>
        /// <returns>Rectangle representing square's absolute screen region</returns>
        public static Rectangle SquareWorldRectangle(Vector2 square)
        {
            return SquareWorldRectangle(
                (int)square.X,
                (int)square.Y);
        }

        /// <summary>
        /// Gets rectangle on screen of given square
        /// </summary>
        /// <param name="squareX">X-index of square</param>
        /// <param name="squareY">Y-index of square</param>
        /// <returns>Rectangle representing screen region of square</returns>
        public static Rectangle SquareScreenRectangle(int squareX, int squareY)
        {
            return Camera.TransformToScreen(
                SquareWorldRectangle(squareX, squareY));
        }

        /// <summary>
        /// Gets rectangle on screen of given square
        /// </summary>
        /// <param name="square">Vector representing square's indices</param>
        /// <returns>Rectangle representing screen region of square</returns>
        public static Rectangle SquareScreenRectangle(Vector2 square)
        {
            return SquareScreenRectangle(
                (int)square.X,
                (int)square.Y);
        }

        #endregion

        #region Information about Map Tiles

        /// <summary>
        /// Gets the tile value at a given square
        /// </summary>
        /// <param name="squareX">X-index of square</param>
        /// <param name="squareY">Y-index of square</param>
        /// <returns>Integer indicating tile value</returns>
        public static int GetTileAtSquare(int squareX, int squareY)
        {
            if ((squareX >= 0) && (squareX < MapWidth) && (squareY >= 0) && (squareY < MapHeight)) // checks if valid square
            {
                return mapSquares[squareX, squareY];
            }
            else // invalid square, return error value
            {
                return -1;
            }
        }

        /// <summary>
        /// Sets tile value for a given square
        /// </summary>
        /// <param name="squareX">X-index of square</param>
        /// <param name="squareY">Y-index of square</param>
        /// <param name="tile">Integer indicating tile value</param>
        public static void SetTileAtSquare(int squareX, int squareY, int tile)
        {
            if ((squareX >= 0) && (squareX < MapWidth) && (squareY >= 0) && (squareY < MapHeight)) // checks if valid square
            {
                mapSquares[squareX, squareY] = tile;
            }
        }

        /// <summary>
        /// Gets the tile value at the given pixel coordinates
        /// </summary>
        /// <param name="pixelX">X-coordinate of pixel</param>
        /// <param name="pixelY">Y-coordinate of pixel</param>
        /// <returns>Integer indicating tile value</returns>
        public static int GetTileAtPixel(int pixelX, int pixelY)
        {
            return GetTileAtSquare(
                GetSquareByPixelX(pixelX),
                GetSquareByPixelY(pixelY));
        }

        /// <summary>
        /// Gets the tile value at the given pixel location
        /// </summary>
        /// <param name="pixelLocation">Vector representing coordinates of pixel</param>
        /// <returns>Integer indicating tile value</returns>
        public static int GetTileAtPixel(Vector2 pixelLocation)
        {
            return GetTileAtPixel(
                (int)pixelLocation.X,
                (int)pixelLocation.Y);
        }

        /// <summary>
        /// Checks if tile at a given square is a wall (including breakable)
        /// </summary>
        /// <param name="squareX">X-index of square</param>
        /// <param name="squareY">Y-index of square</param>
        /// <returns>True if tile is a wall, false if not</returns>
        public static bool IsWallTile(int squareX, int squareY)
        {
            int tileIndex = GetTileAtSquare(squareX, squareY); // get tile index for square

            if (tileIndex == -1) // if no such square return false
            {
                return false;
            }

            return (tileIndex >= WallTileStart); // includes breakable wall tiles
        }

        /// <summary>
        /// Checks if tile at a given square is a wall (including breakable)
        /// </summary>
        /// <param name="square">Vector representing square indices</param>
        /// <returns>True if tile is a wall, false if not</returns>
        public static bool IsWallTile(Vector2 square)
        {
            return IsWallTile(
                (int)square.X,
                (int)square.Y);
        }

        /// <summary>
        /// Checks if tile at a given pixel location is a wall (including breakable)
        /// </summary>
        /// <param name="pixelLocation">Vector representing coordinates of pixel to check</param>
        /// <returns>True if tile is a wall, false if not</returns>
        public static bool IsWallTileByPixel(Vector2 pixelLocation)
        {
            return IsWallTile(
                GetSquareAtPixel(pixelLocation));
        }

        /// <summary>
        /// Checks if tile at a given square is a breakable wall
        /// </summary>
        /// <param name="squareX">X-index of square</param>
        /// <param name="squareY">Y-index of square</param>
        /// <returns>True if tile is a breakable wall, false if not</returns>
        public static bool IsBreakWallTile(int squareX, int squareY)
        {
            int tileIndex = GetTileAtSquare(squareX, squareY);

            if (tileIndex == -1) // if no such square return false
            {
                return false;
            }

            return ((tileIndex >= BreakWallTileStart) && (tileIndex <= BreakWallTileEnd));
        }

        /// <summary>
        /// Checks if tile at a given square is a breakable wall
        /// </summary>
        /// <param name="square">Vector representing square indices</param>
        /// <returns>True if tile is a breakable wall, false if not</returns>
        public static bool IsBreakWallTile(Vector2 square)
        {
            return IsBreakWallTile(
                (int)square.X,
                (int)square.Y);
        }

        /// <summary>
        /// Checks if tile at a given pixel location is a breakable wall
        /// </summary>
        /// <param name="pixelLocation">Vector representing coordinates of pixel to check</param>
        /// <returns>True if tile is a breakable wall, false if not</returns>
        public static bool IsBreakWallTileByPixel(Vector2 pixelLocation)
        {
            return IsBreakWallTile(
                GetSquareAtPixel(pixelLocation));
        }

        #endregion

        #region Map Generation

        /// <summary>
        /// Generate wall tiles at random locations on the map
        /// </summary>
        public static void GenerateRandomMap()
        {
            // percent chance that a wall or breakable wall will be spawned 
            int wallChancePerSquare = 10;
            int breakWallChancePerSquare = 2;

            // choose random floor tile, wall tile, breakable wall tile combo for map
            currentFloorTile = rand.Next(FloorTileStart, FloorTileEnd + 1);
            int wallTile = rand.Next(WallTileStart, WallTileEnd + 1);
            int breakWallTile = rand.Next(BreakWallTileStart, BreakWallTileEnd + 1);

            // assign each square in map to a specific tile
            for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    mapSquares[x, y] = currentFloorTile; // set square initially to floor tile

                    if ((x == 0) || (x == MapWidth - 1) || (y == 0) || (y == MapHeight - 1))
                    {
                        mapSquares[x, y] = wallTile; // line edges of map with walls
                        continue;
                    }

                    if ((x == 1) || (x == 2) || (x == MapWidth - 2) || (x == MapWidth - 3) ||
                        (y == 1) || (y == 2) || (y == MapHeight - 2) || (y== MapHeight - 3))
                    {
                        continue; // leave one block of free space on the insides of each edge
                    }

                    int chance = getWallChanceAtSquare(x, y);
                    if ((chance != 0) && chance <= wallChancePerSquare) // if in wall tile chance range
                    {
                        if (chance <= breakWallChancePerSquare) // if in breakable wall tile chance range
                        {
                            mapSquares[x, y] = breakWallTile;
                        }
                        else
                        {
                            mapSquares[x, y] = wallTile;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets chance of spawning wall tile at given square
        /// </summary>
        /// <param name="x">X-coordinate of square</param>
        /// <param name="y">Y-coordinate of square</param>
        /// <returns>0 if wall shouldn't spawn, upper chance limit for random if it can spawn</returns>
        private static int getWallChanceAtSquare(int x, int y)
        {
            // return 0 if square in a no-spawn location
            if (IsWallTile(x - 2, y) && !IsWallTile(x - 1, y))
                return 0;
            if (IsWallTile(x - 2, y - 1) && (IsWallTile(x - 1, y - 2) || (!IsWallTile(x - 1, y) && !IsWallTile(x - 1, y - 1))))
                return 0;
            if (IsWallTile(x - 2, y - 2) && !IsWallTile(x - 1, y - 1))
                return 0;
            if (IsWallTile(x - 1, y - 2) && (IsWallTile(x - 2, y - 1) || (!IsWallTile(x - 1, y - 1) && !IsWallTile(x, y - 1))))
                return 0;
            if (IsWallTile(x, y - 2) && !IsWallTile(x, y - 1))
                return 0;
            if (IsWallTile(x + 1, y - 2) && (IsWallTile(x + 2, y - 1) || (!IsWallTile(x, y - 1) && !IsWallTile(x + 1, y - 1))))
                return 0;
            if (IsWallTile(x + 2, y - 2) && !IsWallTile(x + 1, y - 1))
                return 0;
            if (IsWallTile(x + 2, y - 1) && (IsWallTile(x + 1, y - 2) || (!IsWallTile(x + 1, y - 1) && !IsWallTile(x + 1, y))))
                return 0;

            // if it can spawn, give it a randomly high chance of spawning
            return rand.Next(0, 30);
        }

        #endregion

        #region Drawing

        /// <summary>
        /// Draws all tiles within camera bounds.
        /// </summary>
        /// <param name="spriteBatch">Used to draw tiles to screen</param>
        public static void Draw(SpriteBatch spriteBatch)
        {
            // get tiles within bounds of window and only draw those
            int startX = GetSquareByPixelX((int)Camera.Position.X);
            int endX = GetSquareByPixelX((int)Camera.Position.X + Camera.ViewPortWidth);

            int startY = GetSquareByPixelY((int)Camera.Position.Y);
            int endY = GetSquareByPixelY((int)Camera.Position.Y + Camera.ViewPortHeight);

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    if ((x >= 0) && (x < MapWidth) && (y >= 0) && (y < MapHeight))
                    {
                        spriteBatch.Draw(
                            texture,
                            SquareScreenRectangle(x, y),
                            tiles[GetTileAtSquare(x, y)],
                            Color.White);
                    }
                }
            }
        }

        #endregion
    }
}
