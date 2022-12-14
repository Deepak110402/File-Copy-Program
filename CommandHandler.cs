using System;
using System.Windows.Input;

namespace FileCopyProgram
{
    public class CommandHandler : ICommand
    {
        private readonly Action action;
        private readonly Func<bool> canExecute;

        /// <summary>
        /// Creates instance of the command handler
        /// </summary>
        /// <param name="action">Action to be executed by the command</param>
        /// <param name="canExecute">A bolean property to containing current permissions to execute the command</param>
        public CommandHandler(Action action, Func<bool> canExecute)
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Wires CanExecuteChanged event 
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Forcess checking if execute is allowed
        /// </summary>
        /// <param name="parameter">object</param>
        /// <returns>returns the canExecute value</returns>
        public bool CanExecute(object parameter)
        {
            return canExecute.Invoke();
        }

        /// <summary>
        /// performs main action
        /// </summary>
        /// <param name="parameter">object</param>
        public void Execute(object parameter)
        {
            action();
        }
    }
}
