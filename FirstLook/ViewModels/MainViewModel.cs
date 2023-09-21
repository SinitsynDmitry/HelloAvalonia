/******************************************************************************
 *
 * File: MainViewModel.cs
 *
 * Description: MainViewModel.cs class and he's methods.
 *
 * Copyright (C) 2023 by Dmitry Sinitsyn
 *
 * Date: 18.8.2023	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

using Avalonia.Controls.ApplicationLifetimes;
using FirstLook.Data;
using MsBox.Avalonia.Enums;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FirstLook.ViewModels;
/// <summary>
/// The main view model.
/// </summary>

public partial class MainViewModel : ViewModelBase
{
    #region Private Fields

    private ViewModelBase _contentViewModel;
    private IDataSource dataSourceService;
    private bool isTestRunning;

    #endregion Private Fields

    #region Public Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    /// <param name="dataSourceService">The data source service.</param>
    public MainViewModel(IDataSource dataSourceService)
    {
        this.dataSourceService = dataSourceService;
        _contentViewModel = new HomeViewModel();
    }

    #endregion Public Constructors

    #region Public Properties

    /// <summary>
    /// Gets the content view model.
    /// </summary>
    public ViewModelBase ContentViewModel
    {
        get => _contentViewModel;
        private set => this.SetProperty(ref _contentViewModel, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether test is canceled.
    /// </summary>
    public bool IsTestCanceled { get; set; }

    /// <summary>
    /// Gets a value indicating whether test is running.
    /// </summary>
    public bool IsTestRunning
    {
        get => isTestRunning;
        private set => this.SetProperty(ref isTestRunning, value);
    }

    #endregion Public Properties

    #region Public Methods

    /// <summary>
    /// Cancels the test.
    /// </summary>
    public void CancelTest()
    {
        IsTestCanceled = true;
    }

    /// <summary>
    /// Exits the.
    /// </summary>
    public async void Exit()
    {
        if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            var result = await ((App)App.Current).ShowQuestion("Are you sure you want to exit?");
            if (result == ButtonResult.Yes)
                desktopLifetime.MainWindow?.Close();
        }
    }

    /// <summary>
    /// Opens the home.
    /// </summary>
    public void OpenHome()
    {
        ContentViewModel = new HomeViewModel();
    }

    /// <summary>
    /// Opens the results.
    /// </summary>
    public void OpenResults()
    {
        ContentViewModel = new ResultsViewModel(dataSourceService);
    }

    /// <summary>
    /// Opens the settings.
    /// </summary>
    public void OpenSettings()
    {
        ContentViewModel = new SettingsViewModel();
    }

    /// <summary>
    /// Starts the test.
    /// </summary>
    public async void StartTest()
    {
        try
        {
            IsTestRunning = true;
            IsTestCanceled = false;
            if (!string.IsNullOrWhiteSpace(Settings.Default.TestPath) && File.Exists(Settings.Default.TestPath))
            {
                Process testProcess = Process.Start(Settings.Default.TestPath);

                Task task = Task.Run(() =>
                {
                    Thread.Sleep(3000);

                    string logFilePath = Settings.Default.TestPath.Replace(".exe", ".log");
                    bool isLogRecorded = false;

                    while (!isLogRecorded && !IsTestCanceled)//wait for Cancel
                    {
                        Thread.Sleep(1000); // Wait for 1 second (adjust as needed)

                        if (Settings.Default.IsCheckLog && File.Exists(logFilePath))//if the user wants - check the log for the last entry -> and close the test process
                        {
                            try
                            {
                                using (FileStream stream = File.Open(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                {
                                    using (StreamReader reader = new StreamReader(stream))
                                    {
                                        string logContents = reader.ReadToEnd();
                                        if (logContents.Contains("Query accuracy:"))//last entry
                                        {
                                            isLogRecorded = true;
                                        }
                                    }
                                }
                            }
                            catch (IOException ex)
                            {
                                // The file is still locked; continue waiting
                            }
                        }

                        if (testProcess.HasExited)
                        {
                            break; // Break the loop if the process exits before log is recorded OR Cancel
                        }
                    }

                    // Close the process
                    if (!testProcess.HasExited)
                    {
                        testProcess.CloseMainWindow();
                        if (!testProcess.WaitForExit(5000)) // Wait for the process to exit
                        {
                            testProcess.Kill(); // Kill the process if it doesn't exit gracefully
                        }
                    }
                });
                await task;

                testProcess.WaitForExit();
                if (!IsTestCanceled)
                {
                    var clipboard = await DoGetClipboardTextAsync();// get clipboard text
                    if (!string.IsNullOrWhiteSpace(clipboard))
                    {
                        try
                        {
                            var data = TestValue.ExtractData(clipboard);
                            if (data.IsValid())
                            {
                                await dataSourceService.AddDataAsync(data);
                            }
                            else
                            {
                                await ((App)App.Current).ShowError("The clipboard does not contain the expected data.");
                            }
                        }
                        catch
                        {
                            await ((App)App.Current).ShowError("The clipboard does not contain the expected data.");
                        }
                    }
                    else
                    {
                        await ((App)App.Current).ShowError("The clipboard is empty.");
                    }

                    ContentViewModel = new ResultsViewModel(dataSourceService);
                }
            }
            else
            {
                await ((App)App.Current).ShowError("Test application is not available.\nPlease review your settings.");
            }
        }
        catch (Exception e)
        {
            await ((App)App.Current).ShowError(e.Message);
        }
        finally
        {
            IsTestCanceled = IsTestRunning = false;
        }
    }

    #endregion Public Methods

    #region Private Methods

    /// <summary>
    /// Get clipboard text async.
    /// </summary>
    /// <returns>A Task.</returns>
    private async Task<string?> DoGetClipboardTextAsync()
    {
        if (App.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.Clipboard is not { } provider)
            throw new NullReferenceException("Missing Clipboard instance.");

        return await provider.GetTextAsync();
    }

    #endregion Private Methods
}