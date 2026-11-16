#region "Header"

// File: ModelBuilderExtensions.cs
// Created 2026/10/23
// Developer: Pascal Klose
// SPDX-FileCopyrightText: 2026 The Istarnova Contributors
// SPDX-License-Identifier: GPL-3.0-or-later

#endregion

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Istarnova.EfCore.Extensions;

/// <summary>
///     Shortcuts for mapping database functions and the keyless rows they return
/// </summary>
public static class ModelBuilderExtensions
{
    #region "Methods"

    extension(ModelBuilder builder)
    {
        #region "Methods"

        /// <summary>
        ///     <c>HasDbFunction</c> for a method of the context, which is always the same construct.
        /// </summary>
        /// <typeparam name="TContext">The context that declares the method.</typeparam>
        /// <param name="functionName">The name of the method.</param>
        /// <param name="parameters">The method's parameter types.</param>
        /// <param name="schema">The schema in the database.</param>
        public DbFunctionBuilder HasCustomDbFunction<TContext>(string functionName, Type[] parameters, string schema)
            where TContext : DbContext
            => builder.HasDbFunction(typeof(TContext).GetMethod(functionName, parameters)
                                     ?? throw new InvalidOperationException($"{typeof(TContext).Name} has no method {functionName} with these parameters."))
                .HasSchema(schema);

        /// <summary>
        ///     <c>HasDbFunction</c> for a method of the context without overloads.
        /// </summary>
        /// <typeparam name="TContext">The context that declares the method.</typeparam>
        /// <param name="functionName">The name of the method.</param>
        /// <param name="schema">The schema in the database.</param>
        public DbFunctionBuilder HasCustomDbFunction<TContext>(string functionName, string schema)
            where TContext : DbContext
            => builder.HasDbFunction(typeof(TContext).GetMethod(functionName)
                                     ?? throw new InvalidOperationException($"{typeof(TContext).Name} has no method {functionName}."))
                .HasSchema(schema);

        /// <summary>
        ///     Maps a function whose rows are an entity that has no table of its own.
        /// </summary>
        /// <typeparam name="TContext">The context that declares the method.</typeparam>
        /// <typeparam name="TEntity">The keyless entity the function returns.</typeparam>
        public DbFunctionBuilder HasCustomDbFunctionAndToTableNullify<TContext, TEntity>(string functionName, Type[] parameters, string schema)
            where TContext : DbContext where TEntity : class
            => builder.Entity<TEntity>(entity => entity.ToTable((string?)null))
                .HasCustomDbFunction<TContext>(functionName, parameters, schema);

        /// <summary>
        ///     Takes an entity out of the tables, for the rows of a function.
        /// </summary>
        public ModelBuilder ToNullableTable<TEntity>() where TEntity : class
            => builder.Entity<TEntity>(entity => entity.ToTable((string?)null));

        #endregion
    }

    extension<TEntity>(EntityTypeBuilder<TEntity> builder) where TEntity : class
    {
        #region "Methods"

        /// <summary>
        ///     Takes the entity out of the tables, for the rows of a function.
        /// </summary>
        public EntityTypeBuilder<TEntity> ToNullableTable()
            => builder.ToTable((string?)null);

        #endregion
    }

    #endregion
}
