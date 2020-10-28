using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Zen.Utilities;

namespace Zen.GuiControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class DynamicSlots : ControlWithSingleTexture
    {
        #region State
        private readonly int _numberOfSlotsX;
        private readonly int _numberOfSlotsY;
        private readonly float _slotPadding;
        #endregion

        /// <summary>
        /// Use this constructor if DynamicSlots is to be used as a child of another control.
        /// When a control is a child of another control, it's position will be relative
        /// to the parent control. Therefore there is no need to pass in a position.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="size"></param>
        /// <param name="textureName"></param>
        /// <param name="numberOfSlotsX"></param>
        /// <param name="numberOfSlotsY"></param>
        /// <param name="slotPadding"></param>
        public DynamicSlots(
            string name,
            Vector2 size,
            string textureName,
            int numberOfSlotsX,
            int numberOfSlotsY,
            float slotPadding) :
            this(
                Vector2.Zero,
                Alignment.TopLeft,
                size,
                textureName,
                numberOfSlotsX,
                numberOfSlotsY,
                slotPadding,
                name,
                0.0f)
        {
        }

        /// <summary>
        /// Use this constructor if DynamicSlots is expected to be stand alone (have no parent).
        /// </summary>
        /// <param name="position"></param>
        /// <param name="positionAlignment"></param>
        /// <param name="size"></param>
        /// <param name="textureName"></param>
        /// <param name="numberOfSlotsX"></param>
        /// <param name="numberOfSlotsY"></param>
        /// <param name="slotPadding"></param>
        /// <param name="name"></param>
        public DynamicSlots(
            Vector2 position,
            Alignment positionAlignment,
            Vector2 size,
            string textureName,
            int numberOfSlotsX,
            int numberOfSlotsY,
            float slotPadding,
            string name) :
            this(
                position,
                positionAlignment,
                size,
                textureName,
                numberOfSlotsX,
                numberOfSlotsY,
                slotPadding,
                name,
                0.0f)
        {
        }

        private DynamicSlots(
            Vector2 position,
            Alignment positionAlignment,
            Vector2 size,
            string textureName,
            int numberOfSlotsX,
            int numberOfSlotsY,
            float slotPadding,
            string name,
            float layerDepth = 0.0f) :
            base(textureName, name)
        {
            _numberOfSlotsX = numberOfSlotsX;
            _numberOfSlotsY = numberOfSlotsY;
            _slotPadding = slotPadding;

            var startX = TopLeft.X + _slotPadding;
            var startY = TopLeft.Y + _slotPadding;
            var slotWidth = (Size.X - _slotPadding * 2.0f) / _numberOfSlotsX;
            var slotHeight = (Size.Y - _slotPadding * 2.0f) / _numberOfSlotsY;
            CreateSlots(new Vector2(startX, startY), new Vector2(slotWidth, slotHeight), _numberOfSlotsX, _numberOfSlotsY);
        }

        public override void LoadContent(ContentManager content, bool loadChildrenContent = false)
        {
            base.LoadContent(content, loadChildrenContent);

            ChildControls.LoadChildControls(content, loadChildrenContent);
        }

        private void CreateSlots(Vector2 startPosition, Vector2 size, int numberOfSlotsX, int numberOfSlotsY)
        {
            var x = startPosition.X;
            var y = startPosition.Y;

            for (var j = 0; j < numberOfSlotsY; ++j)
            {
                for (var i = 0; i < numberOfSlotsX; ++i)
                {
                    AddControl(new Slot($"slot[{i}.{j}]", size, $"{TextureAtlas}.{TextureName}"), Alignment.TopLeft, Alignment.TopLeft, new PointI((int)x, (int)y));
                    x += size.X;
                }

                x = startPosition.X;
                y += size.Y;
            }
        }
    }
}