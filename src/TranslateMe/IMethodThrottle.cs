using System;

namespace TranslateMe
{
    public interface IMethodThrottle : IDisposable
    {
        void CallDelayed();
    }

    public interface IMethodThrottle<in TArgument> : IDisposable
    {
        void CallDelayed(TArgument e);
    }
}