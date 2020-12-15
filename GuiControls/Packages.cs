using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Zen.Input;

namespace Zen.GuiControls
{
    public class Packages : IEnumerable<IPackage>
    {
        private List<IPackage> PackagesList { get; }

        internal Packages()
        {
            PackagesList = new List<IPackage>();
        }

        private int Count => PackagesList.Count;

        internal void Add(IPackage package)
        {
            PackagesList.Add(package);
        }

        internal void Reset()
        {
            foreach (var package in PackagesList)
            {
                package.Reset();
            }
        }

        internal ControlStatus Update(IControl control, InputHandler input, GameTime gameTime)
        {
            var controlStatus = control.Status;

            foreach (var package in PackagesList)
            {
                controlStatus = package.Update(control, input, gameTime);
                control.Status = controlStatus;
            }

            return controlStatus;
        }

        public IEnumerator<IPackage> GetEnumerator()
        {
            foreach (var item in PackagesList)
            {
                yield return item;
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