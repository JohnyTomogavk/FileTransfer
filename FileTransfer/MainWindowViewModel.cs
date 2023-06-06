using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using FileTransfer.Command;

namespace FileTransfer;

internal class MainWindowViewModel : INotifyPropertyChanged
{
    private int _counter;

    #region Notify property changed implementation

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);

        return true;
    }

    #endregion

    public ICommand IncCommand { get; }

    public MainWindowViewModel()
    {
        Counter = 0;
        IncCommand = new AppCommand(() =>
        {
            Counter += 1;
        }, () => true);
    }

    public int Counter
    {
        get => _counter;
        set
        {
            if (value == _counter) return;
            _counter = value;
            OnPropertyChanged();
        }
    }
}