using ACME.Business.Calculator;
using System;
using Xunit;

namespace ACME.Tests.Unit.CalculatorTests
{
    public class CalculatorTests
    {
        [Theory]
        [InlineData(1, 2, 3)]
        [InlineData(-4, 5, 1)]
        [InlineData(-3, -2, -5)]
        public void TestAdd(int a, int b, int expected)
        {
            Calculator calc = new Calculator();
            int actual = calc.Add(a, b);
            Assert.Equal(expected, actual);
        }
    }
}
