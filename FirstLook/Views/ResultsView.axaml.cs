/******************************************************************************
 *
 * File: ResultsView.axaml.cs
 *
 * Description: ResultsView.axaml.cs class and he's methods.
 *
 * Copyright (C) 2023 by Dmitry Sinitsyn
 *
 * Date: 18.8.2023	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

using Avalonia.Controls;
using Avalonia.Interactivity;
using FirstLook.ViewModels;

namespace FirstLook.Views
{
    /// <summary>
    /// The results view.
    /// </summary>
    public partial class ResultsView : UserControl
    {
        public ResultsView()
        {
            InitializeComponent();

            //This hack was used because the binding from XAML didn't work
            var dg1 = this.FindControl<DataGrid>("dataGrid1");
            if (dg1 != null)
                dg1.RowEditEnded += Dg1_RowEditEnded;
        }

        /// <summary>
        /// Dg1_S the row edit ended.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Dg1_RowEditEnded(object? sender, DataGridRowEditEndedEventArgs e)
        {
            _ = ((ResultsViewModel)DataContext).SaveAsync();
        }
    }
}
