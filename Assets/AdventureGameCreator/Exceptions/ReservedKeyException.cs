using System;

namespace AdventureGameCreator.Exceptions
{
    public class ReservedKeyException : Exception
    {
        public ReservedKeyException()
        {

        }

        public ReservedKeyException(string message) : base(message)
        {

        }

        public ReservedKeyException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
