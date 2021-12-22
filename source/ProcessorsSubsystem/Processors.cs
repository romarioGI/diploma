namespace ProcessorsSubsystem
{
    public static class Processors
    {
        public static readonly ZeroArityPredicateEliminator ZeroArityPredicateEliminator = new ();
        public static readonly TarskiQuantifierEliminator TarskiQuantifierEliminator = new ();
    }
}