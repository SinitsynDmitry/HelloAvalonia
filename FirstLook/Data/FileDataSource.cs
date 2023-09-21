/******************************************************************************
 *
 * File: FileDataSource.cs
 *
 * Description: FileDataSource.cs class and he's methods.
 *
 * Copyright (C) 2023 by Dmitry Sinitsyn
 *
 * Date: 18.8.2023	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FirstLook.Data
{
    /// <summary>
    /// The file data source.
    /// </summary>
    internal class FileDataSource : IDataSource
    {
        #region Public Properties

        /// <summary>
        /// Gets or Sets the file path.
        /// </summary>
        public string FilePath { get; set; } = "DataSource.json";

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Add data asynchronously.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>A Task.</returns>
        public async Task AddDataAsync(TestValue data)
        {
            var values = await GetDataAsync();

            values.Add(data);

            await SaveDataAsync(values);
        }

        /// <summary>
        /// Gets the data asynchronously.
        /// </summary>
        /// <returns><![CDATA[Task<List<TestValue>>]]></returns>
        public async Task<List<TestValue>> GetDataAsync()
        {
            FileInfo dataFile = new FileInfo(FilePath);
            var values = new List<TestValue>();
            if (dataFile.Exists)
            {
                var text = await File.ReadAllTextAsync(dataFile.FullName);
                try
                {
                    var old = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TestValue>>(text);
                    if (old != null)
                        values.AddRange(old);
                }
                catch { }
            }

            return values;
        }

        /// <summary>
        /// Save data asynchronously.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>A Task.</returns>
        public async Task SaveDataAsync(List<TestValue> data)
        {
            if (data != null)
            {
                data = data.OrderByDescending(t => t.RunTime).ToList();
                var newData = Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
                await File.WriteAllTextAsync(FilePath, newData);
            }
        }

        #endregion Public Methods
    }
}