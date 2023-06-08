using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using FileTransfer.Command;
using FileTransfer.Models;
using FileTransfer.Services.Abstract;

namespace FileTransfer.ViewModels;

internal class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly IDialogService _dialogService;
    private readonly IFileService _fileService;
    private readonly IMemoryMappedFileService _memoryMappedService;
    private ObservableCollection<FileDescriptor> _filesDescriptors;
    private int _selectedIndex = -1;
    private FileDescriptor _selectedDescriptor;
    private Visibility _isDescriptionVisible = Visibility.Hidden;

    public ObservableCollection<FileDescriptor> FilesDescriptors
    {
        get => _filesDescriptors;
        set => SetField(ref _filesDescriptors, value);
    }

    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (value == _selectedIndex) return;
            _selectedIndex = value;
            SelectedDescriptor = _filesDescriptors[value];
            IsDescriptionVisible = value == -1 ? Visibility.Hidden : Visibility.Visible;
            OnPropertyChanged();
        }
    }

    public FileDescriptor SelectedDescriptor
    {
        get => _selectedDescriptor;
        set => SetField(ref _selectedDescriptor, value);
    }

    public Visibility IsDescriptionVisible
    {
        get => _isDescriptionVisible;
        set => SetField(ref _isDescriptionVisible, value);
    }

    public MainWindowViewModel(IDialogService dialogService, IFileService fileService, IMemoryMappedFileService memoryMappedService)
    {
        _dialogService = dialogService;
        _fileService = fileService;
        _memoryMappedService = memoryMappedService;

        LoadFileDescriptors();

        #region Commands init
        LoadNewFileCommand = new AppCommand(LoadNewFile);
        RefreshFilesCommand = new AppCommand(RefreshFilesList);
        SelectDownloadFolderCommand = new AppCommand(SelectDownloadFolder);
        #endregion
    }

    ~MainWindowViewModel()
    {
        _memoryMappedService?.Dispose();
    }

    private void LoadFileDescriptors()
    {
        var existedDescriptors = _memoryMappedService.GetExistingDescriptors();
        FilesDescriptors = new ObservableCollection<FileDescriptor>(existedDescriptors);
    }

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

    private void LoadNewFile()
    {
        var selectedFileName = _dialogService.SelectFileDialog();

        if (string.IsNullOrEmpty(selectedFileName) || !File.Exists(selectedFileName))
        {
            return;
        }

        var fileInfo = new FileInfo(selectedFileName);
        var fileDescriptor = _fileService.GetFileDescriptorFromFileInfo(fileInfo);
        var existedDescriptors = _memoryMappedService.GetExistingDescriptors();
        var mergedDescriptors = existedDescriptors.Append(fileDescriptor).ToArray();

        _memoryMappedService.WriteDescriptorToFile(mergedDescriptors);
        _memoryMappedService.CreateMemoryMappedFileFromFile(fileDescriptor.FileName, fileInfo.FullName);

        LoadFileDescriptors();

        var indexOfAddedFile = FilesDescriptors.IndexOf(fileDescriptor);
        SelectedIndex = indexOfAddedFile;
    }

    public ICommand RefreshFilesCommand { get; }

    private void RefreshFilesList()
    {
        LoadFileDescriptors();
    }

    public ICommand SelectDownloadFolderCommand { get; }

    private void SelectDownloadFolder()
    {

    }

    #endregion
}
