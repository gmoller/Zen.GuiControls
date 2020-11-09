using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Zen.Input;
using Zen.Utilities;

namespace Zen.GuiControls
{
    public class Controls : IEnumerable<IControl>
    {
        private Dictionary<string, IControl> ChildControlsList { get; }

        internal Controls()
        {
            ChildControlsList = new Dictionary<string, IControl>();
        }

        public IControl this[int index] => ChildControlsList.Values.ElementAt(index);
        public IControl this[string name] => FindControl(name);
        public int Count => ChildControlsList.Count;

        internal void Add(string key, IControl control)
        {
            ChildControlsList.Add(key, control);
        }

        internal IControl FindControl(string key)
        {
            var split = key.Split('.');
            var childControl = ChildControlsList[split[0]];
            for (var i = 1; i < split.Length; i++)
            {
                childControl = childControl[split[i]];
            }

            return childControl;
        }

        public void LoadContent(ContentManager content, bool loadChildrenContent = false)
        {
            foreach (var childControl in ChildControlsList.Values)
            {
                childControl.LoadContent(content, loadChildrenContent);
            }
        }

        internal void SetTopLeftPosition(PointI point)
        {
            foreach (var child in ChildControlsList.Values)
            {
                child.SetPosition(point + child.RelativeTopLeft);
            }
        }

        internal void MoveTopLeftPosition(PointI point)
        {
            foreach (var child in ChildControlsList.Values)
            {
                child.MovePosition(point);
            }
        }

        public void Update(InputHandler input, float deltaTime, Viewport? viewport)
        {
            foreach (var childControl in ChildControlsList.Values)
            {
                childControl.Update(input, deltaTime, viewport);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var childControl in ChildControlsList.Values)
            {
                childControl.Draw(spriteBatch);
            }
        }

        public void Draw(Matrix? transform = null)
        {
            foreach (var childControl in ChildControlsList.Values)
            {
                childControl.Draw(transform);
            }
        }

        public void SetOwner(object owner)
        {
            foreach (var childControl in ChildControlsList.Values)
            {
                childControl.Owner = owner;
            }
        }

        public IEnumerator<IControl> GetEnumerator()
        {
            foreach (var item in ChildControlsList)
            {
                yield return item.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return DebuggerDisplay;
        }

        private string DebuggerDisplay => $"{{Count={Count}}}";
    }
}