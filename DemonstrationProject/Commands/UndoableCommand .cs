using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DemonstrationProject.Commands
{
    public class UndoableCommand:IUndoableCommand
    {
        private readonly Action<object?> _execute;
        private readonly Action _undo;
        private readonly Predicate<object?>? _canExecute;

        public UndoableCommand(Action<object?> execute, Action undo, Predicate<object?>? canExecute = null)
        {
            _execute = execute;
            _undo = undo;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter) => _execute(parameter);

        public void Undo() => _undo();

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
