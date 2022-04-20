// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.DigitalWorkplace.DigitalTwins.QueryBuilder.Dynamic
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.DigitalWorkplace.DigitalTwins.QueryBuilder.Common.Clauses;
    using Microsoft.DigitalWorkplace.DigitalTwins.QueryBuilder.Dynamic.Statements;

    /// <summary>
    /// Base query for all select queries. This query does not support Joins.
    /// </summary>
    public abstract class TwinSelectQueryBase<TQuery, TWhereStatement> : JoinQuery<TQuery, TWhereStatement>
        where TQuery : TwinSelectQueryBase<TQuery, TWhereStatement>
        where TWhereStatement : WhereBaseStatement<TWhereStatement>
    {
        internal TwinSelectQueryBase(string rootAlias, IList<string> definedAliases, SelectClause selectClause, FromClause fromClause, IList<JoinClause> joinClauses, WhereClause whereClause) : base(rootAlias, definedAliases, selectClause, fromClause, joinClauses, whereClause)
        {
        }

        /// <summary>
        /// Select Top(N) records.
        /// </summary>
        /// <param name="numberOfRecords">Postive number.</param>
        /// <returns>The ADT Query with TOP clause.</returns>
        public TQuery Top(ushort numberOfRecords)
        {
            selectClause.NumberOfRecords = numberOfRecords;
            return (TQuery)this;
        }
    }

    /// <summary>
    /// Base query for all select queries. This query does not support Joins.
    /// </summary>
    public abstract class RelationshipSelectQueryBase<TQuery, TWhereStatement> : FilterQuery<TQuery, TWhereStatement>
        where TQuery : RelationshipSelectQueryBase<TQuery, TWhereStatement>
        where TWhereStatement : WhereBaseStatement<TWhereStatement>
    {
        internal RelationshipSelectQueryBase(string rootAlias, IList<string> definedAliases, SelectClause selectClause, FromClause fromClause, IList<JoinClause> joinClauses, WhereClause whereClause) : base(rootAlias, definedAliases, selectClause, fromClause, joinClauses, whereClause)
        {
        }

        /// <summary>
        /// Select Top(N) records.
        /// </summary>
        /// <param name="numberOfRecords">Postive number.</param>
        /// <returns>The ADT Query with TOP clause.</returns>
        public TQuery Top(ushort numberOfRecords)
        {
            selectClause.NumberOfRecords = numberOfRecords;
            return (TQuery)this;
        }
    }

    /// <summary>
    /// The query that has the default select that was generated by the FROM clause.
    /// </summary>
    public class TwinDefaultSelectQuery<TWhereStatement> : TwinSelectQueryBase<TwinDefaultSelectQuery<TWhereStatement>, TWhereStatement>
    where TWhereStatement : WhereBaseStatement<TWhereStatement>
    {
        internal TwinDefaultSelectQuery(string rootAlias, IList<string> definedAliases, SelectClause selectClause, FromClause fromClause, IList<JoinClause> joinClauses, WhereClause whereClauses) : base(rootAlias, definedAliases, selectClause, fromClause, joinClauses, whereClauses)
        {
        }

        /// <summary>
        /// Overrides the default SELECT statement with a custom select alias or aliases.
        /// </summary>
        /// <param name="aliases">Optional: One or more aliases to apply to the SELECT clause.</param>
        /// <returns>A query instance with one SELECT clause.</returns>
        public TwinQuery<TWhereStatement> Select(params string[] aliases)
        {
            ClearSelects();
            foreach (var name in aliases)
            {
                ValidateAndAddSelect(name);
            }

            return new TwinQuery<TWhereStatement>(RootAlias, definedAliases, selectClause, fromClause, joinClauses, whereClause);
        }
    }

    /// <summary>
    /// The query that has the default select that was generated by the FROM RELATIONSHIPS clause. This query does not support Joins.
    /// </summary>
    public class RelationshipDefaultSelectQuery<TWhereStatement> : RelationshipSelectQueryBase<RelationshipDefaultSelectQuery<TWhereStatement>, TWhereStatement>
    where TWhereStatement : WhereBaseStatement<TWhereStatement>
    {
        internal RelationshipDefaultSelectQuery(string rootAlias, IList<string> definedAliases, SelectClause selectClause, FromClause fromClause, IList<JoinClause> joinClauses, WhereClause whereClauses) : base(rootAlias, definedAliases, selectClause, fromClause, joinClauses, whereClauses)
        {
        }

        /// <summary>
        /// Overrides the default SELECT statement with a custom select alias or aliases.
        /// Because relationships cannot join on anything, the Select method narrows to specific relationship properties.
        /// </summary>
        /// <param name="propertyNames">Optional: One or more relationship properties to apply to the SELECT clause.</param>
        /// <returns>A query instance with one SELECT clause.</returns>
        public RelationshipQuery<TWhereStatement> Select(params string[] propertyNames)
        {
            ClearSelects();
            foreach (var name in propertyNames)
            {
                /*
                 Even though ValidateAndAddSelect checks for null, since we may alter the value passed into it
                 we need to check ahead of the alteration, otherwise we could end up with "rootName." or "rootName.    "
                */
                ValidateAliasNotNullOrWhiteSpace(name);
                var alias = name == RootAlias ? name : $"{RootAlias}.{name}";
                ValidateAndAddSelect(alias);
            }

            return new RelationshipQuery<TWhereStatement>(RootAlias, definedAliases, selectClause, fromClause, joinClauses, whereClause);
        }
    }

    /// <summary>
    /// The query that has one select clause.
    /// </summary>
    public class TwinQuery<TWhereStatement> : TwinSelectQueryBase<TwinQuery<TWhereStatement>, TWhereStatement>
        where TWhereStatement : WhereBaseStatement<TWhereStatement>
    {
        internal TwinQuery(string rootAlias, IList<string> definedAliases, SelectClause selectClause, FromClause fromClause, IList<JoinClause> joinClauses, WhereClause whereClauses) : base(rootAlias, definedAliases, selectClause, fromClause, joinClauses, whereClauses)
        {
        }
    }

    /// <summary>
    /// The query that has one select clause. This query does not support Joins.
    /// </summary>
    public class RelationshipQuery<TWhereStatement> : RelationshipSelectQueryBase<RelationshipQuery<TWhereStatement>, TWhereStatement>
    where TWhereStatement : WhereBaseStatement<TWhereStatement>
    {
        internal RelationshipQuery(string rootAlias, IList<string> definedAliases, SelectClause selectClause, FromClause fromClause, IList<JoinClause> joinClauses, WhereClause whereClauses) : base(rootAlias, definedAliases, selectClause, fromClause, joinClauses, whereClauses)
        {
        }
    }
}