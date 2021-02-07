// Rapbit Game development
//

namespace NorthGame.Core.Abstractions
{
    public interface IEffectable
    {
        IFadeEffect FadeEffect { get; }
        ISpriteSheetEffect SpriteSheetEffect { get; }

        string Effects { get; set; }

        void DeactivateEffect(string effect);
    }
}
