using System;

namespace IronBasic.Compilor
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