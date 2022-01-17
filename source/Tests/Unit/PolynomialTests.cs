using System.Collections.Generic;
using ProcessorsSubsystem;
using Xunit;

namespace Tests.Unit
{
    public class PolynomialTests
    {
        private static readonly VariableName XName = new("x");
        private static readonly VariableName YName = new("y");

        [Fact]
        public void ConstructorTests()
        {
            var _ = new Polynomial(new RationalNumber[] { }, XName);
            _ = new Polynomial(new RationalNumber[] {1}, XName);
            _ = new Polynomial(new RationalNumber[] {1, 1}, XName);
            _ = new Polynomial(GetRepeatCoefficients(0, 100), YName);
            _ = new Polynomial(GetRepeatCoefficients(new RationalNumber(1, 2), 3), YName);
        }

        private static IEnumerable<RationalNumber> GetRepeatCoefficients(RationalNumber num, int cnt)
        {
            while (cnt-- > 0)
                yield return num;
        }

        [Fact]
        public void DegreeTests()
        {
            var p0 = new Polynomial(new RationalNumber[] { }, YName);
            Assert.Equal(-1, p0.Degree);

            var p1 = new Polynomial(GetRepeatCoefficients(new RationalNumber(5, 7), 10), YName);
            Assert.Equal(9, p1.Degree);

            var p2 = new Polynomial(GetRepeatCoefficients(new RationalNumber(0, 1), 100), YName);
            Assert.Equal(-1, p2.Degree);

            var p3 = new Polynomial(new RationalNumber[] {1, 1, 0, 5, 0, 0, 0}, YName);
            Assert.Equal(3, p3.Degree);
        }

        [Fact]
        public void LeadingTests()
        {
            var p1 = new Polynomial(new RationalNumber[] {1}, XName);
            Assert.Equal(1, p1.Leading);

            var p2 = new Polynomial(new RationalNumber[] {1, -10}, XName);
            Assert.Equal(-10, p2.Leading);

            var p3 = new Polynomial(new RationalNumber[] {1, -120, 0, 0}, YName);
            Assert.Equal(-120, p3.Leading);

            var p4 = new Polynomial(GetRepeatCoefficients(new RationalNumber(1, 2), 3), YName);
            Assert.Equal(new RationalNumber(1, 2), p4.Leading);


            var p5 = new Polynomial(new RationalNumber[] { }, XName);
            Assert.Equal(0, p5.Leading);

            var p6 = new Polynomial(new RationalNumber[] {0, 0, 0}, YName);
            Assert.Equal(0, p6.Leading);
        }

        [Fact]
        public void AddTests()
        {
            var p1 = new Polynomial(new RationalNumber[] {1}, XName);
            var p2 = new Polynomial(new RationalNumber[] {1}, XName);
            var expectedP1P2 = new Polynomial(new RationalNumber[] {2}, XName);

            Assert.Equal(expectedP1P2, p1 + p2);
            Assert.Equal(expectedP1P2, p2 + p1);


            var p3 = new Polynomial(new RationalNumber[] {7, -3, 5}, XName);
            var p4 = new Polynomial(new RationalNumber[] {15, -13, 0, 3, 0, -6, 32}, XName);
            var expectedP3P4 = new Polynomial(new RationalNumber[] {22, -16, 5, 3, 0, -6, 32}, XName);

            Assert.Equal(expectedP3P4, p3 + p4);
            Assert.Equal(expectedP3P4, p4 + p3);


            var p5 = new Polynomial(GetRepeatCoefficients(new RationalNumber(1, 3), 5), XName);
            var p6 = new Polynomial(GetRepeatCoefficients(new RationalNumber(2, 3), 5), XName);
            var expectedP5P6 = new Polynomial(GetRepeatCoefficients(1, 5), XName);

            Assert.Equal(expectedP5P6, p5 + p6);
            Assert.Equal(expectedP5P6, p6 + p5);


            var p7 = new Polynomial(new RationalNumber[] {1, 2, 0, 1}, YName);
            var p8 = new Polynomial(new RationalNumber[] {1, 2, 0, -1}, YName);
            var expectedP7P8 = new Polynomial(new RationalNumber[] {2, 4}, YName);

            Assert.Equal(expectedP7P8, p7 + p8);
            Assert.Equal(expectedP7P8, p8 + p7);


            var p9 = new Polynomial(new RationalNumber[] {1, 2, 0, 1}, YName);
            var p10 = new Polynomial(new RationalNumber[] {0}, YName);
            var expectedP9P10 = new Polynomial(new RationalNumber[] {1, 2, 0, 1}, YName);

            Assert.Equal(expectedP9P10, p9 + p10);
            Assert.Equal(expectedP9P10, p10 + p9);
        }

        [Fact]
        public void SubTests()
        {
            var p1 = new Polynomial(new RationalNumber[] {1}, XName);
            var p2 = new Polynomial(new RationalNumber[] {-1}, XName);
            var expectedP1P2 = new Polynomial(new RationalNumber[] {2}, XName);

            Assert.Equal(expectedP1P2, p1 - p2);


            var p3 = new Polynomial(new RationalNumber[] {7, -3, 5}, XName);
            var p4 = new Polynomial(new RationalNumber[] {15, -13, 0, 3, 0, -6, 32}, XName);
            var expectedP3P4 = new Polynomial(new RationalNumber[] {-8, 10, 5, -3, 0, 6, -32}, XName);

            Assert.Equal(expectedP3P4, p3 - p4);


            var p5 = new Polynomial(GetRepeatCoefficients(new RationalNumber(5, 3), 5), XName);
            var p6 = new Polynomial(GetRepeatCoefficients(new RationalNumber(2, 3), 5), XName);
            var expectedP5P6 = new Polynomial(GetRepeatCoefficients(1, 5), XName);

            Assert.Equal(expectedP5P6, p5 - p6);


            var p7 = new Polynomial(new RationalNumber[] {1, 2, 0, 1}, YName);
            var p8 = new Polynomial(new RationalNumber[] {1, -2, 0, 1}, YName);
            var expectedP7P8 = new Polynomial(new RationalNumber[] {0, 4}, YName);

            Assert.Equal(expectedP7P8, p7 - p8);


            var p9 = new Polynomial(new RationalNumber[] {1, 2, 0, 1}, YName);
            var p10 = new Polynomial(new RationalNumber[] {0}, YName);
            var expectedP9P10 = new Polynomial(new RationalNumber[] {1, 2, 0, 1}, YName);

            Assert.Equal(expectedP9P10, p9 - p10);

            var expectedP10P9 = new Polynomial(new RationalNumber[] {-1, -2, 0, -1}, YName);

            Assert.Equal(expectedP10P9, p10 - p9);
        }

        [Fact]
        public void MultiTests()
        {
            var p1 = new Polynomial(new RationalNumber[] {1}, XName);
            var p2 = new Polynomial(new RationalNumber[] {-1}, XName);
            var expectedP1P2 = new Polynomial(new RationalNumber[] {-1}, XName);

            Assert.Equal(expectedP1P2, p1 * p2);
            Assert.Equal(expectedP1P2, p2 * p1);


            var p3 = new Polynomial(new RationalNumber[] {7, -3, 5}, XName);
            var p4 = new Polynomial(new RationalNumber[] {15, -13, 0, 3, 0, -6, 32}, XName);
            var expectedP3P4 = new Polynomial(new RationalNumber[] {105, -136, 114, -44, -9, -27, 242, -126, 160},
                XName);

            Assert.Equal(expectedP3P4, p3 * p4);
            Assert.Equal(expectedP3P4, p4 * p3);


            var p5 = new Polynomial(GetRepeatCoefficients(new RationalNumber(3, 2), 3), XName);
            var p6 = new Polynomial(GetRepeatCoefficients(new RationalNumber(2, 3), 3), XName);
            var expectedP5P6 = new Polynomial(new RationalNumber[] {1, 2, 3, 2, 1}, XName);

            Assert.Equal(expectedP5P6, p5 * p6);
            Assert.Equal(expectedP5P6, p6 * p5);


            var p9 = new Polynomial(new RationalNumber[] {1, 2, 0, 1}, YName);
            var p10 = new Polynomial(new RationalNumber[] {0}, YName);
            var expectedP9P10 = new Polynomial(new RationalNumber[] {0}, YName);

            Assert.Equal(expectedP9P10, p9 * p10);
            Assert.Equal(expectedP9P10, p10 * p9);
        }

        [Fact]
        public void MultiToIntTests()
        {
            var p1 = new Polynomial(new RationalNumber[] {1}, XName);
            var p2 = -1;
            var expectedP1P2 = new Polynomial(new RationalNumber[] {-1}, XName);

            Assert.Equal(expectedP1P2, p1 * p2);
            Assert.Equal(expectedP1P2, p2 * p1);


            var p3 = new Polynomial(new RationalNumber[] {7, -3, 5}, XName);
            var p4 = 3;
            var expectedP3P4 = new Polynomial(new RationalNumber[] {21, -9, 15},
                XName);

            Assert.Equal(expectedP3P4, p3 * p4);
            Assert.Equal(expectedP3P4, p4 * p3);


            var p5 = new Polynomial(new RationalNumber[] {1, 2, 0, 1}, YName);
            var p6 = 0;
            var expectedP5P6 = new Polynomial(new RationalNumber[] {0}, YName);

            Assert.Equal(expectedP5P6, p5 * p6);
            Assert.Equal(expectedP5P6, p5 * p6);
        }

        [Fact]
        public void DivTests()
        {
            var p1 = new Polynomial(new RationalNumber[] {1}, XName);
            var p2 = new Polynomial(new RationalNumber[] {1}, XName);
            var expectedP1P2 = new Polynomial(new RationalNumber[] {1}, XName);

            Assert.Equal(expectedP1P2, p1 / p2);


            var p3 = new Polynomial(new RationalNumber[] {7, -3, 5}, XName);
            var p4 = new Polynomial(new RationalNumber[] {15, -13, 0, 3, 0, -6, 32}, XName);
            var expectedP3P4 = new Polynomial(new RationalNumber[] {0}, XName);
            var expectedP4P3 = new Polynomial(new []
            {
                new RationalNumber(18167, 3125),
                new RationalNumber(-4701, 625),
                new RationalNumber(-922, 125),
                new RationalNumber(66, 25),
                new RationalNumber(32, 5),
            }, XName);

            Assert.Equal(expectedP3P4, p3 / p4);
            Assert.Equal(expectedP4P3, p4 / p3);


            var p5 = new Polynomial(GetRepeatCoefficients(new RationalNumber(2, 3), 5), XName);
            var p6 = new Polynomial(GetRepeatCoefficients(new RationalNumber(1, 3), 5), XName);
            var expectedP5P6 = new Polynomial(new RationalNumber[] {2}, XName);

            Assert.Equal(expectedP5P6, p5 / p6);


            var p7 = new Polynomial(new RationalNumber[] {0}, YName);
            var p8 = new Polynomial(new RationalNumber[] {1, 2, 0, -1}, YName);
            var expectedP7P8 = new Polynomial(new RationalNumber[] {0}, YName);

            Assert.Equal(expectedP7P8, p7 / p8);
        }

        [Fact]
        public void ReminderTests()
        {
            var p1 = new Polynomial(new RationalNumber[] { 1 }, XName);
            var p2 = new Polynomial(new RationalNumber[] { 1 }, XName);
            var expectedP1P2 = new Polynomial(new RationalNumber[] { 0 }, XName);

            Assert.Equal(expectedP1P2, p1 % p2);


            var p3 = new Polynomial(new RationalNumber[] { 7, -3, 5 }, XName);
            var p4 = new Polynomial(new RationalNumber[] { 15, -13, 0, 3, 0, -6, 32 }, XName);
            var expectedP3P4 = new Polynomial(new RationalNumber[] { 7, -3, 5 }, XName);
            var expectedP4P3 = new Polynomial(new []
            {
                new RationalNumber(-80294, 3125),
                new RationalNumber(178411, 3125),
            }, XName);

            Assert.Equal(expectedP3P4, p3 % p4);
            Assert.Equal(expectedP4P3, p4 % p3);


            var p5 = new Polynomial(GetRepeatCoefficients(new RationalNumber(2, 3), 5), XName);
            var p6 = new Polynomial(GetRepeatCoefficients(new RationalNumber(1, 3), 5), XName);
            var expectedP5P6 = new Polynomial(new RationalNumber[] { 0 }, XName);

            Assert.Equal(expectedP5P6, p5 % p6);


            var p7 = new Polynomial(new RationalNumber[] { 0 }, YName);
            var p8 = new Polynomial(new RationalNumber[] { 1, 2, 0, -1 }, YName);
            var expectedP7P8 = new Polynomial(new RationalNumber[] { 0 }, YName);

            Assert.Equal(expectedP7P8, p7 % p8);
        }

        [Fact]
        public void EqualTests()
        {
            var p1 = new Polynomial(new RationalNumber[] { 1 }, XName);
            var p2 = new Polynomial(new RationalNumber[] { 1 }, XName);

            Assert.True(p1 == p2);
            Assert.True(p2 == p1);


            var p3 = new Polynomial(new RationalNumber[] { 7, -3, 5 }, XName);
            var p4 = new Polynomial(new RationalNumber[] { 15, -13, 0, 3, 0, -6, 32 }, XName);

            Assert.False(p3 == p4);
            Assert.False(p4 == p3);


            var p5 = new Polynomial(GetRepeatCoefficients(new RationalNumber(1, 3), 5), XName);
            var p6 = new Polynomial(GetRepeatCoefficients(new RationalNumber(2, 6), 5), XName);

            Assert.True(p5 == p6);
            Assert.True(p6 == p5);


            var p7 = new Polynomial(new RationalNumber[] { 1, 2, 0, 1 }, YName);
            var p8 = new Polynomial(new RationalNumber[] { 1, 2, 0, 1, 0, 0, 0 }, YName);

            Assert.True(p7 == p8);
            Assert.True(p8 == p7);


            var p9 = new Polynomial(new RationalNumber[] { 1, 2, 0, 1 }, YName);
            p9 -= p9;
            var p10 = new Polynomial(new RationalNumber[] { 0 }, YName);

            Assert.True(p9 == p10);
            Assert.True(p10 == p9);
        }

        [Fact]
        public void GetDerivativeTests()
        {
            var p1 = new Polynomial(new RationalNumber[] { 1 }, XName);
            var expectedP1 = new Polynomial(new RationalNumber[]{}, XName);

            Assert.Equal(expectedP1, p1.GetDerivative());


            var p2 = new Polynomial(new RationalNumber[] { 15, 0, -13, 0, 3, 0, -6, 32 }, XName);
            var expectedP2 = new Polynomial(new RationalNumber[] { 0, -26, 0, 12, 0, -36, 224 }, XName);

            Assert.Equal(expectedP2, p2.GetDerivative());


            var p3 = new Polynomial(GetRepeatCoefficients(1, 5), XName);
            var expectedP3 = new Polynomial(new RationalNumber[]{1,2,3,4}, XName);

            Assert.Equal(expectedP3, p3.GetDerivative());


            var p4 = new Polynomial(new RationalNumber[] { 0 }, YName);
            var expectedP4 = new Polynomial(new RationalNumber[] { 0 }, YName);

            Assert.Equal(expectedP4, p4.GetDerivative());
        }

        [Fact]
        public void HashCodeTests()
        {
            var p1 = new Polynomial(new RationalNumber[] { 1 }, XName);
            var p2 = new Polynomial(new RationalNumber[] { 1 }, XName);

            Assert.True(p1.GetHashCode() == p2.GetHashCode());


            var p3 = new Polynomial(new RationalNumber[] { 7, -3, 5 }, XName);
            var p4 = new Polynomial(new RationalNumber[] { 15, -13, 0, 3, 0, -6, 32 }, XName);

            Assert.False(p3.GetHashCode() == p4.GetHashCode());


            var p5 = new Polynomial(GetRepeatCoefficients(new RationalNumber(1, 3), 5), XName);
            var p6 = new Polynomial(GetRepeatCoefficients(new RationalNumber(2, 3), 5), XName);

            Assert.False(p5.GetHashCode() == p6.GetHashCode());


            var p7 = new Polynomial(new RationalNumber[] { 1, 2, 0, 1 }, YName);
            var p8 = new Polynomial(new RationalNumber[] { 1, 2, 0, 1, 0, 0, 0 }, YName);

            Assert.True(p7.GetHashCode() == p8.GetHashCode());


            var p9 = new Polynomial(new RationalNumber[] { 1, 2, 0, 1 }, YName);
            p9 -= p9;
            var p10 = new Polynomial(new RationalNumber[] { 0 }, YName);

            Assert.True(p9.GetHashCode() == p10.GetHashCode());


            var p11 = new Polynomial(new RationalNumber[]{}, XName);
            var p12 = new Polynomial(new RationalNumber[]{0}, YName);

            Assert.False(p11.GetHashCode() == p12.GetHashCode());
        }
    }
}