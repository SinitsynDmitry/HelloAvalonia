/******************************************************************************
 *
 * File: ViewModelBase.cs
 *
 * Description: ViewModelBase.cs class and he's methods.
 *
 * Copyright (C) 2023 by Dmitry Sinitsyn
 *
 * Date: 18.8.2023	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

using CommunityToolkit.Mvvm.ComponentModel;

namespace FirstLook.ViewModels;

/// <summary>
/// The view model base.
/// </summary>

public class ViewModelBase : ObservableObject
{
    private string header;

    /// <summary>
    /// Header
    /// </summary>
    public string Header
    {
        get => header;
        set => this.SetProperty(ref header, value);
    }
}