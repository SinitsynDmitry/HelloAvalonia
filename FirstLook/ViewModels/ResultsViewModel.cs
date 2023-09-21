/******************************************************************************
 *
 * File: ResultsViewModel.cs
 *
 * Description: ResultsViewModel.cs class and he's methods.
 *
 * Copyright (C) 2023 by Dmitry Sinitsyn
 *
 * Date: 18.8.2023	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using FirstLook.Data;
using MsBox.Avalonia.Enums;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FirstLook.ViewModels
{
    /// <summary>
    /// The results view model.
    /// </summary>
    internal class ResultsViewModel : ViewModelBase
    {
        #region Private Fields

        /// <summary>
        /// The data source service.
        /// </summary>
        private IDataSource dataSourceService;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultsViewModel"/> class.
        /// </summary>
        /// <param name="dataSourceService">The data source service.</param>
        public ResultsViewModel(IDataSource dataSourceService)
        {
            Header = "Results";
            this.dataSourceService = dataSourceService;
            TestValues = new NotifyTaskCompletion<List<TestValue>>(dataSourceService.GetDataAsync());
            TestValues.PropertyChanged += TestValues_PropertyChanged;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether data grid can be showed.
        /// </summary>
        public bool IsDataGridCanBeShowed { get { return TestValues.IsSuccessfullyCompleted && !IsDataSourceEmpty; } }

        /// <summary>
        /// Gets a value indicating whether data source is empty.
        /// </summary>
        public bool IsDataSourceEmpty { get { return TestValues.Result?.Count <= 0; } }

        /// <summary>
        /// Gets the test values.
        /// </summary>
        public NotifyTaskCompletion<List<TestValue>> TestValues { get; private set; }

        #endregion Public Properties

        #region Public Methods


        /// <summary>
        /// Exports test values.
        /// </summary>
        public async void Export()
        {
            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                var data = await dataSourceService.GetDataAsync();
                if (data?.Count > 0)
                {
                    var topLevel = desktopLifetime.MainWindow;
                    var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                    {
                        Title = "Exporting results",
                        DefaultExtension = ".json",
                    });

                    if (file is not null)
                    {
                        // Open writing stream from the file.
                        await using var stream = await file.OpenWriteAsync();
                        using var streamWriter = new StreamWriter(stream);


                        var newData = Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
                        // Write content to the file.
                        await streamWriter.WriteAsync(newData);
                    }
                }
                else
                {
                    await ((App)App.Current).ShowError("No data to export.");
                }
            }
        }

        /// <summary>
        /// Imports the log.
        /// </summary>
        public async void ImportLog()
        {
            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                // Get top level from the current control.
                var topLevel = desktopLifetime.MainWindow;

                // Start async operation to open the dialog.
                var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Select log file",
                    FileTypeFilter = new[] {
                    new FilePickerFileType("Log file")
                    {
                        Patterns = new[] { "*.log" },
                        MimeTypes = new[] { "text/plain" }
                    }
                },

                    AllowMultiple = false
                });

                if (files.Count >= 1)
                {
                    // Open reading stream from the first file.
                    await using var stream = await files[0].OpenReadAsync();
                    using var streamReader = new StreamReader(stream);
                    // Reads all the content of file as a text.
                    var fileContent = await streamReader.ReadToEndAsync();
                    if (!string.IsNullOrWhiteSpace(fileContent))
                    {
                        try
                        {
                            var data = TestValue.ExtractData(fileContent);
                            if (data.IsValid())
                            {
                                var old = await dataSourceService.GetDataAsync();
                                var find = old.Any(x => x.IsSimular(data));
                                bool yes = true;
                                if (find)
                                {
                                    var result = await ((App)App.Current).ShowQuestion("Very similar data is already present in the list.\nAre you sure you want to add them?");
                                    if (result == ButtonResult.No)
                                    {
                                        yes = false;
                                    }
                                }

                                if (yes)
                                    await dataSourceService.AddDataAsync(data);

                                TestValues.PropertyChanged -= TestValues_PropertyChanged;
                                TestValues = new NotifyTaskCompletion<List<TestValue>>(dataSourceService.GetDataAsync());
                                TestValues.PropertyChanged += TestValues_PropertyChanged;
                            }
                            else
                            {
                                await ((App)App.Current).ShowError("The file does not contain the correct data.");
                            }
                        }
                        catch
                        {
                            await ((App)App.Current).ShowError("The file does not contain the correct data.");
                        }
                    }
                    else
                    {
                        await ((App)App.Current).ShowError("The file is empty.");
                    }
                }
            }
        }

        /// <summary>
        /// Saving changes asynchronously.
        /// </summary>
        /// <returns>A Task.</returns>
        public async Task SaveAsync()
        {
            var list = TestValues.Result;
            await dataSourceService.SaveDataAsync(list);
        }

        #endregion Public Methods

        #region Private Methods

        private void TestValues_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged("TestValues");
            this.OnPropertyChanged("IsDataSourceEmpty");
            this.OnPropertyChanged("IsDataGridCanBeShowed");
        }

        #endregion Private Methods
    }
}