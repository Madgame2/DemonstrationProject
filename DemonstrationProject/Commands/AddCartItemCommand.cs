using DemonstrationProject.DB;
using DemonstrationProject.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DemonstrationProject.Commands
{
    public class AddCartItemCommand : IUndoableCommand
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly Cart _cartItem;
        private int _addedCartItemId = -1; // Для хранения ID добавленного элемента

        public event EventHandler? CanExecuteChanged;

        public AddCartItemCommand(UnitOfWork unitOfWork, Cart cartItem)
        {
            _unitOfWork = unitOfWork;
            _cartItem = cartItem;
        }

        public bool CanExecute(object? parameter)
        {
            // Логика, определяющая, может ли команда быть выполнена
            return _cartItem != null;
        }

        public async void Execute(object? parameter)
        {
            try
            {
                // Выполняем добавление и получаем ID
                _addedCartItemId = await _unitOfWork.Carts.AddAsync(_cartItem);
                await _unitOfWork.CommitAsync();
                
                // Возможно, здесь нужно обновить UI через событие или что-то подобное
                // Например, вызвать LoadCartItemsAsync в CartControlViewModel
                // _unitOfWork.OnDataChanged(); // Если событие публичное и вызывается UnitOfWork

            }
            catch (Exception)
            {
                // Обработка ошибок, возможно, откат транзакции
                await _unitOfWork.RollbackAsync();
                // Можно пробросить исключение или обработать его другим способом
                // throw;
            }
        }

        public async void Undo()
        {
            if (_addedCartItemId != -1)
            {
                try
                {
                    // Удаляем элемент по его ID
                    await _unitOfWork.Carts.RemoveAsync(_addedCartItemId);
                    await _unitOfWork.CommitAsync();

                    // Возможно, здесь нужно обновить UI
                    // _unitOfWork.OnDataChanged();

                    _addedCartItemId = -1; // Сбрасываем ID после отмены
                }
                catch (Exception)
                {
                    // Обработка ошибок при отмене
                    await _unitOfWork.RollbackAsync();
                    // throw;
                }
            }
        }

        // Этот метод нужен для ICommand, но CanExecuteChanged обрабатывается CommandManager
        protected virtual void OnCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
} 