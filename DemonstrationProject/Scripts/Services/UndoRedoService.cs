using DemonstrationProject.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DemonstrationProject.Scripts.Services
{
    public class UndoRedoService
    {
        private readonly Stack<IUndoableCommand> _undoStack = new Stack<IUndoableCommand>();
        private readonly Stack<IUndoableCommand> _redoStack = new Stack<IUndoableCommand>();

        public bool CanUndo => _undoStack.Any();
        public bool CanRedo => _redoStack.Any();

        public void ExecuteCommand(IUndoableCommand command)
        {
            MessageBox.Show($"Executing command: {command.GetType().Name}");
            command.Execute(null); // Выполняем команду
            _undoStack.Push(command); // Добавляем команду в стек отмены
            _redoStack.Clear(); // Очищаем стек повтора
            MessageBox.Show($"Command executed. Undo stack size: {_undoStack.Count}, Redo stack size: {_redoStack.Count}");
        }

        public void Undo()
        {
            MessageBox.Show("Attempting Undo...");
            if (CanUndo)
            {
                var command = _undoStack.Pop(); // Извлекаем команду из стека отмены
                MessageBox.Show($"Undoing command: {command.GetType().Name}");
                command.Undo(); // Выполняем отмену команды
                _redoStack.Push(command); // Добавляем команду в стек повтора
                MessageBox.Show($"Undo successful. Undo stack size: {_undoStack.Count}, Redo stack size: {_redoStack.Count}");
            }
            else
            {
                MessageBox.Show("Cannot Undo. Undo stack is empty.");
            }
        }

        public void Redo()
        {
            MessageBox.Show("Attempting Redo...");
            if (CanRedo)
            {
                var command = _redoStack.Pop(); // Извлекаем команду из стека повтора
                MessageBox.Show($"Redoing command: {command.GetType().Name}");
                command.Execute(null); // Выполняем команду заново
                _undoStack.Push(command); // Добавляем команду в стек отмены
                MessageBox.Show($"Redo successful. Undo stack size: {_undoStack.Count}, Redo stack size: {_redoStack.Count}");
            }
            else
            {
                MessageBox.Show("Cannot Redo. Redo stack is empty.");
            }
        }
    }
} 