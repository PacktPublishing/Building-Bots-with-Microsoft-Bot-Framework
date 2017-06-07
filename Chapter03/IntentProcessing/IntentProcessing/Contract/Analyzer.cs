
using System;

namespace IntentProcessing.Contract
{
    /// <summary>
    /// Represents a single analysis operation that can be performed on text, like finding sentence boundaries or part-of-speech tags.
    /// </summary>
    public class Analyzer
    {
        /// <summary>
        /// Unique identifier for this analyzer used to communicate with the service
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// List of two letter ISO language codes for which this analyzer is available. e.g. "en" represents "English"
        /// </summary>
        public string[] Languages { get; set; }

        /// <summary>
        /// Description of the type of analysis used here, such as Constituency_Tree or POS_tags.
        /// </summary>
        public string Kind { get; set; }

        /// <summary>
        /// The specification for how a human should produce ideal output for this task. Most use the specification from the Penn Teeebank.
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// Description of the implementaiton used in this analyzer.
        /// </summary>
        public string Implementation { get; set; }
    }
}