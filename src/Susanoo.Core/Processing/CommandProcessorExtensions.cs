﻿using Susanoo.Processing;
using System;

namespace Susanoo
{
    /// <summary>
    /// Class CommandProcessorExtensions.
    /// </summary>
    public static class CommandProcessorExtensions
    {
        /// <summary>
        /// Sets the timeout.
        /// </summary>
        /// <typeparam name="TCommandProcessor">The type of the t command processor.</typeparam>
        /// <param name="processor">The processor.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>TCommandProcessor.</returns>
        public static TCommandProcessor SetTimeout<TCommandProcessor>(this TCommandProcessor processor, TimeSpan timeout)
            where TCommandProcessor : ICommandProcessor
        {
            processor.Timeout = timeout;
            return processor;
        }
    }
}
