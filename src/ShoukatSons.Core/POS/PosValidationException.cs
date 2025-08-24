// File: src/ShoukatSons.Core/POS/PosValidationException.cs
using System;

namespace ShoukatSons.Core.POS
{
    public class PosValidationException : Exception
    {
        public PosValidationException() { }
        public PosValidationException(string message) : base(message) { }
        public PosValidationException(string message, Exception inner) : base(message, inner) { }
    }
}