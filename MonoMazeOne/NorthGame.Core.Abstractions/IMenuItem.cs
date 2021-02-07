
namespace NorthGame.Core.Abstractions
{

    public interface IMenuItem
    {
        /// <summary>
        /// Linked to a screen or a menu.
        /// TODO: Use enum for link type
        /// </summary>
        string LinkType { get; }

        /// <summary>
        /// Reference to the linked item.
        /// </summary>
        string LinkId { get; }

        /// <summary>
        /// Display element for the menu
        /// </summary>
        ISprite Image { get; }
    }
}
