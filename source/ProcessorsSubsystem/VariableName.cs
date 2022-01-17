using System;

namespace ProcessorsSubsystem
{
    public class VariableName : IEquatable<VariableName>, IComparable<VariableName>
    {
        private readonly string _name;

        public VariableName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            _name = name;
        }

        public int CompareTo(VariableName other)
        {
            return string.Compare(_name, other._name, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return (_name != null ? _name.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return _name;
        }

        public bool Equals(VariableName other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _name == other._name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((VariableName) obj);
        }
    }
}