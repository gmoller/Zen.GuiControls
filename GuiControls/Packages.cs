using System.Collections;
using System.Collections.Generic;
using Zen.Input;

namespace Zen.GuiControls
{
    internal class Packages : IEnumerable<IPackage>
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

        internal ControlStatus Process(IControl control, InputHandler input, float deltaTime)
        {
            var controlStatus = control.Status;

            foreach (var package in PackagesList)
            {
                controlStatus = package.Process(control, input, deltaTime);
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