﻿#region

using System;
using System.Linq.Expressions;

#endregion

namespace Susanoo
{
    /// <summary>
    /// Helpful Expression extension methods
    /// </summary>
    internal static class ExpressionExtensions
    {
        /// <summary>
        /// Given an expression, extract the listed property name; similar to reflection but with familiar LINQ+lambdas.
        /// </summary>
        /// <typeparam name="TModel">the model type to extract property names</typeparam>
        /// <typeparam name="TValue">the value type of the expected property</typeparam>
        /// <param name="propertySelector">expression that just selects a model property to be turned into a string</param>
        /// <returns>indicated property name</returns>
        internal static string GetPropertyName<TModel, TValue>(this Expression<Func<TModel, TValue>> propertySelector)
        {
            var body = propertySelector.Body as UnaryExpression;
            var name = ((MemberExpression) body?.Operand)?.Member.Name 
                ?? ((MemberExpression) propertySelector.Body).Member.Name;

            return name;
        }
    }
}