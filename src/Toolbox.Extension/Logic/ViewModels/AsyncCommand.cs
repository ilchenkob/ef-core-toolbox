using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Toolbox.Extension.Logic.ViewModels
{
    public interface IAsyncCommand : IAsyncCommand<object>
    {
    }

    public interface IAsyncCommand<in T>
    {
        Task ExecuteAsync(T obj);
        bool CanExecute(object obj);
        ICommand Command { get; }
    }

    public class AsyncCommand : AsyncCommand<object>, IAsyncCommand
    {
        public AsyncCommand(Func<Task> executeMethod)
            : base(o => executeMethod())
        {
        }

        public AsyncCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod)
            : base(o => executeMethod(), o => canExecuteMethod())
        {
        }
    }

    public class AsyncCommand<T> : IAsyncCommand<T>, ICommand
    {
        private readonly Func<T, Task> _executeMethod;
        private readonly Command _underlyingCommand;
        private bool _isExecuting;

        public AsyncCommand(Func<T, Task> executeMethod)
            : this(executeMethod, _ => true)
        {
        }

        public AsyncCommand(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod)
        {
            _executeMethod = executeMethod;
            _underlyingCommand = new Command(() => { });
        }

        public async Task ExecuteAsync(T obj)
        {
            try
            {
                _isExecuting = true;
                await _executeMethod(obj);
            }
            finally
            {
                _isExecuting = false;
            }
        }

        public ICommand Command { get { return this; } }

        public bool CanExecute(object parameter)
        {
            return !_isExecuting && _underlyingCommand.CanExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { _underlyingCommand.CanExecuteChanged += value; }
            remove { _underlyingCommand.CanExecuteChanged -= value; }
        }

        public void Execute(object parameter) =>
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            ExecuteAsync((T)parameter);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

    }
}
