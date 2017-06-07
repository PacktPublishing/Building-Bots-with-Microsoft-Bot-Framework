
using System;

namespace IntentProcessing.Contract
{
    /// <summary>
    /// This is the result of analysis from one analyzer.
    /// </summary>
    public class AnalyzeTextResult
    {
        /// <summary>
        /// The unique ID of the analyzer; see Analyzer for more information.
        /// </summary>
        public Guid AnalyzerId { get; set; }

        /// <summary>
        /// The resulting analysis, encoded as JSON. See the documentation for the relevant analyzer kind for more information on formatting.
        /// </summary>
        public object Result { get; set; }
    }
}