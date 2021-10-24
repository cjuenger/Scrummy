using System;

namespace Scrummy.GitLab.Contracts.Exceptions
{
    public class MissingBoardException : Exception
    {
        public MissingBoardException() {}

        public MissingBoardException(string message) : base(message) {}

        public MissingBoardException(string message, Exception innerException) : base(message, innerException) {}
    }
}