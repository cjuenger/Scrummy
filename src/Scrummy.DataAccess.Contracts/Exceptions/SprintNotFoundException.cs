using System;

namespace Scrummy.DataAccess.Contracts.Exceptions
{
    public class SprintNotFoundException : Exception
    {
        public SprintNotFoundException() {}

        public SprintNotFoundException(string message) : base(message) {}

        public SprintNotFoundException(string message, Exception innerException) : base(message, innerException) {}
    }
}