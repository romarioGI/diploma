using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ProcessorsSubsystem
{
    public class Polynomial : IEquatable<Polynomial>
    {
        private readonly RationalNumber[] _coefficients;
        private readonly int _hashCode;

        public readonly VariableName VariableName;

        public bool IsZero => Degree == -1;

        public Polynomial(IEnumerable<RationalNumber> coefficients, VariableName variableName) :
            this(coefficients?.ToArray(), variableName)
        {
        }

        private Polynomial(RationalNumber[] coefficients, VariableName variableName)
        {
            VariableName = variableName;

            if (coefficients is null)
                throw new ArgumentNullException(nameof(coefficients));

            var degree = coefficients.Length - 1;
            while (degree >= 0 && coefficients[degree].IsZero)
                --degree;

            _coefficients = new RationalNumber[degree + 1];
            for (var i = 0; i <= degree; ++i)
                _coefficients[i] = coefficients[i];

            Degree = degree;
            _hashCode = CalcHashCode();
        }

        public readonly int Degree;

        private RationalNumber this[int degree]
        {
            get
            {
                if (degree < 0 || degree > Degree)
                    throw new ArgumentOutOfRangeException();

                return _coefficients[degree];
            }
        }

        public RationalNumber Leading => IsZero ? 0 : _coefficients[Degree];

        public static Polynomial operator +(Polynomial f, Polynomial g)
        {
            if (f is null || g is null)
                throw new ArgumentNullException();

            if (!f.VariableName.Equals(g.VariableName))
                throw new Exception();

            var result = new RationalNumber[Math.Max(f.Degree, g.Degree) + 1];
            var minDegree = Math.Min(f.Degree, g.Degree);
            for (var d = 0; d <= minDegree; ++d)
                result[d] = f[d] + g[d];

            for (var d = minDegree + 1; d <= f.Degree; ++d)
                result[d] = f[d];
            for (var d = minDegree + 1; d <= g.Degree; ++d)
                result[d] = g[d];

            return new Polynomial(result, f.VariableName);
        }

        public static Polynomial operator -(Polynomial f, Polynomial g)
        {
            if (f is null || g is null)
                throw new ArgumentNullException();

            if (!f.VariableName.Equals(g.VariableName))
                throw new Exception();

            var result = new RationalNumber[Math.Max(f.Degree, g.Degree) + 1];
            var minDegree = Math.Min(f.Degree, g.Degree);
            for (var d = 0; d <= minDegree; ++d)
                result[d] = f[d] - g[d];

            for (var d = minDegree + 1; d <= f.Degree; ++d)
                result[d] = f[d];
            for (var d = minDegree + 1; d <= g.Degree; ++d)
                result[d] = -g[d];

            return new Polynomial(result, f.VariableName);
        }

        public static Polynomial operator -(Polynomial f)
        {
            if (f is null)
                throw new ArgumentNullException();

            var result = f._coefficients.Select(c => -c);

            return new Polynomial(result, f.VariableName);
        }

        public static Polynomial operator *(Polynomial f, Polynomial g)
        {
            if (f is null || g is null)
                throw new ArgumentNullException();

            if (!f.VariableName.Equals(g.VariableName))
                throw new Exception();

            if (f.IsZero)
                return f;
            if (g.IsZero)
                return g;

            var result = new RationalNumber[f.Degree + g.Degree + 1];
            for (var i = 0; i < result.Length; ++i)
                result[i] = new RationalNumber(0, 1);

            for (var d1 = 0; d1 <= f.Degree; ++d1)
            for (var d2 = 0; d2 <= g.Degree; ++d2)
                result[d1 + d2] += f[d1] * g[d2];

            return new Polynomial(result, f.VariableName);
        }

        public static Polynomial operator *(Polynomial f, int a)
        {
            if (f is null)
                throw new ArgumentNullException();

            if (f.IsZero)
                return f;

            var result = f._coefficients.Select(c => c * a).ToArray();

            return new Polynomial(result, f.VariableName);
        }

        public static Polynomial operator *(int a, Polynomial f)
        {
            if (f is null)
                throw new ArgumentNullException();

            return f * a;
        }

        public static Polynomial operator /(Polynomial f, Polynomial g)
        {
            if (f is null || g is null)
                throw new ArgumentNullException();

            if (!f.VariableName.Equals(g.VariableName))
                throw new Exception();

            if (g.IsZero)
                throw new DivideByZeroException();
            if (f.IsZero)
                return f;

            return DivisionWithRemainder(f, g).Item1;
        }

        public static Polynomial operator %(Polynomial f, Polynomial g)
        {
            if (f is null || g is null)
                throw new ArgumentNullException();

            if (!f.VariableName.Equals(g.VariableName))
                throw new Exception();

            if (g.IsZero)
                throw new DivideByZeroException();
            if (f.IsZero)
                return f;

            return DivisionWithRemainder(f, g).Item2;
        }

        private static (Polynomial, Polynomial) DivisionWithRemainder(Polynomial f, Polynomial g)
        {
            var resultLength = Math.Max(f.Degree - g.Degree + 1, 0);
            var result = new RationalNumber[resultLength];

            var fCoefficients = (RationalNumber[]) f._coefficients.Clone();
            var leadingG = g[g.Degree];

            for (var d1 = f.Degree; d1 >= g.Degree; --d1)
            {
                if (fCoefficients[d1].IsZero)
                    continue;

                var newCoefficient = fCoefficients[d1] / leadingG;
                var monomDegree = d1 - g.Degree;
                result[monomDegree] = newCoefficient;

                for (var d2 = 0; d2 <= g.Degree; ++d2)
                    fCoefficients[monomDegree + d2] -= newCoefficient * g[d2];
            }

            var q = new Polynomial(result, f.VariableName);

            var r = new Polynomial(fCoefficients, f.VariableName);

            return (q, r);
        }

        public static bool operator ==(Polynomial f, Polynomial g)
        {
            if (f is null || g is null)
                throw new ArgumentNullException();

            if (!f.VariableName.Equals(g.VariableName))
                throw new Exception();

            if (ReferenceEquals(f, g))
                return true;

            if (f.GetHashCode() != g.GetHashCode() || f.Degree != g.Degree)
                return false;

            for (var d = 0; d < f.Degree; d++)
                if (!f[d].Equals(g[d]))
                    return false;

            return true;
        }

        public static bool operator !=(Polynomial f, Polynomial g)
        {
            return !(f == g);
        }

        public Polynomial GetDerivative()
        {
            if (IsZero)
                return this;

            var result = new RationalNumber[Degree];
            for (var d = 1; d <= Degree; d++)
                result[d - 1] = this[d] * d;

            return new Polynomial(result, VariableName);
        }

        public Polynomial Pow(BigInteger degree)
        {
            if (degree < 0)
                throw new ArgumentOutOfRangeException(nameof(degree));
            var result = new Polynomial(new List<RationalNumber> {1}, VariableName);
            var a = this;

            while (degree != 0)
            {
                if (degree % 2 == 1)
                    result *= a;
                degree /= 2;
                a *= a;
            }

            return result;
        }

        public bool Equals(Polynomial other)
        {
            if (other is null)
                return false;

            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Polynomial polynomial && Equals(polynomial);
        }

        public override int GetHashCode() => _hashCode;

        private int CalcHashCode()
        {
            var res = 0;
            foreach (var coefficient in _coefficients)
                res = HashCode.Combine(coefficient, res);

            return HashCode.Combine(res, VariableName, Degree);
        }
    }
}