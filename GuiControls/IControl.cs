using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Zen.Input;
using Zen.Utilities;

namespace Zen.GuiControls
{
    public interface IControl
    {
        /// <summary>
        /// Name of the control.
        /// This will be used as a dictionary key if control is added to a parent.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Reference to class that owns this control.
        /// </summary>
        object Owner { get; set; }

        /// <summary>
        /// Parent control that "owns" this control.
        /// </summary>
        IControl Parent { get; set; }

        /// <summary>
        /// Position on screen of the control.
        /// </summary>
        PointI Position { get; set; }

        /// <summary>
        /// Qualifies the position, is that position TopLeft, Center, etc. ?
        /// </summary>
        public Alignment PositionAlignment { get; set; }

        /// <summary>
        /// Size (in pixels) of the control.
        /// </summary>
        PointI Size { get; set; }

        /// <summary>
        /// Color of the control.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Background color of the control.
        /// </summary>
        public Color BackgroundColor { get; set; }

        /// <summary>
        /// Border color of the control.
        /// </summary>
        public Color BorderColor { get; set; }

        /// <summary>
        /// Is enabled?
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Is drawn to the screen?
        /// </summary>
        bool Visible { get; set; }

        public float LayerDepth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        ControlStatus Status { get; set; }

        /// <summary>
        /// Enumerable list of all child controls "owned" by this control.
        /// </summary>
        Controls ChildControls { get; }

        /// <summary>
        /// 
        /// </summary>
        Packages Packages { get; }

        /// <summary>
        /// Indexer by index to get a child control.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Child control</returns>
        IControl this[int index] { get; }
        
        /// <summary>
        /// Indexer by key to get a child control (Name of child control).
        /// Note: A syntax like OuterFrame["InnerFrame.PrettyLabel.HappyImage"] can be used to get child
        /// controls within child controls. This will return 'HappyImage' within the control hierarchy.
        /// </summary>
        /// <param name="key">Key child control name)</param>
        /// <returns>Child control</returns>
        IControl this[string key] { get; }

        /// <summary>
        /// Position of this controls Top, relative to the top of the screen.
        /// </summary>
        int Top { get; }

        /// <summary>
        /// Position of this controls Bottom, relative to the top of the screen.
        /// </summary>
        int Bottom { get; }

        /// <summary>
        /// Position of this controls Left, relative to the left of the screen.
        /// </summary>
        int Left { get; }

        /// <summary>
        /// Position of this controls Right, relative to the left of the screen.
        /// </summary>
        int Right { get; }

        /// <summary>
        /// Position of this controls Center, relative to the top left of the screen.
        /// </summary>
        PointI Center { get; }

        /// <summary>
        /// Position of this controls TopLeft point, relative to the top left of the screen.
        /// </summary>
        PointI TopLeft { get; }

        /// <summary>
        /// Position of this controls TopRight point, relative to the top left of the screen.
        /// </summary>
        PointI TopRight { get; }

        /// <summary>
        /// Position of this controls BottomLeft point, relative to the top left of the screen.
        /// </summary>
        PointI BottomLeft { get; }

        /// <summary>
        /// Position of this controls BottomRight point, relative to the top left of the screen.
        /// </summary>
        PointI BottomRight { get; }

        Rectangle Bounds { get; }

        /// <summary>
        /// Position of this controls TopLeft point, relative to the TopLeft of the parent control.
        /// </summary>
        PointI RelativeTopLeft { get; }

        /// <summary>
        /// Position of this controls TopRight point, relative to the TopLeft of the parent control.
        /// </summary>
        PointI RelativeTopRight { get; }

        /// <summary>
        /// Position of this controls MiddleRight point, relative to the TopLeft of the parent control.
        /// </summary>
        PointI RelativeMiddleRight { get; }

        /// <summary>
        /// Position of this controls BottomLeft point, relative to the TopLeft of the parent control.
        /// </summary>
        PointI RelativeBottomLeft { get; }

        int Width { get; }
        int Height { get; }

        void AddPackage(IPackage package);
        void AddPackages(List<string> packages, string callingTypeFullName, string callingAssemblyFullName);
        void AddControl(Control childControl, Alignment parentAlignment = Alignment.TopLeft, Alignment childAlignment = Alignment.None, PointI offset = new PointI());

        void MovePosition(PointI point);

        void LoadContent(ContentManager content, bool loadChildrenContent = false);
        void Update(InputHandler input, float deltaTime, Viewport? viewport = null);
        void Draw(Matrix? transform = null);
        void Draw(SpriteBatch spriteBatch);
    }
}