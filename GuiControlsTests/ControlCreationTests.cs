using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using Zen.GuiControls;
using Zen.Utilities;

namespace Zen.GuiControlsTests
{
    public class ControlCreationTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Very_basic_declarative_control_creation()
        {
            var spec = @"
frmTest : Frame
{
  TextureName: GUI_Textures_1.frame_texture
  Position: 1680;0
  Size: 100;100
  TopPadding: 5
  BottomPadding: 5
  LeftPadding: 5
  RightPadding: 5
}";
            var controls = ControlCreator.CreateFromSpecification(spec);
            controls.SetOwner(this);

            Assert.AreEqual(1, controls.Count);
            var ctrl1 = controls[0];
            var ctrl2 = controls["frmTest"];
            Assert.IsTrue(ctrl1 == ctrl2);
            Assert.IsTrue(ctrl1 is Frame);
            Assert.IsTrue(ctrl1.Owner is ControlCreationTests);
            Assert.AreEqual("frmTest", ctrl1.Name);
            Assert.AreEqual(null, ctrl1.Parent);
            Assert.AreEqual(new Rectangle(1680, 0, 100, 100), ctrl1.Area);
            Assert.AreEqual(new PointI(1680, 0), ctrl1.TopLeft);
            Assert.AreEqual(new PointI(1780, 0), ctrl1.TopRight);
            Assert.AreEqual(new PointI(1680, 100), ctrl1.BottomLeft);
            Assert.AreEqual(new PointI(1780, 100), ctrl1.BottomRight);
        }

        [Test]
        public void Nested_declarative_control_creation()
        {
            var spec = @"
frmTest : Frame
{
  TextureName: GUI_Textures_1.frame_texture
  Position: %position1%
  Size: 100;100
  TopPadding: 5
  BottomPadding: 5
  LeftPadding: 5
  RightPadding: 5

  Contains: [lblTest]
}

lblTest : Label
{
  FontName: Arial
  Size: 100;15
  ContentAlignment: TopLeft
  Text: Hello
  TextColor: Yellow

  ParentContainerAlignment: ParentTopLeftAlignsWithChildTopLeft
  Offset: 20;20
}";
            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("position1", "1680;0")
            };

            var controls = ControlCreator.CreateFromSpecification(spec, pairs);
            controls.SetOwner(this);

            Assert.AreEqual(1, controls.Count);
            var ctrl1 = controls[0];
            var ctrl2 = controls["frmTest"];
            var ctrl3 = controls["frmTest.lblTest"];
            Assert.IsTrue(ctrl1 == ctrl2);
            Assert.IsTrue(ctrl1 is Frame);
            Assert.IsTrue(ctrl1.Owner is ControlCreationTests);
            Assert.AreEqual("frmTest", ctrl1.Name);
            Assert.AreEqual(null, ctrl1.Parent);
            Assert.AreEqual(new Rectangle(1680, 0, 100, 100), ctrl1.Area);
            Assert.AreEqual(new PointI(1680, 0), ctrl1.TopLeft);
            Assert.AreEqual(new PointI(1780, 0), ctrl1.TopRight);
            Assert.AreEqual(new PointI(1680, 100), ctrl1.BottomLeft);
            Assert.AreEqual(new PointI(1780, 100), ctrl1.BottomRight);

            Assert.IsTrue(ctrl3 is Label);
            Assert.IsTrue(ctrl3.Owner is ControlCreationTests);
            Assert.AreEqual("lblTest", ctrl3.Name);
            Assert.AreEqual(ctrl1, ctrl3.Parent);
            Assert.AreEqual(ctrl2, ctrl3.Parent);
            Assert.AreEqual(new Rectangle(1700, 20, 100, 15), ctrl3.Area);
            Assert.AreEqual(new PointI(1700, 20), ctrl3.TopLeft);
            Assert.AreEqual(new PointI(1800, 20), ctrl3.TopRight);
            Assert.AreEqual(new PointI(1700, 35), ctrl3.BottomLeft);
            Assert.AreEqual(new PointI(1800, 35), ctrl3.BottomRight);
        }
    }
}