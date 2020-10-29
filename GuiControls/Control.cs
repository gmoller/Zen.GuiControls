﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Zen.Input;
using Zen.MonoGameUtilities.ExtensionMethods;
using Zen.Utilities;

namespace Zen.GuiControls
{
    public abstract class Control : IControl
    {
        #region State
        public int Id { get; }
        public string Name { get; }

        public ControlStatus Status { get; set; }
        public bool Enabled { get; set; }
        public Controls ChildControls { get; }

        /// <summary>
        /// Qualifies the position, is that position TopLeft, Center, etc. ?
        /// </summary>
        public Alignment PositionAlignment { get; set; }

        /// <summary>
        /// Size of control in pixels.
        /// </summary>
        public PointI Size { get; set; }
        public IControl Parent { get; set; }
        public float LayerDepth { get; set; }

        /// <summary>
        /// Position of control.
        /// </summary>
        private Vector2 Position { get; set; }
        private Packages Packages { get; }
        #endregion

        protected Control(string name)
        {
            Name = name;
            LayerDepth = 0.0f;

            Position = Vector2.Zero;
            PositionAlignment = Alignment.TopLeft;
            Size = PointI.Zero;

            Status = ControlStatus.None;
            Enabled = true;

            ChildControls = new Controls();
            Packages = new Packages();
        }

        #region Accessors
        protected Rectangle ActualDestinationRectangle => ControlHelper.DetermineArea(Position, PositionAlignment, Size);
        public int Top => ActualDestinationRectangle.Top;
        public int Bottom => ActualDestinationRectangle.Bottom;
        public int Left => ActualDestinationRectangle.Left;
        public int Right => ActualDestinationRectangle.Right;
        public PointI Center => new PointI(ActualDestinationRectangle.Center.X, ActualDestinationRectangle.Center.Y);
        public PointI TopLeft => new PointI(Left, Top);
        public PointI TopRight => new PointI(Right, Top);
        public PointI BottomLeft => new PointI(Left, Bottom);
        public PointI BottomRight => new PointI(Right, Bottom);
        public int Width => ActualDestinationRectangle.Width;
        public int Height => ActualDestinationRectangle.Height;

        public Rectangle Area => ActualDestinationRectangle; // TODO

        public PointI RelativeTopLeft => new PointI(Left - (Parent?.Left ?? 0), Top - (Parent?.Top ?? 0));
        public PointI RelativeTopRight => new PointI(RelativeTopLeft.X + Width, RelativeTopLeft.Y);
        public PointI RelativeMiddleRight => new PointI(RelativeTopLeft.X + Width, RelativeTopLeft.Y + (int)(Height * 0.5f));
        public PointI RelativeBottomLeft => new PointI(RelativeTopLeft.X, RelativeTopLeft.Y + Height);

        public IControl this[int index] => ChildControls[index];
        public IControl this[string key] => ChildControls.FindControl(key);
        #endregion

        /// <summary>
        /// Adds packages to this control.
        /// </summary>
        /// <param name="packages">Packages to add</param>
        public void AddPackages(List<string> packages)
        {
            foreach (var package in packages)
            {
                AddPackage(package);
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
        public void AddPackage(string package)
        {
            var pack = package.Split('-');

            var assemblyQualifiedName1 = pack[1].Trim(); // Game1.EventHandlers, Game1
            var methodName = pack[2].Trim(); // ApplySettings
            var action = ObjectCreator.CreateActionDelegate(assemblyQualifiedName1, methodName);

            var assemblyQualifiedName2 = pack[0].Trim(); // Zen.GuiControls.PackagesClasses.ControlClick, Zen.GuiControls
            var instantiatedObject2 = ObjectCreator.CreateInstance(assemblyQualifiedName2, action);

            var packageToAdd = (IPackage)instantiatedObject2;
            AddPackage(packageToAdd);
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

            var topLeft = ControlHelper.DetermineTopLeft(childControl, parentAlignment, childAlignment, offset, Position, PositionAlignment, Size);

            childControl.SetPosition(topLeft);
            childControl.PositionAlignment = Alignment.TopLeft;
            ChildControls.Add(childControl.Name, childControl);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        public virtual void SetPosition(PointI point)
        {
            ChildControls.SetTopLeftPosition(point);
            Position = point.ToVector2();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        public void MovePosition(PointI point)
        {
            ChildControls.MoveTopLeftPosition(point);
            Position = new Vector2(Position.X + point.X, Position.Y + point.Y);
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
                ChildControls.LoadChildControls(content, true);
            }
        }

        public virtual void Update(InputHandler input, float deltaTime, Viewport? viewport = null)
        {
            if (!Enabled)
            {
                Packages.Reset();
                Status = ControlStatus.None;
                return;
            }

            Status = Status switch
            {
                ControlStatus.None when ControlHelper.IsMouseOverControl(ActualDestinationRectangle, input.MousePosition, viewport) => ControlStatus.MouseOver,
                ControlStatus.MouseOver when !ControlHelper.IsMouseOverControl(ActualDestinationRectangle, input.MousePosition, viewport) => ControlStatus.None,
                _ => Status
            };

            Status = Packages.Process(this, input, deltaTime);

            ChildControls.UpdateChildControls(input, deltaTime, viewport);
        }

        protected virtual void InDraw(SpriteBatch spriteBatch)
        {
        }

        protected virtual void InDraw(Matrix? transform = null)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            InDraw(spriteBatch);

            ChildControls.DrawChildControls(spriteBatch);
        }

        public void Draw(Matrix? transform = null)
        {
            InDraw(transform);

            ChildControls.DrawChildControls(transform);
        }

        public override string ToString()
        {
            return DebuggerDisplay;
        }

        protected string DebuggerDisplay => $"{{Name={Name},TopLeftPosition={TopLeft},RelativeTopLeft={RelativeTopLeft},Size={Size}}}";
    }
}