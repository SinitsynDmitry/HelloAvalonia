/******************************************************************************
 *
 * File: IDataSource.cs
 *
 * Description: IDataSource.cs class and he's methods.
 *
 * Copyright (C) 2023 by Dmitry Sinitsyn
 *
 * Date: 18.8.2023	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

using System.Collections.Generic;
using System.Threading.Tasks;

namespace FirstLook.Data
{
    /// <summary>
    /// The data source interface.
    /// </summary>
    public interface IDataSource
    {
        /// <summary>
        /// Add data asynchronously.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>A Task.</returns>
        Task AddDataAsync(TestValue data);

        /// <summary>
        /// Gets the data asynchronously.
        /// </summary>
        /// <returns><![CDATA[Task<List<TestValue>>]]></returns>
        Task<List<TestValue>> GetDataAsync();

        /// <summary>
        /// Save data asynchronously.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>A Task.</returns>
        Task SaveDataAsync(List<TestValue> data);
    }
}