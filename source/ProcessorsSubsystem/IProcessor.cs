using ParserSubsystem;

namespace ProcessorsSubsystem
{
    public interface IProcessor<T> where T:IExpression
    {
        public T Do(T expression);
    }
}