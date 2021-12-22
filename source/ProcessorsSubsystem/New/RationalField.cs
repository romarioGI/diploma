namespace ProcessorsSubsystem.New
{
    public class RationalField<T> : Ring<T> where T : IRingElement
    {
        private readonly Ring<T> _denominator;

        //TODO
        public override Sign Sign
        {
            get
            {
                if (Value.Sign == Sign.None || _denominator.Sign == Sign.None || _denominator.Sign == Sign.EqualZero)
                    return Sign.None;
                return Value.Sign switch
                {
                    Sign.EqualZero => Sign.EqualZero,
                    Sign.LessZero => _denominator.Sign switch
                    {
                        Sign.LessZero => Sign.MoreZero,
                        Sign.MoreZero => Sign.LessZero,
                        Sign.NotEqual => Sign.NotEqual,
                        _ => Sign.Any
                    },
                    Sign.MoreZero => _denominator.Sign switch
                    {
                        Sign.LessZero => Sign.LessZero,
                        Sign.MoreZero => Sign.MoreZero,
                        Sign.NotEqual => Sign.NotEqual,
                        _ => Sign.Any
                    },
                    Sign.Any => Sign.Any,
                    Sign.NotEqual => _denominator.Sign switch
                    {
                        Sign.NotEqual => Sign.NotEqual,
                        _ => Sign.Any
                    },
                    _ => Sign.Any
                };
            }
        }

        private RationalField(T value) : base(value)
        {
            _denominator = One;
        }

        private RationalField(T value, Ring<T> denominator) : base(value)
        {
            _denominator = denominator;
        }

        public static RationalField<T> operator +(RationalField<T> left, RationalField<T> right)
        {
            var value = left.Value * right._denominator + left._denominator * right.Value;
            var denominator = left._denominator * right._denominator;

            return new RationalField<T>(value, denominator);
        }

        public static RationalField<T> operator -(RationalField<T> left, RationalField<T> right)
        {
            var value = left.Value * right._denominator - left._denominator * right.Value;
            var denominator = left._denominator * right._denominator;

            return new RationalField<T>(value, denominator);
        }

        public static RationalField<T> operator -(RationalField<T> left)
        {
            return new RationalField<T>(Zero) - left;
        }

        public static RationalField<T> operator *(RationalField<T> left, RationalField<T> right)
        {
            var value = (Ring<T>) left.Value * right.Value;
            var denominator = left._denominator * right._denominator;

            return new RationalField<T>(value, denominator);
        }

        public static RationalField<T> operator /(RationalField<T> left, RationalField<T> right)
        {
            var value = left.Value * right._denominator;
            var denominator = left._denominator * right.Value;

            return new RationalField<T>(value, denominator);
        }

        public static bool operator ==(RationalField<T> left, RationalField<T> right)
        {
            return (left - right).IsZero;
        }

        public static bool operator !=(RationalField<T> left, RationalField<T> right)
        {
            return !(left == right);
        }
    }
}