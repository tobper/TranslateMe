using System;
using System.Windows;
using System.Windows.Threading;

namespace TranslateMe
{
    public class MethodThrottle : IMethodThrottle
    {
        private readonly Action _action;
        private readonly DispatcherTimer _timer;

        public MethodThrottle(Action action, TimeSpan interval)
        {
            _action = action;
            _timer = new DispatcherTimer(interval, DispatcherPriority.Background, Completed, Application.Current.Dispatcher)
            {
                IsEnabled = false
            };
        }

        public void CallDelayed()
        {
            _timer.Stop();
            _timer.Start();
        }

        public void Dispose()
        {
            if (_timer.IsEnabled)
            {
                CallAction();
            }
        }

        private void Completed(object sender, EventArgs eventArgs)
        {
            CallAction();
        }

        private void CallAction()
        {
            _timer.Stop();
            _action();
        }
    }

    public class MethodThrottle<TArgument> : IMethodThrottle<TArgument>
    {
        private readonly Action<TArgument> _action;
        private readonly DispatcherTimer _timer;
        private TArgument _lastArgument;

        public MethodThrottle(Action<TArgument> action, TimeSpan interval)
        {
            _action = action;
            _timer = new DispatcherTimer(interval, DispatcherPriority.Background, Completed, Application.Current.Dispatcher)
            {
                IsEnabled = false
            };
        }

        public void CallDelayed(TArgument e)
        {
            _timer.Stop();
            _lastArgument = e;
            _timer.Start();
        }

        public void Dispose()
        {
            if (_timer.IsEnabled)
            {
                CallAction();
            }
        }

        private void Completed(object sender, EventArgs eventArgs)
        {
            CallAction();
        }

        private void CallAction()
        {
            _timer.Stop();
            _action(_lastArgument);
        }
    }
}