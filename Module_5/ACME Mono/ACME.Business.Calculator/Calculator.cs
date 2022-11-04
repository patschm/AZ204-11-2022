using ACME.Business.Interfaces;
using System;

namespace ACME.Business.Calculator
{
    public class Calculator : ICalculator
    {
        public int Add(int a, int b)
        {
            return a + b;
        }
    }
}
