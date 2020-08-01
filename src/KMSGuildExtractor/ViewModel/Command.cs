using System;
using System.Windows.Input;

namespace KMSGuildExtractor.ViewModel
{
    public class Command : ICommand
    {
#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;

        private readonly Action<object> _executeMethod;
        private readonly Func<object, bool> _canexecuteMethod;

        public Command(Action<object> executeMethod, Func<object, bool> canexecuteMethod)
        {
            _executeMethod = executeMethod;
            _canexecuteMethod = canexecuteMethod;
        }

        public bool CanExecute(object parameter) => _canexecuteMethod(parameter);

        public void Execute(object parameter) => _executeMethod(parameter);
    }
}
