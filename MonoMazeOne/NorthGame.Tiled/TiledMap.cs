// Rapbit Game development
//
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NorthGame.Core.Abstractions;
using NorthGame.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NorthGame.Tiled
{
    public interface ITiledMap
    {

    }

    public class TiledMap : IGameElement, ITiledMap
    {
        /// <summary>
        /// The layout contains a file path to the design file
        /// </summary>
        public string Layout { get; set; }

        /// <summary>
        /// Height and width contain the size of the design layer
        /// measured in tiles.
        /// </summary>
        public int Height;

        public int Width;
        public bool Infinite;
        public string Orientation;
        public string TiledVersion;
        public float Version;
        public int TileHeight;
        public int TileWidth;
        public string Type;

        public List<MapLayer> Layers = new List<MapLayer>();
        public List<TileSet> TileSets = new List<TileSet>();

        public Point TileDimensions => _tileDimensions;
        private Point _tileDimensions;


        public void LoadContent(string layout, IGameElementFactory tileFactory, bool colisionMap)
        {
            Layout = layout;
            this.Populate(Layout);
            _tileDimensions = new Point(TileWidth, TileHeight);
            Layers.Visit((m) => m.LoadContent(TileDimensions, tileFactory, TileSets.First(), colisionMap));
        }

        public void LoadContent()
        {
            throw new NotSupportedException("Use LoadContent(string layout, ITileFactory tileFactory)");
        }

        public void UnloadContent()
        {
            Layers.Visit((m) => m.UnloadContent());
        }

        public void Update(GameTime gameTime)
        {
            Layers.Visit((m) => m.Update(gameTime));
        }

        public void Update(GameTime gameTime, Direction playerMoveDirection)
        {
            Layers.Visit((m) => m.Update(gameTime, playerMoveDirection));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Layers.Visit((m) => m.Draw(spriteBatch));
        }
        //spriteBatch, 0, 0, 3, 3, 9, 9);

        public void Draw(SpriteBatch spriteBatch, Vector2 screenoffset, Vector2 viewSize)
        {
            Layers.Visit((m) => m.Draw(spriteBatch, screenoffset, viewSize));
        }

        public void Draw(SpriteBatch spriteBatch, LayerType zPLane)
        {
            var layer = Layers.FirstOrDefault(m => m.ZPlane == zPLane);
            layer?.Draw(spriteBatch);
        }
      
    }
}
