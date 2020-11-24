using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using Zen.GuiControls;
using Zen.GuiControls.TheControls;
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
  TextureNormal: 'GUI_Textures_1.frame_texture'
  Position: [1680;0]
  Size: [100;100]
  BorderSize: 5
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
            Assert.AreEqual(new Rectangle(1680, 0, 100, 100), ctrl1.Bounds);
            Assert.AreEqual(new PointI(1680, 0), ctrl1.TopLeft);
            Assert.AreEqual(new PointI(1780, 0), ctrl1.TopRight);
            Assert.AreEqual(new PointI(1680, 100), ctrl1.BottomLeft);
            Assert.AreEqual(new PointI(1780, 100), ctrl1.BottomRight);

            var frame1 = (Frame)ctrl1;
            Assert.AreEqual("GUI_Textures_1.frame_texture", frame1.TextureNormal);
        }

        [Test]
        public void Nested_declarative_control_creation()
        {
            var spec = @"
frmTest : Frame
{
  TextureName: 'GUI_Textures_1.frame_texture'
  Position: %position1%
  Size: [100;100]
  BorderSizeTop: 5
  BorderSizeBottom: 5
  BorderSizeLeft: 5
  BorderSizeRight: 5

  Contains: [lblTest]
}

lblTest : Label
{
  FontName: 'Arial'
  Size: [100;15]
  ContentAlignment: TopLeft
  Text: 'Hello'
  Color: Yellow

  ParentContainerAlignment: ParentTopLeftAlignsWithChildTopLeft
  Offset: [20;20]
}";
            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("position1", "[1680;0]")
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
            Assert.AreEqual(new Rectangle(1680, 0, 100, 100), ctrl1.Bounds);
            Assert.AreEqual(new PointI(1680, 0), ctrl1.TopLeft);
            Assert.AreEqual(new PointI(1780, 0), ctrl1.TopRight);
            Assert.AreEqual(new PointI(1680, 100), ctrl1.BottomLeft);
            Assert.AreEqual(new PointI(1780, 100), ctrl1.BottomRight);

            Assert.IsTrue(ctrl3 is Label);
            Assert.IsTrue(ctrl3.Owner is ControlCreationTests);
            Assert.AreEqual("lblTest", ctrl3.Name);
            Assert.AreEqual(ctrl1, ctrl3.Parent);
            Assert.AreEqual(ctrl2, ctrl3.Parent);
            Assert.AreEqual(new Rectangle(1700, 20, 100, 15), ctrl3.Bounds);
            Assert.AreEqual(new PointI(1700, 20), ctrl3.TopLeft);
            Assert.AreEqual(new PointI(1800, 20), ctrl3.TopRight);
            Assert.AreEqual(new PointI(1700, 35), ctrl3.BottomLeft);
            Assert.AreEqual(new PointI(1800, 35), ctrl3.BottomRight);
        }

        [Test]
        public void Control_templates_can_be_used()
        {
            var spec = @"
lbl : <Label>
{
  FontName: 'Arial'
}

lblTemplate1 : <lbl>
{
  Position: [50;50]
  Size: [100;15]
  ContentAlignment: TopLeft
  Text: 'Hello'
  Color: Yellow
}

lblTemplate2 : <lblTemplate1>
{
  BorderColor: Red
}

lblTest1 : lblTemplate1
{
}

lblTest2 : lblTemplate2
{
}

lblTest3 : lblTemplate1
{
  Text: 'Goodbye'
  BorderColor: Red
}";

            var controls = ControlCreator.CreateFromSpecification(spec);
            controls.SetOwner(this);

            Assert.AreEqual(3, controls.Count);
            var ctrl1 = controls[0];
            var lblTest1 = controls["lblTest1"];
            var lblTest2 = controls["lblTest2"];
            var lblTest3 = controls["lblTest3"];
            Assert.IsTrue(ctrl1 == lblTest1);
            Assert.IsTrue(lblTest1 is Label);
            Assert.IsTrue(lblTest2 is Label);
            Assert.IsTrue(lblTest3 is Label);
            Assert.IsTrue(lblTest1.Owner is ControlCreationTests);
            Assert.IsTrue(lblTest2.Owner is ControlCreationTests);
            Assert.IsTrue(lblTest3.Owner is ControlCreationTests);

            var label1 = (Label)lblTest1;
            Assert.AreEqual("lblTest1", label1.Name);
            Assert.AreEqual(null, label1.Parent);
            Assert.AreEqual(new Rectangle(50, 50, 100, 15), label1.Bounds);
            Assert.AreEqual(new PointI(50, 50), label1.TopLeft);
            Assert.AreEqual(new PointI(150, 50), label1.TopRight);
            Assert.AreEqual(new PointI(50, 65), label1.BottomLeft);
            Assert.AreEqual(new PointI(150, 65), label1.BottomRight);
            Assert.AreEqual("Arial", label1.FontName);
            Assert.AreEqual(Alignment.TopLeft, label1.ContentAlignment);
            Assert.AreEqual("Hello", label1.Text);
            Assert.AreEqual(Color.Yellow, label1.Color, "Color incorrect.");
            Assert.AreEqual(Color.Transparent, label1.BorderColor);

            var label2 = (Label)lblTest2;
            Assert.AreEqual("lblTest2", label2.Name);
            Assert.AreEqual(null, label2.Parent);
            Assert.AreEqual(new Rectangle(50, 50, 100, 15), label2.Bounds);
            Assert.AreEqual(new PointI(50, 50), label2.TopLeft);
            Assert.AreEqual(new PointI(150, 50), label2.TopRight);
            Assert.AreEqual(new PointI(50, 65), label2.BottomLeft);
            Assert.AreEqual(new PointI(150, 65), label2.BottomRight);
            Assert.AreEqual("Arial", label2.FontName);
            Assert.AreEqual(Alignment.TopLeft, label2.ContentAlignment);
            Assert.AreEqual("Hello", label2.Text);
            Assert.AreEqual(Color.Yellow, label2.Color);
            Assert.AreEqual(Color.Red, label2.BorderColor, "BorderColor incorrect.");

            var label3 = (Label)lblTest3;
            Assert.AreEqual("lblTest3", label3.Name);
            Assert.AreEqual(null, label3.Parent);
            Assert.AreEqual(new Rectangle(50, 50, 100, 15), label3.Bounds);
            Assert.AreEqual(new PointI(50, 50), label3.TopLeft);
            Assert.AreEqual(new PointI(150, 50), label3.TopRight);
            Assert.AreEqual(new PointI(50, 65), label3.BottomLeft);
            Assert.AreEqual(new PointI(150, 65), label3.BottomRight);
            Assert.AreEqual("Arial", label3.FontName);
            Assert.AreEqual(Alignment.TopLeft, label3.ContentAlignment);
            Assert.AreEqual("Goodbye", label3.Text);
            Assert.AreEqual(Color.Yellow, label3.Color);
            Assert.AreEqual(Color.Red, label3.BorderColor);
        }
    }
}