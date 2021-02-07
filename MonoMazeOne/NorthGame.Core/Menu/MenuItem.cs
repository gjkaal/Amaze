// Rapbit Game development
//

using NorthGame.Core.Abstractions;
using NorthGame.Core.Game;

namespace NorthGame.Core.Menu
{
    public class MenuItem : IMenuItem
    {
        /// <summary>
        /// Linked to a screen or a menu.
        /// TODO: Use enum for link type
        /// </summary>
        public string LinkType { get; set; }

        /// <summary>
        /// Reference to the linked item.
        /// </summary>
        public string LinkId { get; set; }

        /// <summary>
        /// Display element for the menu
        /// </summary>
        public ISprite Image { get; set; } = new Sprite();
    }
}
