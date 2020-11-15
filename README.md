# Zen.GuiControls

A project for user controls for use with MonoGame.
Current controls: Button, Label, Image and Frame.

Nuget package download: https://www.nuget.org/packages/Zen.GuiControls/0.1.6

# Examples
To use:
(programmatically)

    // in constructor or LoadContent
    
    // button
    _control1 = new Button("btnApply", "TextureNormal", "TextureActive", "TextureHover", "TextureDisabled")
    {
        Size = new PointI(100, 25),
        Color = Color.Green
    };
    _control1.SetPosition(new PointI(50, 50));
    _control1.AddPackage(new ControlClick(ApplySettings)); // will call method ApplySettings(object o, EventArgs args) when clicked with mouse
    
    // label
    _control2 = new Label("lblHealth", "arial")
    {
        Size = new PointI(100, 25),
        ContentAlignment = Alignment.TopRight,
        Text = "Health:",
        TextColor = Color.Green,
        BorderColor = Color.Red
    };
    _control2.SetPosition(new PointI(10, 10));
    
    // frame
    _control3 = new Frame("frmMain")
    _control3.AddControl(_control1, _control2);
    _control3.SetPosition(new PointI(100, 100));
    
    // in LoadContent
    _control3.LoadContent(content, true);
    
    // in Update
    _control3.Update(_input, (float)gameTime.ElapsedGameTime.TotalMilliseconds);
    
    // in Draw
    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
    _control3.Draw(spriteBatch);
    spriteBatch.End();

(descriptively)

    // in constructor or LoadContent
    var spec = @"
<pre>
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
</pre>
    var pairs = new List<KeyValuePair<string, string>>
    {
        new KeyValuePair<string, string>("position1", "1680;0")
    };
    _controls = ControlCreator.CreateFromSpecification(spec, pairs);
    
    // in LoadContent
    _controls.LoadContent(content, true);
    
    // in Update
    _controls.Update(_input, (float)gameTime.ElapsedGameTime.TotalMilliseconds);
    
    // in Draw
    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
    _controls.Draw(spriteBatch);
    spriteBatch.End();

# Developer
Written by Greg Moller (greg.moller@gmail.com)  
If you have any questions drop me a line at the above email.

# License
Licensed under the MIT License. See the LICENCE file for more information.
