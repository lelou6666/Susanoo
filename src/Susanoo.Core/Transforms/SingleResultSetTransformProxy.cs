﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Numerics;
#if !NETFX40
using System.Threading;
using System.Threading.Tasks;
#endif
using Susanoo.Command;
using Susanoo.Processing;
using Susanoo.ResultSets;

namespace Susanoo.Transforms
{
    /// <summary>
    /// A proxy for single result command processors that allows transforms to be applied prior to execution.
    /// </summary>
    /// <typeparam name="TFilter">The type of the filter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class SingleResultSetTransformProxy<TFilter, TResult>
            : ICommandProcessor<TFilter, TResult>
    {
        private readonly ICommandProcessor<TFilter, TResult> _source;
        private readonly IEnumerable<CommandTransform> _transforms;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleResultSetTransformProxy{TFilter,TResult}"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="transforms">The transforms.</param>
        public SingleResultSetTransformProxy(ICommandProcessor<TFilter, TResult> source, IEnumerable<CommandTransform> transforms)
        {
            _source = source;
            _transforms = transforms;
        }

        /// <summary>
        /// Gets the hash code used for caching result mapping compilations.
        /// </summary>
        /// <value>The cache hash.</value>
        public BigInteger CacheHash =>
            _source.CacheHash;

        /// <summary>
        /// Clears any column index information that may have been cached.
        /// </summary>
        public void ClearColumnIndexInfo()
        {
            _source.ClearColumnIndexInfo();
        }

        /// <summary>
        /// Flushes the result cache.
        /// </summary>
        public void FlushCache()
        {
            _source.FlushCache();
        }

        /// <summary>
        /// Gets the CommandBuilder result information.
        /// </summary>
        /// <value>The CommandBuilder result information.</value>
        public ICommandResultInfo<TFilter> CommandResultInfo =>
            _source.CommandResultInfo;

        /// <summary>
        /// Gets the CommandBuilder information.
        /// </summary>
        /// <value>The CommandBuilder information.</value>
        public ICommandBuilderInfo<TFilter> CommandBuilderInfo =>
            _source.CommandBuilderInfo;

        /// <summary>
        /// Gets or sets the timeout of a command execution.
        /// </summary>
        /// <value>The timeout.</value>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        public IEnumerable<TResult> Execute(IDatabaseManager databaseManager, IExecutableCommandInfo executableCommandInfo)
        {
            var transformed = _transforms.Aggregate(executableCommandInfo, (current, commandTransform) =>
                commandTransform.Transform(current));

            return _source.Execute(databaseManager, transformed);
        }

        /// <summary>
        /// Allows a hook in an instance of a processor
        /// </summary>
        /// <param name="interceptOrProxy">The intercept or proxy.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult&gt;.</returns>
        public ICommandProcessor<TFilter, TResult> InterceptOrProxyWith(Func<ICommandProcessor<TFilter, TResult>, ICommandProcessor<TFilter, TResult>> interceptOrProxy)
        {
            return interceptOrProxy(this);
        }

        /// <summary>
        /// Updates the column index information.
        /// </summary>
        /// <param name="info">The column checker.</param>
        public void UpdateColumnIndexInfo(ColumnChecker info)
        {
            _source.UpdateColumnIndexInfo(info);
        }

        /// <summary>
        /// Retrieves a copy of the column index info.
        /// </summary>
        public ColumnChecker RetrieveColumnIndexInfo()
        {
            return _source.RetrieveColumnIndexInfo();
        }

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        public IEnumerable<TResult> Execute(IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
        {
            return Execute(databaseManager, default(TFilter), null, explicitParameters);
        }

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        public IEnumerable<TResult> Execute(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
        {
            return Execute(databaseManager, filter, null, explicitParameters);
        }

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>IEnumerable&lt;TResult&gt;.</returns>
        public IEnumerable<TResult> Execute(IDatabaseManager databaseManager, TFilter filter, object parameterObject,
            params DbParameter[] explicitParameters)
        {
            IExecutableCommandInfo executableCommandInfo = new ExecutableCommandInfo
            {
                CommandText = CommandBuilderInfo.CommandText,
                DbCommandType = CommandBuilderInfo.DbCommandType,
                Parameters = CommandBuilderInfo.BuildParameters(databaseManager, filter, parameterObject, explicitParameters)
            };

            return Execute(databaseManager, executableCommandInfo);
        }

        /// <summary>
        /// Enables result caching.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>ICommandProcessor&lt;TFilter, TResult&gt;.</returns>
        /// <exception cref="System.ArgumentException">@Calling EnableResultCaching with CacheMode None effectively would disable caching,
        /// this is confusing and therefor is not allowed.;mode</exception>
        public ICommandProcessor<TFilter, TResult> EnableResultCaching(CacheMode mode = CacheMode.Permanent, double? interval = null)
        {
            _source.EnableResultCaching(mode, interval);
            return this;
        }

#if !NETFX40
        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="executableCommandInfo">The executable command information.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        public async Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager, IExecutableCommandInfo executableCommandInfo,
            CancellationToken cancellationToken)
        {
            var transformed = _transforms.Aggregate(executableCommandInfo, (current, commandTransform) =>
                commandTransform.Transform(current));

            return await _source.ExecuteAsync(databaseManager, transformed, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        public async Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager, CancellationToken cancellationToken,
            params DbParameter[] explicitParameters)
        {
            return await ExecuteAsync(databaseManager, default(TFilter), null, cancellationToken, explicitParameters)
                    .ConfigureAwait(false);
        }

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        public async Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager, params DbParameter[] explicitParameters)
        {
            return await ExecuteAsync(databaseManager, default(TFilter), null, CancellationToken.None, explicitParameters)
                    .ConfigureAwait(false);
        }

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        public async Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager, TFilter filter, object parameterObject,
            CancellationToken cancellationToken, params DbParameter[] explicitParameters)
        {
            IExecutableCommandInfo executableCommandInfo = new ExecutableCommandInfo
            {
                CommandText = CommandBuilderInfo.CommandText,
                DbCommandType = CommandBuilderInfo.DbCommandType,
                Parameters = CommandBuilderInfo.BuildParameters(databaseManager, filter, parameterObject, explicitParameters)
            };

            return await ExecuteAsync(databaseManager, executableCommandInfo, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="parameterObject">The parameter object.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        public async Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager, TFilter filter, object parameterObject,
            params DbParameter[] explicitParameters)
        {
            return await ExecuteAsync(databaseManager, filter, parameterObject, CancellationToken.None, explicitParameters)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        public async Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager, TFilter filter, params DbParameter[] explicitParameters)
        {
            return await ExecuteAsync(databaseManager, filter, null, CancellationToken.None, explicitParameters)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Assembles a data CommandBuilder for an ADO.NET provider,
        /// executes the CommandBuilder and uses pre-compiled mappings to assign the resultant data to the result object type.
        /// </summary>
        /// <param name="databaseManager">The database manager.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="explicitParameters">The explicit parameters.</param>
        /// <returns>Task&lt;IEnumerable&lt;TResult&gt;&gt;.</returns>
        public async Task<IEnumerable<TResult>> ExecuteAsync(IDatabaseManager databaseManager, TFilter filter, CancellationToken cancellationToken,
            params DbParameter[] explicitParameters)
        {
            return await ExecuteAsync(databaseManager, filter, null, cancellationToken, explicitParameters)
                .ConfigureAwait(false);
        }
#endif
    }
}