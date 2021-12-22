using System;
using System.Collections.Generic;

namespace ProcessorsSubsystem.New
{
    public class Ring<T>: IEquatable<Ring<T>> where T: IRingElement
    {
        protected readonly T Value;

        public virtual Sign Sign => Value.Sign;
        
        protected Ring(T value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            Value = value;
        }

        public static Ring<T> One => RingConstants.One<T>();

        public static Ring<T> Zero => RingConstants.Zero<T>();

        public bool IsZero => this == Zero;

        //TODO проверки на null
        public static Ring<T> operator +(Ring<T> left, Ring<T> right)
        {
            return (T)((T)left).Add((T)right);
        }
        
        public static Ring<T> operator -(Ring<T> left, Ring<T> right)
        {
            return (T)((T)left).Subtract((T)right);
        }
        
        public static Ring<T> operator -(Ring<T> left)
        {
            return (T)((T)Zero).Subtract((T)left);
        }

        public static Ring<T> operator *(Ring<T> left, Ring<T> right)
        {
            return (T)((T)left).Multiply((T)right);
        }
        
        public static bool operator ==(Ring<T> left, Ring<T> right)
        {
            return ((T)left).Equal((T)right);
        }

        public static bool operator !=(Ring<T> left, Ring<T> right)
        {
            return !(left == right);
        }
        
        public static implicit operator T(Ring<T> value)
        {
            return value.Value;
        }
        
        public static implicit operator Ring<T>(T value)
        {
            return new(value);
        }

        public bool Equals(Ring<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Ring<T>) obj);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(Value);
        }
    }
}