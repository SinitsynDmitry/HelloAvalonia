/******************************************************************************
 *
 * File: App.axaml.cs
 *
 * Description: App.axaml.cs class and he's methods.
 *
 * Copyright (C) 2023 by Dmitry Sinitsyn
 *
 * Date: 18.8.2023	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;

using FirstLook.ViewModels;
using FirstLook.Views;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System.Threading.Tasks;
using System;
using System.IO;
using FirstLook.Data;

namespace FirstLook;
/// <summary>
/// The app.
/// </summary>

public partial class App : Application
{
    /// <summary>
    /// Initializes the.
    /// </summary>
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// Ons the framework initialization completed.
    /// </summary>
    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        var dataSourceService = new FileDataSource();
        var mainViewModel = new MainViewModel(dataSourceService);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = mainViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>
    /// Shows the error.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>A Task.</returns>
    public async Task<ButtonResult> ShowError(string message)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            var box = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
            {
                ContentTitle = desktopLifetime.MainWindow.Title,
                ContentMessage = message,
                ButtonDefinitions = ButtonEnum.Ok,
                Icon = Icon.Error,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                WindowIcon = desktopLifetime.MainWindow.Icon
            });

            return await box.ShowWindowDialogAsync(desktopLifetime.MainWindow);
        }
        throw new NullReferenceException("This is not windows");
    }

    /// <summary>
    /// Shows the question.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>A Task.</returns>
    public async Task<ButtonResult> ShowQuestion(string message)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            var box = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
            {
                ContentTitle = desktopLifetime.MainWindow.Title,
                ContentMessage = message,
                ButtonDefinitions = ButtonEnum.YesNo,
                Icon = Icon.Question,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                WindowIcon = desktopLifetime.MainWindow.Icon
            });

            return await box.ShowWindowDialogAsync(desktopLifetime.MainWindow);
        }
        throw new NullReferenceException("This is not windows");
    }

}
