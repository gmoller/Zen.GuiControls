# Zen.GuiControls

A project for user controls for use with MonoGame.
Current controls: Button, Label, Image and Frame.

Nuget package download: https://www.nuget.org/packages/Zen.GuiControls/0.1.2

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
    _controls = ControlCreator.CreateFromResource("Game1.PanelControls.txt", new KeyValuePair<string, string>("backgroundColor", "CornflowerBlue"));
    
    // in LoadContent
    _foreach (var control in _controls)
    {
        control.LoadContent(content, true);
    }
    
    // in Update
    foreach (var control in _controls)
    {
        control.Update(_input, (float)gameTime.ElapsedGameTime.TotalMilliseconds);
    }
    
    // in Draw
    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
    foreach (var control in _controls)
    {
        control.Draw(spriteBatch);
    }
    spriteBatch.End();
    
The file: Game1.PanelControls.txt  
lblTest : Label  
{  
  FontName: arial  
  Size: 100;50  
  ContentAlignment: BottomRight  
  Text: Test  
  GetTextFunc: 'Game1.EventHandlers.GetTextFunc'  
  TextColor: Aqua  
  TextShadowColor: White  
  BackgroundColor: %backgroundColor%  
  BorderColor: Red  
  PositionAlignment: BottomRight  
  Position: 100;50  
  Scale: 1.0  
  LayerDepth: 0.0  
  Enabled: true  
  Packages: ['Zen.GuiControls.PackagesClasses.ControlClick, Zen.GuiControls - Game1.EventHandlers.ApplySettings']  
}  
  
lblApply : Label  
{  
  FontName: arial  
  Size: 100;25  
  Text: Apply  
  TextColor: CornflowerBlue  
  ContentAlignment: MiddleCenter  
}  
  
btnApply : Button  
{  
  TextureNormal: TextureNormal  
  TextureActive: TextureActive  
  TextureHover: TextureHover  
  TextureDisabled: TextureDisabled  
  Size: 100;25  
  Color: Green  
  PositionAlignment: TopLeft  
  Position: 50;200  
  LayerDepth: 1.0  
  Enabled: true  
  Contains: [lblApply]  
  Packages: ['Zen.GuiControls.PackagesClasses.ControlClick, Zen.GuiControls - Game1.EventHandlers.ApplySettings']  
}  
  
frmPanel : Frame  
{  
  Size: 200;500  
  Contains: [lblTest;btnApply]  
}
    
# Developer
Written by Greg Moller (greg.moller@gmail.com)

# License
Licensed under the MIT License. See the LICENCE file for more information.
