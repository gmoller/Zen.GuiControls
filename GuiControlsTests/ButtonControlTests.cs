﻿using Microsoft.Xna.Framework;
using NUnit.Framework;
using Zen.GuiControls;
using Zen.GuiControls.TheControls;

namespace Zen.GuiControlsTests
{
    public class ButtonControlTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Declarative_button_control_creation()
        {
            var spec = @"
btnTest : Button
{
  Position: [100;100]
  PositionAlignment: MiddleCenter
  Size: [100;25]
  Color: Blue
  BackgroundColor: Green
  BorderColor: Red
  Enabled: false
  Visible: false
  LayerDepth: 0.1
  TextureActive: 'GUI_Textures_1.active'
  TextureHover: 'GUI_Textures_1.hover'
  TextureNormal: 'GUI_Textures_1.normal'
  TextureDisabled: 'GUI_Textures_1.disabled'
}";
            var controls = ControlCreator.CreateFromSpecification(spec);
            controls.SetOwner(this);

            Assert.AreEqual(1, controls.Count);
            var ctrl1 = controls["btnTest"];
            ctrl1.Status = ControlStatus.Active | ControlStatus.HasFocus;
            Assert.IsTrue(ctrl1 is Button);
            Assert.IsTrue(ctrl1.Owner is ButtonControlTests);
            Assert.AreEqual("btnTest", ctrl1.Name);
            Assert.AreEqual(null, ctrl1.Parent);
            Assert.AreEqual(new Rectangle(50, 87, 100, 25), ctrl1.Bounds);
            Assert.AreEqual(Color.Blue, ctrl1.Color);
            Assert.AreEqual(Color.Green, ctrl1.BackgroundColor);
            Assert.AreEqual(Color.Red, ctrl1.BorderColor);
            Assert.AreEqual(true, ctrl1.Status.HasFlag(ControlStatus.Active));
            Assert.AreEqual(true, ctrl1.Status.HasFlag(ControlStatus.HasFocus));
            Assert.AreEqual(false, ctrl1.Status.HasFlag(ControlStatus.Disabled));
            Assert.AreEqual(false, ctrl1.Visible);
            Assert.AreEqual(0.1f, ctrl1.LayerDepth);

            var button1 = (Button)ctrl1;
            Assert.AreEqual("GUI_Textures_1.active", button1.TextureActive);
            Assert.AreEqual("GUI_Textures_1.hover", button1.TextureHover);
            Assert.AreEqual("GUI_Textures_1.normal", button1.TextureNormal);
            Assert.AreEqual("GUI_Textures_1.disabled", button1.TextureDisabled);
        }
    }
}