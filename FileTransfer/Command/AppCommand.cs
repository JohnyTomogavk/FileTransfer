using System;
using System.Windows.Input;

namespace FileTransfer.Command;

internal class AppCommand : ICommand
{
    private Action execute;
    private Func<bool> canExecute;

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public AppCommand(Action execute, Func<bool> canExecute = null)
    {
        this.execute = execute;
        this.canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return this.canExecute == null || this.canExecute();
    }

    public void Execute(object parameter)
    {
        this.execute();
    }
}