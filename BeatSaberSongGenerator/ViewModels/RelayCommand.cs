using System;
using System.Windows.Input;

namespace BeatSaberSongGenerator.ViewModels
{
    public class RelayCommand : ICommand
    {
        private readonly Action action;
        private readonly Func<bool> canExecute;


        public RelayCommand(Action action)
            : this(action, () => true)
        {}
        public RelayCommand(Action action, Func<bool> canExecute)
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute();
        }

        public void Execute(object parameter)
        {
            action();
        }

        public event EventHandler CanExecuteChanged    
        {    
            add { CommandManager.RequerySuggested += value; }    
            remove { CommandManager.RequerySuggested -= value; }    
        } 
    }
}