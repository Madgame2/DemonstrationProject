using DemonstrationProject.Commands;
using System.Collections.Generic;
using System.Linq;

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
            command.Execute(null); // Выполняем команду
            _undoStack.Push(command); // Добавляем команду в стек отмены
            _redoStack.Clear(); // Очищаем стек повтора
        }

        public void Undo()
        {
            if (CanUndo)
            {
                var command = _undoStack.Pop(); // Извлекаем команду из стека отмены
                command.Undo(); // Выполняем отмену команды
                _redoStack.Push(command); // Добавляем команду в стек повтора
            }
        }

        public void Redo()
        {
            if (CanRedo)
            {
                var command = _redoStack.Pop(); // Извлекаем команду из стека повтора
                command.Execute(null); // Выполняем команду заново
                _undoStack.Push(command); // Добавляем команду в стек отмены
            }
        }
    }
} 