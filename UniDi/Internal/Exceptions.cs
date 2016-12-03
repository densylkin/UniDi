using System;

namespace UniDi
{
    public class RegistrationException : Exception
    {
        public RegistrationException(string message) : base(message)
        {
        }
    }

    public class CyclicDependencyException : Exception
    {
        public CyclicDependencyException(string message) : base(message)
        {
        }
    }

    public class ResolvingException : Exception
    {
        public ResolvingException(string message) : base(message)
        {
        }
    }

    public class InjectionException : Exception
    {
        public InjectionException(string message) : base(message)
        {
        }
    }
}