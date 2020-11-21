using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Zen.Input;
using Zen.MonoGameUtilities;
using Zen.MonoGameUtilities.ExtensionMethods;
using Zen.Utilities;
using Zen.Utilities.ExtensionMethods;

namespace Zen.GuiControls
{
    public abstract class Control : IControl
    {
        #region State
        public string Name { get; set; }

        private object _owner;
        public object Owner
        {
            get => _owner;
            set
            {
                ChildControls?.SetOwner(value);
                _owner = value;
            }
        }

        public IControl Parent { get; set; }

        private PointI _position;
        public PointI Position
        {
            get => _position;
            set
            {
                ChildControls?.SetTopLeftPosition(value);
                _position = value;
            }
        }

        public Alignment PositionAlignment { get; set; }

        public PointI Size { get; set; }

        public Color Color { get; set; }

        public Color BackgroundColor { get; set; }

        public Color BorderColor { get; set; }

        public bool Enabled { get; set; }

        public bool Visible { get; set; }

        public float LayerDepth { get; set; }

        public ControlStatus Status { get; set; }
        protected InputHandler Input { get; private set; }

        public Controls ChildControls { get; }

        public Packages Packages { get; }
        #endregion

        #region Accessors
        public Rectangle Bounds => ControlHelper.DetermineArea(Position.ToVector2(), PositionAlignment, Size);
        public int Top => Bounds.Top;
        public int Bottom => Bounds.Bottom;
        public int Left => Bounds.Left;
        public int Right => Bounds.Right;
        public PointI Center => new PointI(Bounds.Center.X, Bounds.Center.Y);
        public PointI TopLeft => new PointI(Left, Top);
        public PointI TopRight => new PointI(Right, Top);
        public PointI BottomLeft => new PointI(Left, Bottom);
        public PointI BottomRight => new PointI(Right, Bottom);
        public int Width => Bounds.Width;
        public int Height => Bounds.Height;

        public PointI RelativeTopLeft => new PointI(Left - (Parent?.Left ?? 0), Top - (Parent?.Top ?? 0));
        public PointI RelativeTopRight => new PointI(RelativeTopLeft.X + Width, RelativeTopLeft.Y);
        public PointI RelativeMiddleRight => new PointI(RelativeTopLeft.X + Width, RelativeTopLeft.Y + (int)(Height * 0.5f));
        public PointI RelativeBottomLeft => new PointI(RelativeTopLeft.X, RelativeTopLeft.Y + Height);

        public IControl this[int index] => ChildControls[index];
        public IControl this[string key] => ChildControls.FindControl(key);
        #endregion

        protected Control(Control other)
        {
            Name = other.Name;
            ChildControls = other.ChildControls;
            Packages = other.Packages;
            Owner = other.Owner;
            Parent = other.Parent;
            Position = other.Position;
            PositionAlignment = other.PositionAlignment;
            Size = other.Size;
            Color = other.Color;
            BackgroundColor = other.BackgroundColor;
            BorderColor = other.BorderColor;
            Status = other.Status;
            Enabled = other.Enabled;
            Visible = other.Visible;
            LayerDepth = other.LayerDepth;
        }

        protected Control(string name)
        {
            Name = name;

            LayerDepth = 0.0f;
            Position = PointI.Empty;
            PositionAlignment = Alignment.TopLeft;
            Size = PointI.Zero;
            Color = Color.White;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            Status = ControlStatus.None;
            Enabled = true;
            Visible = true;

            ChildControls = new Controls();
            Packages = new Packages();
        }

        public abstract IControl Clone();

        /// <summary>
        /// Adds packages to this control.
        /// </summary>
        /// <param name="packages">Packages to add</param>
        /// <param name="callingTypeFullName"></param>
        /// <param name="callingAssemblyFullName"></param>
        public void AddPackages(List<string> packages, string callingTypeFullName, string callingAssemblyFullName)
        {
            foreach (var package in packages)
            {
                AddPackage(package, callingTypeFullName, callingAssemblyFullName);
            }
        }

        /// <summary>
        /// Adds packages to this control.
        /// </summary>
        /// <param name="packages">Packages to add</param>
        public void AddPackages(List<IPackage> packages)
        {
            foreach (var package in packages)
            {
                AddPackage(package);
            }
        }

        /// <summary>
        /// Add a package to this control.
        /// </summary>
        /// <param name="package">Package to add. For example: 'Zen.GuiControls.PackagesClasses.ControlClick, Zen.GuiControls - Game1.EventHandlers, Game1 - ApplySettings'</param>
        /// <param name="callingTypeFullName"></param>
        /// <param name="callingAssemblyFullName"></param>
        public void AddPackage(string package, string callingTypeFullName, string callingAssemblyFullName)
        {
            var firstAndLastCharactersRemoved = package.RemoveFirstAndLastCharacters(); // remove single quotes
            var split = firstAndLastCharactersRemoved.Split('-');

            if (split.Length == 3)
            {
                var assemblyQualifiedName1 = split[1].Trim(); // Game1.EventHandlers, Game1
                var methodName = split[2].Trim(); // ApplySettings
                var action = ObjectCreator.CreateActionDelegate(assemblyQualifiedName1, methodName);

                var assemblyQualifiedName2 = split[0].Trim(); // Zen.GuiControls.PackagesClasses.ControlClick, Zen.GuiControls
                var instantiatedObject2 = ObjectCreator.CreateInstance(assemblyQualifiedName2, action);

                var packageToAdd = (IPackage) instantiatedObject2;
                AddPackage(packageToAdd);
            }
            else if (split.Length == 2)
            {
                var outerMethod = split[0].Trim(); // Zen.GuiControls.PackagesClasses.ControlClick
                var innerMethod = split[1].Trim(); // PhoenixGamePresentation.Views.SettlementViewComposite.MainFrameEventHandlers.CloseButtonClick
                split = innerMethod.Split('.');

                if (split.Length == 1)
                {
                    var methodName = split[0].Trim();
                    var assemblyQualifiedName1 = $"{callingTypeFullName}, {callingAssemblyFullName}";
                    var action = ObjectCreator.CreateActionDelegate(assemblyQualifiedName1, methodName);

                    var assemblyQualifiedName2 = outerMethod;
                    var instantiatedObject2 = ObjectCreator.CreateInstance(assemblyQualifiedName2, action);

                    var packageToAdd = (IPackage)instantiatedObject2;
                    AddPackage(packageToAdd);
                }
                else if (split.Length > 1)
                {
                    var methodName = split[^1].Trim(); // CloseButtonClick
                    var className = split[^2].Trim(); // MainFrameEventHandlers
                    var nameSpace = innerMethod.Replace($".{methodName}", string.Empty).Replace($".{className}", string.Empty); // PhoenixGamePresentation.Views.SettlementViewComposite
                    var assemblyQualifiedName1 = $"{nameSpace}.{className}, {callingAssemblyFullName}"; // PhoenixGamePresentation.Views.SettlementViewComposite.MainFrameEventHandlers, PhoenixGamePresentation
                    var action = ObjectCreator.CreateActionDelegate(assemblyQualifiedName1, methodName);

                    var assemblyQualifiedName2 = outerMethod; // Zen.GuiControls.PackagesClasses.ControlClick, Zen.GuiControls
                    var instantiatedObject2 = ObjectCreator.CreateInstance(assemblyQualifiedName2, action);

                    var packageToAdd = (IPackage)instantiatedObject2;
                    AddPackage(packageToAdd);
                }
                else
                {
                    throw new Exception($"Badly formed package string. [{package}]");
                }
            }
            else
            {
                throw new Exception($"Badly formed package string. [{package}]");
            }
        }

        /// <summary>
        /// Add a package to this control.
        /// </summary>
        /// <param name="package">Package to add</param>
        public void AddPackage(IPackage package)
        {
            Packages.Add(package);
        }

        /// <summary>
        /// Adds a child control to this control.
        /// </summary>
        /// <param name="childControl">Control to be added</param>
        /// <param name="parentAlignment">Used to determine the position of the child control in relation to the parent</param>
        /// <param name="childAlignment">Used to determine the position of the child control in relation to the parent</param>
        /// <param name="offset">Offset to be added to the child control's top left position</param>
        public void AddControl(Control childControl, Alignment parentAlignment = Alignment.TopLeft,
            Alignment childAlignment = Alignment.None, PointI offset = new PointI())
        {
            if (childAlignment == Alignment.None)
            {
                childAlignment = parentAlignment;
            }

            childControl.Parent = this;

            var topLeft = ControlHelper.DetermineTopLeft(childControl, parentAlignment, childAlignment, offset, Position.ToVector2(), PositionAlignment, Size);

            childControl.Position = topLeft;
            childControl.PositionAlignment = Alignment.TopLeft;
            ChildControls.Add(childControl.Name, childControl);
        }


        /// <summary>
        /// Change position relative to current position.
        /// </summary>
        /// <param name="point"></param>
        public void MovePosition(PointI point)
        {
            ChildControls.MoveTopLeftPosition(point);
            Position += point;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="loadChildrenContent"></param>
        public virtual void LoadContent(ContentManager content, bool loadChildrenContent = false)
        {
            if (loadChildrenContent)
            {
                ChildControls.LoadContent(content, true);
            }
        }

        public virtual void Update(InputHandler input, float deltaTime, Viewport? viewport = null)
        {
            Input = input;

            if (!Enabled)
            {
                Packages.Reset();
                Status = ControlStatus.None;
                return;
            }

            Status = Status switch
            {
                ControlStatus.None when ControlHelper.IsMouseOverControl(Bounds, input.MousePosition, viewport) => ControlStatus.MouseOver,
                ControlStatus.MouseOver when !ControlHelper.IsMouseOverControl(Bounds, input.MousePosition, viewport) => ControlStatus.None,
                _ => Status
            };

            Status = Packages.Update(this, input, deltaTime);

            ChildControls.Update(input, deltaTime, viewport);
        }

        protected virtual void InDraw(SpriteBatch spriteBatch)
        {
        }

        protected virtual void InDraw(Matrix? transform = null)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                spriteBatch.FillRectangle(Bounds, BackgroundColor, LayerDepth);

                InDraw(spriteBatch);

                spriteBatch.DrawRectangle(
                    new Rectangle(Bounds.X, Bounds.Y, Bounds.Width - 1, Bounds.Height - 1),
                    BorderColor,
                    1.0f,
                    LayerDepth);
            }

            ChildControls.Draw(spriteBatch);
        }

        public void Draw(Matrix? transform = null)
        {
            InDraw(transform);

            ChildControls.Draw(transform);
        }

        public override string ToString()
        {
            return DebuggerDisplay;
        }

        protected string DebuggerDisplay => $"{{Name={Name},TopLeftPosition={TopLeft},RelativeTopLeft={RelativeTopLeft},Size={Size}}}";
    }
}