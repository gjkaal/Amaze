// Rapbit Game development
//
namespace NorthGame.Core.Abstractions
{
    public interface ITileSet
    {
        int Columns { get; set; }
        int FirstGid { get; set; }
        string Image { get; set; }
        int ImageHeight { get; set; }
        int ImageWidth { get; set; }
        int Margin { get; set; }
        string Name { get; set; }
        int Spacing { get; set; }
        int TileCount { get; set; }
        int TileHeight { get; set; }
        int TileWidth { get; set; }
        string TransparentColor { get; set; }
    }
}