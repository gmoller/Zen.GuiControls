namespace Zen.GuiControls.TheControls
{
    public class TextBox : Label
    {
        public TextBox(string name) : base(name)
        {
        }

        public TextBox(Label other) : base(other)
        {
        }

        public override IControl Clone()
        {
            return new TextBox(this);
        }
    }
}