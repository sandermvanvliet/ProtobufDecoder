using System;
using System.Windows.Input;

namespace ProtobufDecoder.Application.Wpf.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Func<object, bool> _canExecute;
        private readonly Func<object, CommandResult> _execute;
        private Action<CommandResult> _onFailure;
        private Action<CommandResult> _onSuccess;
        private Action<CommandResult> _onSuccessWithWarnings;

        public RelayCommand(Func<object, CommandResult> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public object CommandParameter { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            var result = _execute(parameter);

            if (result.Result == Result.Success)
            {
                _onSuccess?.Invoke(result);
            }
            else if (result.Result == Result.SuccessWithWarnings)
            {
                _onSuccessWithWarnings?.Invoke(result);
            }
            else if (result.Result == Result.Failure)
            {
                _onFailure?.Invoke(result);
            }
        }

        public RelayCommand OnSuccess(Action<CommandResult> action)
        {
            _onSuccess = action;
            return this;
        }

        public RelayCommand OnSuccessWithWarnings(Action<CommandResult> action)
        {
            _onSuccessWithWarnings = action;
            return this;
        }

        public RelayCommand OnFailure(Action<CommandResult> action)
        {
            _onFailure = action;
            return this;
        }
    }
}