
using System;

namespace IntentProcessing.Contract
{
    /// <summary>
    /// Represents a single batch of text input to the service for analysis
    /// </summary>
    public class AnalyzeTextRequest
    {
        /// <summary>
        /// Two letter ISO language code, e.g. "en" for "English"
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// List of IDs of the analyers to be used on the given input text; see Analyzer for more information.
        /// </summary>
        public Guid[] AnalyzerIds { get; set; }

        /// <summary>
        /// The raw input text to be analyzed.
        /// </summary>
        public string Text { get; set; }
    }
}