/******************************************************************************
 *
 * File: MainWindow.axaml.cs
 *
 * Description: MainWindow.axaml.cs class and he's methods.
 *
 * Copyright (C) 2023 by Dmitry Sinitsyn
 *
 * Date: 18.8.2023	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

using Avalonia.Controls;
using Avalonia.Styling;
using AvaloniaProgressRing;

namespace FirstLook.Views;
/// <summary>
/// The main window.
/// </summary>

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        App.Current.RequestedThemeVariant = Settings.Default.Theme == "Dark" ? ThemeVariant.Dark : ThemeVariant.Light;

    }
}
