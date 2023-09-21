/******************************************************************************
 *
 * File: TestValue.cs
 *
 * Description: TestValue.cs class and he's methods.
 *
 * Copyright (C) 2023 by Dmitry Sinitsyn
 *
 * Date: 18.8.2023	 Authors:  Dmitry Sinitsyn
 *
 *****************************************************************************/

using System;

namespace FirstLook.Data
{
    /// <summary>
    /// The test value.
    /// </summary>
    public record TestValue
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the run time.
        /// </summary>
        public DateTime RunTime { get; set; }

        /// <summary>
        /// Gets or Sets the hits static.
        /// </summary>
        public int HitsStatic { get; set; }

        /// <summary>
        /// Gets or Sets the hits moving.
        /// </summary>
        public int HitsMoving { get; set; }

        /// <summary>
        /// Gets or Sets the miss.
        /// </summary>
        public int Miss { get; set; }

        /// <summary>
        /// Gets or Sets the hit ratio.
        /// </summary>
        public string HitRatio { get; set; }

        /// <summary>
        /// Gets or Sets the gc count.
        /// </summary>
        public int GcCount { get; set; }

        /// <summary>
        /// Gets or Sets the gc collected.
        /// </summary>
        public string GcCollected { get; set; }

        /// <summary>
        /// Gets or Sets the processing time.
        /// </summary>
        public string ProcessingTime { get; set; }

        /// <summary>
        /// Gets or Sets the score.
        /// </summary>
        public int Score { get; set; } = -1;

        /// <summary>
        /// Gets or Sets the precise collision accuracy.
        /// </summary>
        public string PreciseCollisionAccuracy { get; set; }

        /// <summary>
        /// Gets or Sets the precise collision time.
        /// </summary>
        public string PreciseCollisionTime { get; set; }

        /// <summary>
        /// Gets or Sets the query accuracy.
        /// </summary>
        public string QueryAccuracy { get; set; }

        /// <summary>
        /// Gets or Sets the comment.
        /// </summary>
        public string Comment { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Checks if is valid.
        /// </summary>
        /// <returns>A bool.</returns>
        public bool IsValid()
        {
            //Score, Hit ratio,Processing time, GC Collected).
            return Score > 0 && !string.IsNullOrWhiteSpace(HitRatio) && !string.IsNullOrWhiteSpace(ProcessingTime) && !string.IsNullOrWhiteSpace(GcCollected);
        }

        /// <summary>
        /// Checks if is simular.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>A bool.</returns>
        public bool IsSimular(TestValue target)
        {
            return Score == target.Score && HitRatio == target.HitRatio && ProcessingTime == target.ProcessingTime && GcCollected == target.GcCollected && Math.Abs((RunTime - target.RunTime).TotalSeconds) < 30;
        }

        /// <summary>
        /// Extracts the data.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A TestValue.</returns>
        public static TestValue ExtractData(string input)
        {
            DateTime runTime = DateTime.Now;
            string[] lines = input.Split('\n');
            var runTimeStr = "";
            if (lines?.Length > 0)
            {
                foreach (var line in lines)
                {
                    if (line.Contains("Testing task results:"))
                    {
                        var end = line.IndexOf("- Thread:");
                        if (end > 0)
                        {
                            runTimeStr = line.Substring(0, end).Trim();
                        }
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(runTimeStr))
                {
                    runTime = DateTime.Parse(runTimeStr);
                }

                var data = new TestValue()
                {
                    RunTime = runTime,
                    HitsStatic = TestValue.ExtractIntValue(lines, "Hits static:"),
                    HitsMoving = TestValue.ExtractIntValue(lines, "Hits moving:"),
                    Miss = TestValue.ExtractIntValue(lines, "Miss:"),
                    HitRatio = TestValue.ExtractValue(lines, "Hit ratio:"),
                    GcCount = TestValue.ExtractIntValue(lines, "GC Count:"),
                    GcCollected = TestValue.ExtractValue(lines, "GC Collected:"),
                    ProcessingTime = TestValue.ExtractValue(lines, "Processing time:"),
                    Score = TestValue.ExtractIntValue(lines, "Score:"),
                    PreciseCollisionAccuracy = TestValue.ExtractValue(lines, "Precise collision accuracy:"),
                    PreciseCollisionTime = TestValue.ExtractValue(lines, "Precise collision time:"),
                    QueryAccuracy = TestValue.ExtractValue(lines, "Query accuracy:")
                };

                return data;
            }
            return new TestValue();
        }

        /// <summary>
        /// Extracts the value.
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <param name="key">The key.</param>
        /// <returns>A string.</returns>
        public static string ExtractValue(string[] lines, string key)
        {
            foreach (var line in lines)
            {
                if (line.StartsWith(key))
                {
                    return line.Substring(key.Length).Trim();
                }
            }
            return "";
        }

        /// <summary>
        /// Extracts the integer value.
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <param name="key">The key.</param>
        /// <returns>An int.</returns>
        public static int ExtractIntValue(string[] lines, string key)
        {
            foreach (var line in lines)
            {
                if (line.StartsWith(key))
                {
                    return Convert.ToInt32(line.Substring(key.Length).Trim());
                }
            }
            return -1;
        }

        #endregion Public Methods
    }
}