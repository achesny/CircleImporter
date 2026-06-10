using System;
using System.Collections.Generic;

namespace CircleImporter.Core.Interfaces
{
    /// <summary>
    /// Interface for parsing Circle MUD data files
    /// </summary>
    public interface IParser<T>
    {
        /// <summary>
        /// Parse raw data and return a collection of parsed objects
        /// </summary>
        IEnumerable<T> Parse(string rawData);

        /// <summary>
        /// Parse a single object from raw data
        /// </summary>
        T ParseSingle(string rawData);
    }
}
