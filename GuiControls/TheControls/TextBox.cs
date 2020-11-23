namespace Zen.GuiControls.TheControls
{
    public class TextBox : Control
    {
        public TextBox(string name) : base(name)
        {
        }

        public TextBox(Control other) : base(other)
        {
        }

        public override IControl Clone()
        {
            return new TextBox(this);
        }
    }
}