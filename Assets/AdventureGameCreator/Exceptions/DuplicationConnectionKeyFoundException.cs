using System;

namespace AdventureGameCreator.Exceptions
{
    public class DuplicateConnectionKeyFoundException : Exception
    {
        public DuplicateConnectionKeyFoundException()
        {

        }

        public DuplicateConnectionKeyFoundException(string message) : base(message)
        {

        }

        public DuplicateConnectionKeyFoundException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
