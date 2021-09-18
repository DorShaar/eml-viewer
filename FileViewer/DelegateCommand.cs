using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileViewer
{
    public class DelegateCommand : DelegateCommand<object>
    {
        public DelegateCommand(Func<Task> executeMethod)
            : base(o => executeMethod())
        {
        }

        public DelegateCommand(Action executeMethod)
            : base(o => executeMethod())
        {
        }

        public DelegateCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod)
            : base(o => executeMethod(), o => canExecuteMethod())
        {
        }

        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : base(o => executeMethod(), o => canExecuteMethod())
        {

        }
    }

    /// <summary>
    /// A command that calls the specified delegate when the command is executed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DelegateCommand<T> : ICommand
    {
        private readonly Func<T, bool> mCanExecuteMethod;
        private readonly Action<T> mExecuteMethod;
        private bool mIsExecuting;

        public DelegateCommand(Action<T> executeMethod)
            : this(executeMethod, null)
        {
        }

        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
        {
            if ((executeMethod == null) && (canExecuteMethod == null))
            {
                throw new ArgumentNullException("executeMethod", "Execute Method cannot be null");
            }

            mExecuteMethod = executeMethod;
            mCanExecuteMethod = canExecuteMethod;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        bool ICommand.CanExecute(object parameter)
        {
            return !mIsExecuting && CanExecute((T)parameter);
        }

        void ICommand.Execute(object parameter)
        {
            mIsExecuting = true;
            try
            {
                RaiseCanExecuteChanged();
                Execute((T)parameter);
            }
            finally
            {
                mIsExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public bool CanExecute(T parameter)
        {
            bool canExecute;
            if (mCanExecuteMethod == null)
            {
                canExecute = true;
            }
            else
            {
                canExecute = mCanExecuteMethod(parameter);
            }

            return canExecute;
        }

        public void Execute(T parameter)
        {
            mExecuteMethod.Invoke(parameter);
        }
    }
}
