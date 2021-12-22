using System;
using System.Collections.Generic;

namespace ProcessorsSubsystem.New
{
    public static class RingConstants
    {
        private static readonly IReadOnlyDictionary<Type, IRingElement> Zeros;
        private static readonly IReadOnlyDictionary<Type, IRingElement> Ones;

        //TODO для полиномов, для чисел
        static RingConstants()
        {
            Zeros = new Dictionary<Type, IRingElement>();
            Ones = new Dictionary<Type, IRingElement>();
        }

        public static T Zero<T>() where T : IRingElement => (T) Zeros[typeof(T)];

        public static T One<T>() where T : IRingElement => (T) Ones[typeof(T)];
    }
}