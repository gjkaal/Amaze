// Rapbit Game development
//
namespace NorthGame.Core.Abstractions
{
    public interface ITileSet
    {
        int Columns { get;  }
        int FirstGid { get;  }
        string Image { get;  }
        int ImageHeight { get;  }
        int ImageWidth { get;  }
        int Margin { get;  }
        string Name { get;  }
        int Spacing { get;  }
        int TileCount { get;  }
        int TileHeight { get;  }
        int TileWidth { get;  }
        string TransparentColor { get;  }
        string Source { get;  }
    }
}