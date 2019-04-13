using System;
using System.Windows.Input;

namespace Debugger
{
    public class Command : ICommand
    {
        /// <summary>
        /// Инициализация нового экземпляра класса без параметров <see cref="Command"/>.
        /// </summary>
        /// <param name="action">Действие.</param>
        /// <param name="canExecute">Если установлено в<c>true</c> [can execute] (выполнение разрешено).</param>
        public Command(Action action, bool canExecute = true)
        {
            //  Set the action.
            Action = action;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Инициализация нового экземпляра класса с параметрами <see cref="Command"/> class.
        /// </summary>
        /// <param name="parameterizedAction">Параметризированное действие.</param>
        /// <param name="canExecute"> Если установлено в <c>true</c> [can execute](выполнение разрешено).</param>
        public Command(Action<object> parameterizedAction, bool canExecute = true)
        {
            //  Set the action.
            ParameterizedAction = parameterizedAction;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Действие(или параметризованное действие) которое вызывается при активации команды.
        /// </summary>
        protected Action Action;

        protected Action<object> ParameterizedAction = null;

        /// <summary>
        /// Будевое значение, отвечающие за возможность выполнения команды.
        /// </summary>
        private bool _canExecute = false;

        /// <summary>
        /// Установка /  получение значения, отвечающего за возможность выполнения команды
        /// </summary>
        /// <value>
        ///     <c>true</c> если выполнение разрешено; если запрещено - <c>false</c>.
        /// </value>
        public bool CanExecute
        {
            get => _canExecute;
            set
            {
                if (_canExecute == value) return;

                _canExecute = value;

                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Определяем метод, определющий, что выполнение команды допускается в текущем состоянии
        /// </summary>
        /// <param name="parameter">Этот параметр используется командой.
        ///  Если команда вызывается без использования параметра,
        ///  то этот объект может быть установлен в  null.</param>
        /// <returns>
        /// > если выполнение команды разрешено; если запрещено - false.
        /// </returns>
        bool ICommand.CanExecute(object parameter)
        {
            return _canExecute;
        }

        /// <summary>
        /// Задание метода, который будет вызван при активации команды.
        /// </summary>
        /// <param name="parameter"> Этот параметр используется командой.
        ///  Если команда вызывается без использования параметра,
        ///  то этот объект может быть установлен в  null.</param>
        void ICommand.Execute(object parameter)
        {
            DoExecute(parameter);

        }

        /// <summary>
        ///  Вызывается, когда меняется возможность выполнения команды
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Вызывается во время выполнения команды
        /// </summary>
        public event CancelCommandEventHandler Executing;

        /// <summary>
        /// Вызывается, когда команды выполнена
        /// </summary>
        public event CommandEventHandler Executed;

        protected void InvokeAction(object param)
        {
            Action theAction = Action;
            Action<object> theParameterizedAction = ParameterizedAction;
            if (theAction != null)
                theAction();
            else theParameterizedAction?.Invoke(param);
        }

        protected void InvokeExecuted(CommandEventArgs args)
        {
            Executed?.Invoke(this, args);
        }

        protected void InvokeExecuting(CancelCommandEventArgs args)
        {
            CancelCommandEventHandler executing = Executing;

            //  Call the executed event.
            executing?.Invoke(this, args);
        }

        /// <summary>
        /// Выполнение команды
        /// </summary>
        /// <param name="param">The param.</param>
        public virtual void DoExecute(object param)
        {
            //  Вызывает выполнении команды с возможностью отмены
            CancelCommandEventArgs args =
                new CancelCommandEventArgs() { Parameter = param, Cancel = false };
            InvokeExecuting(args);

            //  Если событие было отменено -  останавливаем.
            if (args.Cancel)
                return;

            //  Вызываем действие с / без параметров, в зависимости от того. Какое было устанвленно.
            InvokeAction(param);

            //  Call the executed function.
            InvokeExecuted(new CommandEventArgs() { Parameter = param });
        }
    }

    public delegate void CommandEventHandler(object sender, CommandEventArgs args);
    public delegate void CancelCommandEventHandler(object sender, CancelCommandEventArgs args);
    public class CommandEventArgs : EventArgs
    {
        public object Parameter { get; set; }
    }
    public class CancelCommandEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
        public object Parameter { get; set; }
    }
}