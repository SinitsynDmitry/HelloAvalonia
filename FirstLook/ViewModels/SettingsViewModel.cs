/******************************************************************************
 *
 * File: SettingsViewModel.cs
 *
 * Description: SettingsViewModel.cs class and he's methods.
 *
 * Copyright (C) 2023 by Dmitry Sinitsyn
 *
 * Date: 18.8.2023	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using System.Collections.Generic;

namespace FirstLook.ViewModels
{
    /// <summary>
    /// The settings view model.
    /// </summary>
    internal class SettingsViewModel : ViewModelBase
    {
        /// <summary>
        /// test path.
        /// </summary>
        private string _testPath = "";

        /// <summary>
        /// The theme.
        /// </summary>
        private string _theme = "";

        /// <summary>
        /// The themes.
        /// </summary>
        private List<string> _themes = new List<string>() { "Dark", "Light" };

        /// <summary>
        /// is check log.
        /// </summary>
        private bool isCheckLog;

        /// <summary>
        /// Gets the themes.
        /// </summary>
        public List<string> Themes
        {
            get => _themes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        public SettingsViewModel()
        {
            Header = "Settings";
            _testPath = Settings.Default.TestPath;
            _theme = Settings.Default.Theme;
            isCheckLog = Settings.Default.IsCheckLog;
        }

        /// <summary>
        /// Gets or Sets the test path.
        /// </summary>
        public string TestPath
        {
            get => _testPath;
            set => this.SetProperty(ref _testPath, value);
        }

        /// <summary>
        /// Gets or Sets a value indicating whether check is log.
        /// </summary>
        public bool IsCheckLog
        {
            get => isCheckLog;
            set => this.SetProperty(ref isCheckLog, value);
        }

        /// <summary>
        /// Gets or Sets the theme.
        /// </summary>
        public string Theme
        {
            get => _theme;
            set
            {
                if (value != null)
                {
                    this.SetProperty(ref _theme, value);
                    ChangeTheme();
                }
            }
        }

        /// <summary>
        /// Open file.
        /// </summary>
        public async void OpenFile()
        {
            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                // Get top level from the current control. Alternatively, you can use Window reference instead.
                var topLevel = desktopLifetime.MainWindow;

                // Start async operation to open the dialog.
                var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Select TestingTask.exe",
                    FileTypeFilter = new[] {
                    new FilePickerFileType("TestingTask.exe")
                    {
                        Patterns = new[] { "*.exe" },
                        MimeTypes = new[] { "application/vnd.microsoft.portable-executable" }
                    }
                },
                    AllowMultiple = false
                });

                if (files.Count >= 1)
                {
                    var path = files[0].Path.LocalPath;

                    TestPath = path;
                }
            }
        }

        /// <summary>
        /// Change theme.
        /// </summary>
        public void ChangeTheme()
        {
            App.Current.RequestedThemeVariant = Theme == "Dark" ? ThemeVariant.Dark : ThemeVariant.Light;
        }

        /// <summary>
        ///
        /// </summary>
        public void Save()
        {
            Settings.Default.IsCheckLog = IsCheckLog;
            Settings.Default.TestPath = TestPath;
            Settings.Default.Theme = Theme;
            Settings.Default.Save();
        }
    }
}