using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Zen.MonoGameUtilities.ExtensionMethods;
using Zen.Utilities;

namespace Zen.GuiControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class DynamicSlots : ControlWithSingleTexture
    {
        #region State
        private int NumberOfSlotsX { get; }
        private int NumberOfSlotsY { get; }
        private float SlotPadding { get; }
        #endregion

        /// <summary>
        /// A nice little dynamicslot.
        /// </summary>
        /// <param name="name">Name of control.</param>
        /// <param name="textureName">Texture to use for control.</param>
        /// <param name="numberOfSlotsX">Number of columns.</param>
        /// <param name="numberOfSlotsY">Number of rows.</param>
        /// <param name="slotPadding">Number of pixels between slots.</param>
        public DynamicSlots(
            string name,
            string textureName,
            int numberOfSlotsX,
            int numberOfSlotsY,
            float slotPadding) :
            base(name, textureName)
        {
            NumberOfSlotsX = numberOfSlotsX;
            NumberOfSlotsY = numberOfSlotsY;
            SlotPadding = slotPadding;

            var startX = TopLeft.X + SlotPadding;
            var startY = TopLeft.Y + SlotPadding;
            var slotWidth = (Size.X - SlotPadding * 2.0f) / NumberOfSlotsX;
            var slotHeight = (Size.Y - SlotPadding * 2.0f) / NumberOfSlotsY;
            CreateSlots(new Vector2(startX, startY), new Vector2(slotWidth, slotHeight), NumberOfSlotsX, NumberOfSlotsY);
        }

        public override void LoadContent(ContentManager content, bool loadChildrenContent = false)
        {
            base.LoadContent(content, loadChildrenContent);

            ChildControls.LoadContent(content, loadChildrenContent);
        }

        private void CreateSlots(Vector2 startPosition, Vector2 size, int numberOfSlotsX, int numberOfSlotsY)
        {
            var x = startPosition.X;
            var y = startPosition.Y;

            for (var j = 0; j < numberOfSlotsY; ++j)
            {
                for (var i = 0; i < numberOfSlotsX; ++i)
                {
                    var slot = new Slot($"slot[{i}.{j}]", $"{TextureAtlas}.{TextureName}") { Size = size.ToPointI() };
                    AddControl(slot, Alignment.TopLeft, Alignment.TopLeft, new PointI((int)x, (int)y));
                    x += size.X;
                }

                x = startPosition.X;
                y += size.Y;
            }
        }
    }
}