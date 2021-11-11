using System.Collections.Generic;

namespace InputSubsystem
{
    public interface IInput<out T> : IEnumerable<T> where T: ISymbol
    {
    }
}