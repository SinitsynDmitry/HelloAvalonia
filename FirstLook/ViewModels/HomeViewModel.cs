/******************************************************************************
 *
 * File: HomeViewModel.cs
 *
 * Description: HomeViewModel.cs class and he's methods.
 *
 * Copyright (C) 2023 by Dmitry Sinitsyn
 *
 * Date: 18.8.2023	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

namespace FirstLook.ViewModels
{
    /// <summary>
    /// The home view model.
    /// </summary>
    internal class HomeViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeViewModel"/> class.
        /// </summary>
        public HomeViewModel()
        {
            Header = "Home";
        }
    }
}