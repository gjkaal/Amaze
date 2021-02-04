// Rapbit Game development
//
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NorthGame.Core;
using NorthGame.Core.Extensions;
using NorthGame.Core.Model;
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


        public void LoadContent(string layout, IGameElementFactory tileFactory)
        {
            Layout = layout;
            this.Populate(Layout);
            _tileDimensions = new Point(TileWidth, TileHeight);
            Layers.Visit((m) => m.LoadContent(TileDimensions, tileFactory, TileSets.First()));
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

        public void Draw(SpriteBatch spriteBatch)
        {
            Layers.Visit((m) => m.Draw(spriteBatch));
        }

        public void Draw(SpriteBatch spriteBatch, LayerType zPLane)
        {
            var layer = Layers.FirstOrDefault(m => m.ZPlane == zPLane);
            layer?.Draw(spriteBatch);
        }

        public bool CheckTileCollision(IPlayer player, out ITile tile, out Rectangle hitBox)
        {
            return Layers
                .Single(m => m.ZPlane == LayerType.Collision)
                .CheckTileCollision(player, out tile, out hitBox);
        }
    }
}
