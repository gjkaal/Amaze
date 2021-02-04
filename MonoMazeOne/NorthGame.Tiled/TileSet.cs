// NorthGame Game development
//
using NorthGame.Core.Model;

namespace NorthGame.Tiled
{
    public class TileSet : ITileSet
    {
        public string Name { get; set; }
        public int FirstGid { get; set; }
        public int Columns { get; set; }
        public string Image { get; set; }
        public int ImageHeight { get; set; }
        public int ImageWidth { get; set; }
        public int Margin { get; set; }
        public int Spacing { get; set; }
        public int TileCount { get; set; }
        public int TileHeight { get; set; }
        public int TileWidth { get; set; }
        public string TransparentColor { get; set; }
    }
}
