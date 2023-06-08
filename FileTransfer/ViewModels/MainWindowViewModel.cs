using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using FileTransfer.Command;
using FileTransfer.Models;
using FileTransfer.Services.Abstract;
using MessageBox = System.Windows.MessageBox;

namespace FileTransfer.ViewModels;

internal class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly IDialogService _dialogService;
    private readonly IFileService _fileService;
    private readonly IMemoryMappedFileService _memoryMappedService;
    private readonly IUserConfigService _userConfigService;
    private ObservableCollection<FileDescriptor> _filesDescriptors;
    private int _selectedIndex = -1;
    private FileDescriptor? _selectedDescriptor;
    private Visibility _isDescriptionVisible = Visibility.Hidden;

    #region Properties

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

            if (value == -1)
            {
                IsDescriptionVisible = Visibility.Hidden;
                SelectedDescriptor = null;
            }
            else
            {
                IsDescriptionVisible = Visibility.Visible;
                SelectedDescriptor = _filesDescriptors[value];
            }

            OnPropertyChanged();
        }
    }

    public FileDescriptor? SelectedDescriptor
    {
        get => _selectedDescriptor;
        set => SetField(ref _selectedDescriptor, value);
    }

    public Visibility IsDescriptionVisible
    {
        get => _isDescriptionVisible;
        set => SetField(ref _isDescriptionVisible, value);
    }

    public string DownloadFolder
    {
        get => _userConfigService.GetDownloadFolder();
    }

    #endregion

    public MainWindowViewModel(
        IDialogService dialogService,
        IFileService fileService,
        IMemoryMappedFileService memoryMappedService,
        IUserConfigService userConfigService)
    {
        _dialogService = dialogService;
        _fileService = fileService;
        _memoryMappedService = memoryMappedService;
        _userConfigService = userConfigService;

        InitFileDescriptors();

        #region Commands init

        LoadNewFileCommand = new AppCommand(UploadNewFile);
        RefreshFilesCommand = new AppCommand(RefreshFilesList);
        SelectDownloadFolderCommand = new AppCommand(SelectDownloadFolder);
        DownloadSelectedFileCommand = new AppCommand(DownloadSelectedFile);
        RemoveSelectedFileCommand = new AppCommand(RemoveSelectedFile);

        #endregion
    }

    ~MainWindowViewModel()
    {
        _memoryMappedService?.Dispose();
    }

    private void InitFileDescriptors()
    {
        var existedDescriptors = _memoryMappedService.LoadExistingDescriptors();
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

    private void UploadNewFile()
    {
        var selectedFileName = _dialogService.GetFileByDialog();

        if (string.IsNullOrEmpty(selectedFileName) || !File.Exists(selectedFileName))
        {
            return;
        }

        var fileInfo = new FileInfo(selectedFileName);
        var fileDescriptor = _fileService.GetFileDescriptorFromFileInfo(fileInfo);
        var existedDescriptors = _memoryMappedService.LoadExistingDescriptors();
        var mergedDescriptors = existedDescriptors.Append(fileDescriptor).ToArray();

        _memoryMappedService.CreateMemoryMappedFileFromFile(fileDescriptor.FileName, fileInfo.FullName,
            fileInfo.Length);
        _memoryMappedService.WriteDescriptorToMemoryFile(mergedDescriptors);
        InitFileDescriptors();

        var indexOfAddedFile = FilesDescriptors.IndexOf(fileDescriptor);
        SelectedIndex = indexOfAddedFile;
        MessageBox.Show($"File {selectedFileName} uploaded");
    }

    public ICommand RefreshFilesCommand { get; }

    private void RefreshFilesList()
    {
        InitFileDescriptors();
    }

    public ICommand SelectDownloadFolderCommand { get; }

    private void SelectDownloadFolder()
    {
        var selectedPath = _dialogService.GetFolderByDialog();

        if (string.IsNullOrEmpty(selectedPath) || !Directory.Exists(selectedPath))
        {
            return;
        }

        _userConfigService.SetDownloadFolder(selectedPath);
        OnPropertyChanged(nameof(DownloadFolder));
    }

    public ICommand DownloadSelectedFileCommand { get; }

    private void DownloadSelectedFile()
    {
        if (!_memoryMappedService.DoesSpecifiedDescriptorFileExist(SelectedDescriptor.FileName))
        {
            MessageBox.Show("File was deleted by creator. Files list refreshed");
            InitFileDescriptors();
            return;
        }

        var downloadPath = _userConfigService.GetDownloadFolder();
        var viewStream =
            _memoryMappedService.GetFileStreamFromMemoryMappedFile(SelectedDescriptor.FileName,
                SelectedDescriptor.FileLength);
        _fileService.SaveMemoryMappedStreamToLocalFile(viewStream, downloadPath, SelectedDescriptor.FileName,
            SelectedDescriptor.FileLength);
        MessageBox.Show($"File downloaded to: {downloadPath}");
    }

    public ICommand RemoveSelectedFileCommand { get; }

    private void RemoveSelectedFile()
    {
        if (!_memoryMappedService.IsFileHostedLocally(SelectedDescriptor.FileName))
        {
            MessageBox.Show(
                $"File {SelectedDescriptor.FileName} can't be deleted as it was created by another programm. Files list refreshed");
            InitFileDescriptors();

            return;
        }

        var existedDescriptors = _memoryMappedService.LoadExistingDescriptors();
        var filteredDescriptors = existedDescriptors.Where(d => d.FileName != SelectedDescriptor.FileName).ToArray();
        _memoryMappedService.WriteDescriptorToMemoryFile(filteredDescriptors);
        _memoryMappedService.RemoveFileFromStorageByName(SelectedDescriptor.FileName);
        InitFileDescriptors();
        MessageBox.Show($"File {SelectedDescriptor.FileName} removed");
        SelectedIndex = -1;
    }

    #endregion
}