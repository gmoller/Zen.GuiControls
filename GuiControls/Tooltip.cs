using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Zen.MonoGameUtilities.ExtensionMethods;
using Zen.Utilities;

namespace Zen.GuiControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Tooltip : ControlWithSingleTexture
    {
        #region State
        private bool IsHoveringOver { get; set; }
        private DateTime TimeStartedHovering { get; set; }
        #endregion

        public Tooltip(Vector2 position, Alignment positionAlignment, Vector2 size, string textureName, int topPadding, int bottomPadding, int leftPadding, int rightPadding, string name)
            : base(textureName, name)
        {
            var frame = new Frame("frame", textureName, topPadding, bottomPadding, leftPadding, rightPadding)
            {
                PositionAlignment = positionAlignment,
                Size = size.ToPointI()
            };
            frame.SetPosition(position.ToPointI());
            AddControl(frame);
            frame.AddControl(new Image("TransparentBackground", "background"), Alignment.MiddleCenter, Alignment.MiddleCenter);
            //frame.AddControl(new Label("lblId", new Vector2(100.0f, 30.0f), "Id:", "Arial-12", Color.Yellow), Alignment.TopLeft, Alignment.TopLeft, new PointI(15, 15));
            //frame.AddControl(new Label("lblState", new Vector2(100.0f, 30.0f), "State:", "Arial-12", Color.Yellow), Alignment.TopLeft, Alignment.TopLeft, new PointI(15, 45));
            //frame.AddControl(new Label("lblStackStatus", new Vector2(100.0f, 30.0f), "StackStatus:", "Arial-12", Color.Yellow), Alignment.TopLeft, Alignment.TopLeft, new PointI(15, 75));
            //frame.AddControl(new Label("lblIsSelected", new Vector2(100.0f, 30.0f), "IsSelected:", "Arial-12", Color.Yellow), Alignment.TopLeft, Alignment.TopLeft, new PointI(15, 105));
            //frame.AddControl(new Label("lblOrdersGiven", new Vector2(100.0f, 30.0f), "OrdersGiven:", "Arial-12", Color.Yellow), Alignment.TopLeft, Alignment.TopLeft, new PointI(15, 135));
            //frame.AddControl(new Label("lblMovementPoints", new Vector2(100.0f, 30.0f), "MovementPoints:", "Arial-12", Color.Yellow), Alignment.TopLeft, Alignment.TopLeft, new PointI(15, 165));
            //frame.AddControl(new Label("lblCurrent", new Vector2(100.0f, 30.0f), "Current:", "Arial-12", Color.Yellow), Alignment.TopLeft, Alignment.TopLeft, new PointI(15, 195));
        }

        public override void LoadContent(ContentManager content, bool loadChildrenContent = false)
        {
            this["frame"].LoadContent(content, loadChildrenContent);
            //this["frame.background"].LoadContent(content, loadChildrenContent);
            //this["frame.lblId"].LoadContent(content, loadChildrenContent);
            //this["frame.lblState"].LoadContent(content, loadChildrenContent);
            //this["frame.lblStackStatus"].LoadContent(content, loadChildrenContent);
            //this["frame.lblIsSelected"].LoadContent(content, loadChildrenContent);
            //this["frame.lblOrdersGiven"].LoadContent(content, loadChildrenContent);
            //this["frame.lblMovementPoints"].LoadContent(content, loadChildrenContent);
            //this["frame.lblCurrent"].LoadContent(content, loadChildrenContent);
        }

        public override void SetPosition(PointI point)
        {
            base.SetPosition(point);

            this["frame"].SetPosition(point);
        }

        public bool StartHover()
        {
            if (IsHoveringOver)
            {
                return DateTime.Now > TimeStartedHovering.AddMilliseconds(500); // TODO: make configurable
            }

            IsHoveringOver = true;
            TimeStartedHovering = DateTime.Now;

            return false;
        }

        public void StopHover()
        {
            IsHoveringOver = false;
            TimeStartedHovering = DateTime.MinValue;
        }

        public void SetText()
        {
            //this["frame.lblId"].SetText($"Id: {stackView.Id}");
            //this["frame.lblState"].SetText($"State: {stackView.StackViewState}");
            //this["frame.lblStackStatus"].SetText($"StackStatus: {stackView.Stack.Status}");
            //this["frame.lblIsSelected"].SetText($"IsSelected: {stackView.IsSelected}");
            //this["frame.lblOrdersGiven"].SetText($"OrdersGiven: {stackView.OrdersGiven}");
            //this["frame.lblMovementPoints"].SetText($"MovementPoints: {stackView.MovementPoints}");
            //this["frame.lblCurrent"].SetText($"Current: {StackViews.Current}");
        }
    }
}