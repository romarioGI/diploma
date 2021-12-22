using System;

namespace ProcessorsSubsystem
{
    public struct VariableName : IEquatable<VariableName>, IComparable<VariableName>
    {
        public readonly string Name;

        public VariableName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            Name = name;
        }

        public bool Equals(VariableName other)
        {
            return Name.Equals(other.Name, StringComparison.Ordinal);
        }

        public int CompareTo(VariableName other)
        {
            return string.Compare(Name, other.Name, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}