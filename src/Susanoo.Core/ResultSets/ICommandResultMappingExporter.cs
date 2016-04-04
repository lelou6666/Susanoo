using Susanoo.Mapping.Properties;
using System;
using System.Collections.Generic;

namespace Susanoo.ResultSets
{
    /// <summary>
    /// Exposes property mapping export capabilities.
    /// </summary>
    public interface ICommandResultMappingExporter
    {
        /// <summary>
        /// Exports a results mappings for processing.
        /// </summary>
        /// <param name="resultType">Type of the result.</param>
        /// <returns>IDictionary&lt;System.String, IPropertyMapping&gt;.</returns>
        IDictionary<string, IPropertyMapping> Export(Type resultType);
    }
}