using System;

namespace IronBasic
{
    [Flags]
    public enum Grammar
    {
        Pcjr = 1,
        Tandy = 2,
        Advanced = 4,

        All = Pcjr | Tandy | Advanced
    }
}