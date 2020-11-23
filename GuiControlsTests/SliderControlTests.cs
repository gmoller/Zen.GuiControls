using System;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using Zen.GuiControls;
using Zen.GuiControls.TheControls;
using Zen.Utilities;

namespace Zen.GuiControlsTests
{
    public class SliderControlTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Declarative_slider_control_creation()
        {
            var spec = @"
slrTest : Slider
{
  Position: [50;10]
  PositionAlignment: MiddleCenter
  Size: [100;20]
  Color: Blue
  BackgroundColor: Green
  BorderColor: Red
  Enabled: false
  Visible: false
  LayerDepth: 0.1

  GripSize: [5;5]
  TextureName: 'GUI_Textures_1.texture1'
  TextureGripNormal: 'GUI_Textures_1.texture2'
  MinimumValue: 0
  MaximumValue: 100
  CurrentValue: 0

  OnDrag: 'UpdateSliderCurrentValue'
}";
            var controls = ControlCreator.CreateFromSpecification(spec);
            controls.SetOwner(this);

            Assert.AreEqual(1, controls.Count);
            var ctrl1 = controls["slrTest"];
            Assert.IsTrue(ctrl1 is Slider);
            Assert.IsTrue(ctrl1.Owner is SliderControlTests);
            Assert.AreEqual("slrTest", ctrl1.Name);
            Assert.AreEqual(null, ctrl1.Parent);
            Assert.AreEqual(new Rectangle(0, 0, 100, 20), ctrl1.Bounds);
            Assert.AreEqual(Color.Blue, ctrl1.Color);
            Assert.AreEqual(Color.Green, ctrl1.BackgroundColor);
            Assert.AreEqual(Color.Red, ctrl1.BorderColor);
            Assert.AreEqual(false, ctrl1.Enabled);
            Assert.AreEqual(false, ctrl1.Visible);
            Assert.AreEqual(0.1f, ctrl1.LayerDepth);

            var slider1 = (Slider)ctrl1;
            Assert.AreEqual(new PointI(5, 5), slider1.GripSize);
            Assert.AreEqual("GUI_Textures_1.texture1", slider1.TextureName);
            Assert.AreEqual("GUI_Textures_1.texture2", slider1.TextureGripNormal);
            Assert.AreEqual(0, slider1.MinimumValue);
            Assert.AreEqual(100, slider1.MaximumValue);
            Assert.AreEqual(0, slider1.CurrentValue);

            //var input = new InputHandler();
            //slider1.Update(input, 0.0f);
        }

        public static void UpdateSliderCurrentValue(object sender, EventArgs args)
        {
        }
    }
}