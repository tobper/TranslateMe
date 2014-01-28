using System;
using System.Windows;
using System.Windows.Input;

namespace TranslateMe.Commands.Support
{
    public abstract class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;

        void ICommand.Execute(object parameter)
        {
            Execute();
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        public abstract void Execute();

        public virtual bool CanExecute()
        {
            return true;
        }
    }

    public abstract class Command<TParameter> : ICommand
        where TParameter : class
    {
        public event EventHandler CanExecuteChanged;

        void ICommand.Execute(object parameter)
        {
            Execute(parameter as TParameter);
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute(parameter as TParameter);
        }

        public abstract void Execute(TParameter parameter);

        public virtual bool CanExecute(TParameter parameter)
        {
            return true;
        }
    }

    public abstract class Command<TParameter, TTarget> : ICommand
        where TParameter : class
        where TTarget : class, IInputElement
    {
        public event EventHandler CanExecuteChanged;

        void ICommand.Execute(object parameter)
        {
            Execute(
                parameter as TParameter,
                Keyboard.FocusedElement as TTarget);
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute(
                parameter as TParameter,
                Keyboard.FocusedElement as TTarget);
        }

        public abstract void Execute(TParameter parameter, TTarget target);

        public virtual bool CanExecute(TParameter parameter, TTarget target)
        {
            return true;
        }
    }
}