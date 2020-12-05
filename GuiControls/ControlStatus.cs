using System;
using System.Numerics;

namespace Zen.GuiControls
{
    [Flags]
    public enum ControlStatus
    {
        None = 0,
        Disabled = 1,
        Active = 2,
        MouseOver = 4,
        HasFocus = 8
    }

    public static class ControlStatusExtensions
    {
        public static int GetIndexOfEnumeration(this ControlStatus controlStatus)
        {
            var setBits = BitOperations.PopCount((uint)controlStatus); //get number of set bits
            if (setBits != 1)
            {
                //Finds ControlStatus.none, and all composite flags
                return -1; //undefined index
            }
            var index = BitOperations.TrailingZeroCount((uint)controlStatus); //Efficient bit operation

            return index;
        }
    }
}