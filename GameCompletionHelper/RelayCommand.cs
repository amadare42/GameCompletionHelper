using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GameCompletionHelper
{
    public class RelayCommand : ICommand
    {
        public Action<object> Action { get; private set; }

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<object> action)
        {
            this.Action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.Action(parameter);
        }
    }
}
