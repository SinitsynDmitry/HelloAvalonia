/******************************************************************************
 *
 * File: NotifyTaskCompletion.cs
 *
 * Description: Asynchronous MVVM Applications: Data Binding.
 *
 * From: https://learn.microsoft.com/en-us/archive/msdn-magazine/2014/march/async-programming-patterns-for-asynchronous-mvvm-applications-data-binding
 *
 * Date: March 2014	 Authors:  Stephen Cleary
 *
 *****************************************************************************/

using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace FirstLook
{
    /// <summary>
    /// The notify task completion.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public sealed class NotifyTaskCompletion<TResult> : INotifyPropertyChanged
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyTaskCompletion"/> class.
        /// </summary>
        /// <param name="task">The task.</param>
        public NotifyTaskCompletion(Task<TResult> task)
        {
            Task = task;
            if (task.IsCompleted)
                TaskCompletion = System.Threading.Tasks.Task.FromResult(0);
            else
                TaskCompletion = WatchTaskAsync();
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return (InnerException == null) ?
                    null : InnerException.Message;
            }
        }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        public AggregateException Exception { get { return Task.Exception; } }

        /// <summary>
        /// Gets the inner exception.
        /// </summary>
        public Exception InnerException
        {
            get
            {
                return (Exception == null) ?
                    null : Exception.InnerException;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is canceled.
        /// </summary>
        public bool IsCanceled { get { return Task.IsCanceled; } }

        /// <summary>
        /// Gets a value indicating whether is completed.
        /// </summary>
        public bool IsCompleted { get { return Task.IsCompleted; } }

        /// <summary>
        /// Gets a value indicating whether is faulted.
        /// </summary>
        public bool IsFaulted { get { return Task.IsFaulted; } }

        /// <summary>
        /// Gets a value indicating whether not is completed.
        /// </summary>
        public bool IsNotCompleted { get { return !Task.IsCompleted; } }

        /// <summary>
        /// Gets a value indicating whether successfully is completed.
        /// </summary>
        public bool IsSuccessfullyCompleted
        {
            get
            {
                return Task.Status ==
                    TaskStatus.RanToCompletion;
            }
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        public TResult Result
        {
            get
            {
                return (Task.Status == TaskStatus.RanToCompletion) ?
                    Task.Result : default(TResult);
            }
        }

        /// <summary>
        /// Gets the status.
        /// </summary>
        public TaskStatus Status { get { return Task.Status; } }

        /// <summary>
        /// Gets the task.
        /// </summary>
        public Task<TResult> Task { get; private set; }

        /// <summary>
        /// Gets the task completion.
        /// </summary>
        public Task TaskCompletion { get; private set; }

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// Watch task asynchronously.
        /// </summary>
        /// <returns>A Task.</returns>
        private async Task WatchTaskAsync()
        {
            try
            {
                await Task;
            }
            catch
            {
            }
            var propertyChanged = PropertyChanged;
            if (propertyChanged == null)
                return;
            propertyChanged(this, new PropertyChangedEventArgs("Status"));
            propertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));
            propertyChanged(this, new PropertyChangedEventArgs("IsNotCompleted"));
            if (Task.IsCanceled)
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsCanceled"));
            }
            else if (Task.IsFaulted)
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsFaulted"));
                propertyChanged(this, new PropertyChangedEventArgs("Exception"));
                propertyChanged(this, new PropertyChangedEventArgs("InnerException"));
                propertyChanged(this, new PropertyChangedEventArgs("ErrorMessage"));
            }
            else
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsSuccessfullyCompleted"));
                propertyChanged(this, new PropertyChangedEventArgs("Result"));
            }
        }

        #endregion Private Methods
    }
}