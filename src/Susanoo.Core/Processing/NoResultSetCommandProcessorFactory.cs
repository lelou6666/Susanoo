﻿using Susanoo.Command;

namespace Susanoo.Processing
{
    /// <summary>
    /// Builds a command processor that returns a scalar values or return codes.
    /// </summary>
    public class NoResultSetCommandProcessorFactory 
        : INoResultSetCommandProcessorFactory
    {
        /// <summary>
        /// Builds the command processor.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <param name="command">The command.</param>
        /// <returns>INoResultCommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public virtual INoResultCommandProcessor<TFilter> BuildCommandProcessor<TFilter>(ICommandBuilderInfo<TFilter> command) => 
            new NoResultCommandProcessor<TFilter>(command);
    }
}
