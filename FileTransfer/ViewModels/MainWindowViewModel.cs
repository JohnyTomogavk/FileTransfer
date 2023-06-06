using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FileTransfer.Command;
using FileTransfer.Services;

namespace FileTransfer.ViewModels;

internal class MainWindowViewModel : INotifyPropertyChanged
{
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

    #region Commands

    public ICommand LoadNewFileCommand { get; }

    private bool CanLoadNewFile() => true;

    private void LoadNewFile()
    {
        var selectedFileName = DialogService.GetFileNameFromUser();

        if (string.IsNullOrEmpty(selectedFileName))
        {
            return;
        }

        if (File.Exists(selectedFileName))
        {
            var fileInfo = new FileInfo(selectedFileName);
            var fileDescriptor= _fileService.GetFileDescriptor(fileInfo);
            
            // Write it to mapped file with descriptor
            // Write related file content to another mapped file
            // Refresh side menu items
            // Select just loaded item nad open description
        }
    }

    #endregion

    public MainWindowViewModel()
    {
        _dialogService = new DialogService();
        _fileService = new FileService();
        _memoryMappedFile = new MemoryMappedFileService();

        #region Commands init
        LoadNewFileCommand = new AppCommand(LoadNewFile, CanLoadNewFile);
        #endregion
    }

    private readonly DialogService _dialogService;
    private readonly FileService _fileService;
    private readonly MemoryMappedFileService _memoryMappedFile;
}
