using System;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using Zen.GuiControls;

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
  TextureName: 'GUI_Textures_1.texture1'
  Position: [1680;0]
  Size: [100;25]
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
            Assert.AreEqual(new Rectangle(1680, 0, 100, 25), ctrl1.Area);

            var slider1 = (Slider)ctrl1;
            Assert.AreEqual("GUI_Textures_1", slider1.TextureAtlas);
            Assert.AreEqual("texture1", slider1.TextureName);
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