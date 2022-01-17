using System;
using System.Collections.Generic;
using System.Linq;
using ProcessorsSubsystem;
using Xunit;

namespace Tests.Unit
{
    public class SaturatorTests
    {
        private static readonly VariableName XName = new VariableName("x");

        [Fact]
        public void Test1()
        {
            var polynomials = new List<Polynomial> {new (new List<RationalNumber> {0}, XName)};
            var saturatedSystem = Saturator.Saturate(polynomials).ToList();
            
            polynomials.Sort();
            saturatedSystem.Sort();
            Assert.Equal(polynomials, saturatedSystem);
        }

        [Fact]
        public void Test2()
        {
            var polynomials = new List<Polynomial> {new Polynomial(new List<RationalNumber> {5}, XName)};
            var saturatedSystem = Saturator.Saturate(polynomials).ToList();

            var expected = new List<Polynomial> { new Polynomial(new List<RationalNumber> { 0 }, XName), new Polynomial(new List<RationalNumber> {5}, XName)};

            expected.Sort();
            saturatedSystem.Sort();
            Assert.Equal(expected, saturatedSystem);
        }

        [Fact]
        public void Test3()
        {
            var polynomials = new List<Polynomial> {new Polynomial(new List<RationalNumber> {1, 1, 0, 1}, XName)};
            var saturatedSystem = Saturator.Saturate(polynomials).ToList();

            var expected = new List<Polynomial>
            {
                new Polynomial(new List<RationalNumber> {-9}, XName),
                new Polynomial(new List<RationalNumber> {new RationalNumber(-31, 8)}, XName),
                new Polynomial(new List<RationalNumber> {new RationalNumber(2, 3)}, XName),
                new Polynomial(new List<RationalNumber> {1}, XName),
                new Polynomial(new List<RationalNumber> {6}, XName),
                new Polynomial(new List<RationalNumber> {new RationalNumber(31, 4)}, XName),
                new Polynomial(new List<RationalNumber> {1, new RationalNumber(2, 3)}, XName),
                new Polynomial(new List<RationalNumber> {0, 6}, XName),
                new Polynomial(new List<RationalNumber> {1, 0, 3}, XName),
                new Polynomial(new List<RationalNumber> {1, 1, 0, 1}, XName)
            };

            foreach (var p in expected)
            {
                if(saturatedSystem.Contains(p))
                    continue;
                throw new Exception();
            }
            
            expected.Sort();
            saturatedSystem.Sort();
            Assert.Equal(expected, saturatedSystem);
        }

        [Fact]
        public void Test4()
        {
            var polynomials = new List<Polynomial> {new Polynomial(new List<RationalNumber> {0, 0, 1}, XName)};
            var saturatedSystem = Saturator.Saturate(polynomials).ToList();

            var expected = new List<Polynomial>
            {
                new Polynomial(new List<RationalNumber> {0}, XName),
                new Polynomial(new List<RationalNumber> {2}, XName),
                new Polynomial(new List<RationalNumber> {0, 2}, XName),
                new Polynomial(new List<RationalNumber> {0, 0, 1}, XName),
            };

            expected.Sort();
            saturatedSystem.Sort();
            Assert.Equal(expected, saturatedSystem);
        }

        [Fact]
        public void Test5()
        {
            var polynomials = new List<Polynomial> {new Polynomial(new List<RationalNumber> {1, -1, -1, 1}, XName)};
            var saturatedSystem = Saturator.Saturate(polynomials).ToList();

            var expected = new List<Polynomial>
            {
                new Polynomial(new List<RationalNumber> {0}, XName),
                new Polynomial(new List<RationalNumber> {new RationalNumber(16, 27)}, XName),
                new Polynomial(new List<RationalNumber> {4}, XName),
                new Polynomial(new List<RationalNumber> {new RationalNumber(-4, 3)}, XName),
                new Polynomial(new List<RationalNumber> {new RationalNumber(-8, 9)}, XName),
                new Polynomial(new List<RationalNumber> {new RationalNumber(8, 9), new RationalNumber(-8, 9)},
                    XName),
                new Polynomial(new List<RationalNumber> {6}, XName),
                new Polynomial(new List<RationalNumber> {-2, 6}, XName),
                new Polynomial(new List<RationalNumber> {-1, -2, 3}, XName),
                new Polynomial(new List<RationalNumber> {1, -1, -1, 1}, XName)
            };

            expected.Sort();
            saturatedSystem.Sort();
            Assert.Equal(expected, saturatedSystem);
        }
    }
}