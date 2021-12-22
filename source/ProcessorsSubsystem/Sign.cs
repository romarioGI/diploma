using System;

namespace ProcessorsSubsystem
{
    [Flags]
    public enum Sign
    {
        None = 0b_0000,
        LessZero = 0b_0001,
        MoreZero = 0b_0010,
        EqualZero = 0b_0100,
        Any = LessZero | MoreZero | EqualZero,
        NotEqual = Any ^ EqualZero,
        NotLess = Any ^ LessZero,
        NotMore = Any ^ MoreZero
    }
}