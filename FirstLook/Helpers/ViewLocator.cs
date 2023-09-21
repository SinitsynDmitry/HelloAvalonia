/******************************************************************************
 *
 * File: ViewLocator.cs
 *
 * Description: ViewLocator.cs class and he's methods.
 *
 *
 * Date: 18.8.2023
 *
 *****************************************************************************/

using Avalonia.Controls;
using Avalonia.Controls.Templates;
using FirstLook.ViewModels;
using System;

namespace FirstLook
{
    /// <summary>
    /// The view locator.
    /// </summary>
    public class ViewLocator : IDataTemplate
    {
        /// <summary>
        /// Builds the <see cref="Control"/>.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>A Control.</returns>
        public Control Build(object data)
        {
            var name = data.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }
            else
            {
                return new TextBlock { Text = "Not Found: " + name };
            }
        }


        /// <summary>
        /// Matches the <see cref="ViewModelBase"/>.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>A bool.</returns>
        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
    }
}
