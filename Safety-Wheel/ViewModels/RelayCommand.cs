using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Safety_Wheel.ViewModels
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Action<object> _executeObj;        
        private readonly Func<bool> _canExecute;
        public RelayCommand(Action<object> execute) => _executeObj = execute;
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => _executeObj(parameter);
        public event EventHandler CanExecuteChanged;
    }
}
