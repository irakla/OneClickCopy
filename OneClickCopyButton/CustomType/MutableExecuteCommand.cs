using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OneClickCopy.CustomType
{
    /// <summary>
    /// A Command whose sole purpose is to expose Execute setter so that make it changable.
    /// It allows to set Execute after its construction. it makes that its invoker can invoke encapsulated Execute.
    /// like event but doesn't need subscription/unsubscription.
    /// </summary>
    public class MutableExecuteCommand : ICommand
    {
        private readonly Func<bool> canExecute;
        private Action _execute;

        public Action MutableExecute { get => _execute; set => _execute = value; }

        public event EventHandler CanExecuteChanged;

        public MutableExecuteCommand() : this(null, null) { }
        public MutableExecuteCommand(Action execute) : this(execute, null) { }
        public MutableExecuteCommand(Action execute, Func<bool> canExecute)
        {
            this._execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (canExecute == null)
                return true;

            return canExecute();
        }

        public void Execute(object parameter) => _execute?.Invoke();

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);  
        }
    }
}
