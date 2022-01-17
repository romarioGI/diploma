using System.Collections.Generic;
using System.Linq;
using ProcessorsSubsystem;
using Xunit;

namespace Tests.Unit
{
    public class TarskiTableTests
    {
        private static readonly VariableName XName = new VariableName("x");

        [Fact]
        public void Test1()
        {
            var polynomials = new List<Polynomial> {new Polynomial(new List<RationalNumber> {5}, XName)};
            var saturatedSystem = Saturator.Saturate(polynomials).ToList();
            var table = new TarskiTable(saturatedSystem);
            var signs = table[polynomials[0]].ToList();

            var expected = new List<Sign> {Sign.MoreZero, Sign.MoreZero};

            Assert.Equal(expected, signs);
        }

        [Fact]
        public void Test2()
        {
            var polynomials = new List<Polynomial> {new Polynomial(new List<RationalNumber> {1, 1, 0, 1}, XName)};
            var saturatedSystem = Saturator.Saturate(polynomials).ToList();
            var table = new TarskiTable(saturatedSystem);
            var signs = table[polynomials[0]].ToList();

            var expected = new List<Sign> {Sign.LessZero, Sign.LessZero, Sign.EqualZero, Sign.MoreZero, Sign.MoreZero};

            Assert.Equal(expected, signs);
        }

        [Fact]
        public void Test3()
        {
            var polynomials = new List<Polynomial> {new Polynomial(new List<RationalNumber> {0, 0, 1}, XName)};
            var saturatedSystem = Saturator.Saturate(polynomials).ToList();
            var table = new TarskiTable(saturatedSystem);
            var signs = table[polynomials[0]].ToList();

            var expected = new List<Sign> {Sign.MoreZero, Sign.EqualZero, Sign.MoreZero};

            Assert.Equal(expected, signs);
        }

        [Fact]
        public void Test4()
        {
            var polynomials = new List<Polynomial> {new Polynomial(new List<RationalNumber> {1, -1, -1, 1}, XName)};
            var saturatedSystem = Saturator.Saturate(polynomials).ToList();
            var table = new TarskiTable(saturatedSystem);
            var signs = table[polynomials[0]].ToList();

            var expected = new List<Sign>
                {Sign.LessZero, Sign.EqualZero, Sign.MoreZero, Sign.MoreZero, Sign.EqualZero, Sign.MoreZero};

            Assert.Equal(expected, signs);
        }

        [Fact]
        public void Test5()
        {
            var polynomials = new List<Polynomial> {new Polynomial(new List<RationalNumber> {0}, XName)};
            var saturatedSystem = Saturator.Saturate(polynomials).ToList();
            var table = new TarskiTable(saturatedSystem);
            var signs = table[polynomials[0]].ToList();

            var expected = new List<Sign> {Sign.EqualZero, Sign.EqualZero};

            Assert.Equal(expected, signs);
        }
    }
}