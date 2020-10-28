//using System;
//using System.Diagnostics;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;

//namespace Zen.GuiControls
//{
//    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
//    public class LabelSized : Label
//    {
//        #region State
//        private Alignment ContentAlignment { get; }
//        #endregion

//        /// <summary>
//        /// Use this constructor if Label is to be used as a child of another control.
//        /// When a control is a child of another control, it's position will be relative
//        /// to the parent control. Therefore there is no need to pass in a position.
//        /// </summary>
//        /// <param name="name"></param>
//        /// <param name="size"></param>
//        /// <param name="contentAlignment"></param>
//        /// <param name="text"></param>
//        /// <param name="fontName"></param>
//        /// <param name="textColor"></param>
//        /// <param name="textShadowColor"></param>
//        /// <param name="backColor"></param>
//        /// <param name="borderColor"></param>
//        /// <param name="scale"></param>
//        public LabelSized(
//            string name,
//            Vector2 size,
//            Alignment contentAlignment,
//            string text,
//            string fontName,
//            Color textColor,
//            Color? textShadowColor = null,
//            Color? backColor = null,
//            Color? borderColor = null,
//            float scale = 1.0f) :
//            this(
//                Vector2.Zero,
//                Alignment.TopLeft,
//                size,
//                contentAlignment,
//                text,
//                null,
//                fontName,
//                textColor,
//                name,
//                textShadowColor,
//                backColor,
//                borderColor,
//                scale)
//        {
//        }

//        /// <summary>
//        /// Use this constructor if Label is to be used as a child of another control.
//        /// When a control is a child of another control, it's position will be relative
//        /// to the parent control. Therefore there is no need to pass in a position.
//        /// </summary>
//        /// <param name="name"></param>
//        /// <param name="size"></param>
//        /// <param name="contentAlignment"></param>
//        /// <param name="getTextFunc"></param>
//        /// <param name="fontName"></param>
//        /// <param name="textColor"></param>
//        /// <param name="textShadowColor"></param>
//        /// <param name="backColor"></param>
//        /// <param name="borderColor"></param>
//        /// <param name="scale"></param>
//        public LabelSized(
//            string name,
//            Vector2 size,
//            Alignment contentAlignment,
//            Func<string> getTextFunc,
//            string fontName,
//            Color textColor,
//            Color? textShadowColor = null,
//            Color? backColor = null,
//            Color? borderColor = null,
//            float scale = 1.0f) :
//            this(
//                Vector2.Zero,
//                Alignment.TopLeft,
//                size,
//                contentAlignment,
//                string.Empty,
//                getTextFunc,
//                fontName,
//                textColor,
//                name,
//                textShadowColor,
//                backColor,
//                borderColor,
//                scale)
//        {
//        }

//        /// <summary>
//        /// Use this constructor if Label is expected to be stand alone (have no parent).
//        /// </summary>
//        /// <param name="position"></param>
//        /// <param name="positionAlignment"></param>
//        /// <param name="size"></param>
//        /// <param name="contentAlignment"></param>
//        /// <param name="text"></param>
//        /// <param name="fontName"></param>
//        /// <param name="textColor"></param>
//        /// <param name="name"></param>
//        /// <param name="textShadowColor"></param>
//        /// <param name="backColor"></param>
//        /// <param name="borderColor"></param>
//        /// <param name="scale"></param>
//        public LabelSized(
//            Vector2 position,
//            Alignment positionAlignment,
//            Vector2 size,
//            Alignment contentAlignment,
//            string text,
//            string fontName,
//            Color textColor,
//            string name,
//            Color? textShadowColor = null,
//            Color? backColor = null,
//            Color? borderColor = null,
//            float scale = 1.0f) :
//            this(
//                position,
//                positionAlignment,
//                size,
//                contentAlignment,
//                text,
//                null,
//                fontName,
//                textColor,
//                name,
//                textShadowColor,
//                backColor,
//                borderColor,
//                scale)
//        {
//        }

//        /// <summary>
//        /// Use this constructor if Label is expected to be stand alone (have no parent).
//        /// </summary>
//        /// <param name="position"></param>
//        /// <param name="positionAlignment"></param>
//        /// <param name="size"></param>
//        /// <param name="contentAlignment"></param>
//        /// <param name="text"></param>
//        /// <param name="fontName"></param>
//        /// <param name="textColor"></param>
//        /// <param name="name"></param>
//        /// <param name="textShadowColor"></param>
//        /// <param name="scale"></param>
//        public LabelSized(
//            Vector2 position,
//            Alignment positionAlignment,
//            Vector2 size,
//            Alignment contentAlignment,
//            string text,
//            string fontName,
//            Color textColor,
//            string name,
//            Color? textShadowColor = null,
//            float scale = 1.0f) :
//            this(
//                position,
//                positionAlignment,
//                size,
//                contentAlignment,
//                text,
//                null,
//                fontName,
//                textColor,
//                name,
//                textShadowColor,
//                null,
//                null,
//                scale)
//        {
//        }

//        private LabelSized(
//            Vector2 position,
//            Alignment positionAlignment,
//            Vector2 size,
//            Alignment contentAlignment,
//            string text,
//            Func<string> getTextFunc,
//            string fontName,
//            Color textColor,
//            string name,
//            Color? textShadowColor = null,
//            Color? backColor = null,
//            Color? borderColor = null,
//            float scale = 1.0f,
//            float layerDepth = 0.0f) :
//            base(
//                position,
//                positionAlignment,
//                size,
//                text,
//                getTextFunc,
//                fontName,
//                textColor,
//                name,
//                textShadowColor,
//                backColor,
//                borderColor,
//                scale,
//                layerDepth)
//        {
//            ContentAlignment = contentAlignment;
//        }

//        protected override Vector2 DetermineOffset(SpriteFont font, Vector2 size, string text, float scale)
//        {
//            var textSize = font.MeasureString(text) * scale;

//            return ContentAlignment switch
//            {
//                Alignment.TopLeft => Vector2.Zero,
//                Alignment.TopCenter => new Vector2((size.X - textSize.X) * 0.5f, 0.0f),
//                Alignment.TopRight => new Vector2(size.X - textSize.X, 0.0f),
//                Alignment.MiddleLeft => new Vector2(0.0f, (size.Y - textSize.Y) * 0.5f),
//                Alignment.MiddleCenter => new Vector2((size.X - textSize.X) * 0.5f, (size.Y - textSize.Y) * 0.5f),
//                Alignment.MiddleRight => new Vector2(size.X - textSize.X, (size.Y - textSize.Y) * 0.5f),
//                Alignment.BottomLeft => new Vector2(0.0f, size.Y - textSize.Y),
//                Alignment.BottomCenter => new Vector2((size.X - textSize.X) * 0.5f, size.Y - textSize.Y),
//                Alignment.BottomRight => new Vector2(size.X - textSize.X, size.Y - textSize.Y),
//                _ => throw new Exception($"ContentAlignment [{ContentAlignment}] is not implemented."),
//            };
//        }
//    }
//}