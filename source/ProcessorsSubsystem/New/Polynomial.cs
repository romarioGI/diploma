using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ProcessorsSubsystem.New
{
    public class Polynomial<T> : IRingElement, IEquatable<Polynomial<T>> where T : IRingElement
    {
        private readonly ImmutableArray<Ring<T>> _coefficients;
        private readonly string _variableName;

        public int Degree => _coefficients.Length - 1;

        public Sign Sign => Degree switch
        {
            -1 => Sign.EqualZero,
            0 => _coefficients[0].Sign,
            _ => Sign.Any
        };

        public bool IsZero => Sign == Sign.EqualZero;

        public Polynomial(IEnumerable<T> coefficients, string variableName) : this(
            coefficients.Select(x => (Ring<T>) x),
            variableName
        )
        {
        }

        public Polynomial(IEnumerable<Ring<T>> coefficients, string variableName)
        {
            _variableName = variableName ?? throw new ArgumentNullException(nameof(variableName));

            if (coefficients is null)
                throw new ArgumentNullException(nameof(coefficients));
            var coefficientsArray = coefficients.ToImmutableArray();

            var degree = coefficientsArray.Length - 1;
            while (degree >= 0 && coefficientsArray[degree].IsZero)
                --degree;

            _coefficients = coefficientsArray.Take(degree + 1).ToImmutableArray();
        }

        public Ring<T> this[int index]
        {
            get
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return index < _coefficients.Length ? _coefficients[index] : Ring<T>.Zero;
            }
        }

        private Polynomial<T> ToPolynomial(IRingElement right)
        {
            if (right is null)
                throw new ArgumentNullException(nameof(right));
            if (!(right is Polynomial<T> rightPolynomial))
                throw new Exception("type exception");
            if (_variableName != rightPolynomial._variableName)
                throw new Exception("variable name exception");

            return rightPolynomial;
        }

        public IRingElement Add(IRingElement right)
        {
            var rightPolynomial = ToPolynomial(right);

            var result = Enumerable.Empty<Ring<T>>();
            for (var i = 0; i <= Math.Max(Degree, rightPolynomial.Degree); ++i)
                result = result.Append(this[i] + rightPolynomial[i]);

            return new Polynomial<T>(result, _variableName);
        }

        public IRingElement Subtract(IRingElement right)
        {
            var rightPolynomial = ToPolynomial(right);

            var result = Enumerable.Empty<Ring<T>>();
            for (var i = 0; i <= Math.Max(Degree, rightPolynomial.Degree); ++i)
                result = result.Append(this[i] - rightPolynomial[i]);

            return new Polynomial<T>(result, _variableName);
        }

        public IRingElement Multiply(IRingElement right)
        {
            var rightPolynomial = ToPolynomial(right);

            if (IsZero)
                return this;
            if (rightPolynomial.IsZero)
                return rightPolynomial;

            var resultDegree = Degree + rightPolynomial.Degree;
            var result = Enumerable.Repeat(Ring<T>.Zero, resultDegree + 1).ToArray();

            for (var d1 = 0; d1 <= Degree; ++d1)
            for (var d2 = 0; d2 <= rightPolynomial.Degree; ++d2)
                result[d1 + d2] += this[d1] * rightPolynomial[d2];

            return new Polynomial<T>(result, _variableName);
        }

        public bool Equal(IRingElement right)
        {
            return Equals(right);
        }

        public bool Equals(Polynomial<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (Degree != other.Degree || _variableName != other._variableName)
                return false;
            return _coefficients
                .Zip(other._coefficients)
                .All(p => p.First == p.Second);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Polynomial<T>) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_coefficients, _variableName);
        }
    }
}