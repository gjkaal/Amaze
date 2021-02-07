// Rapbit Game development
//
using NorthGame.Core.Abstractions;

namespace NorthGame.Core.ContainerService
{
    public class NorthGameConfiguration : INorthGameConfiguration
    {
        public float ScreenWidth { get; set; } = 640.0f;
        public float ScreenHeight { get; set; } = 380.0f;
        public bool FullScreen { get; set; }  = false;
    }
}
