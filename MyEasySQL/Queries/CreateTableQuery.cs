﻿using MyEasySQL.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static MyEasySQL.Utils.RegexUtil;

namespace MyEasySQL.Queries;

/// <summary>
/// Provides functionality to create a table in the database with specified columns and constraints.
/// </summary>
public class CreateTableQuery
{
    private readonly MySQL _database;
    private readonly string _table;
    private readonly List<string> _columns = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTableQuery"/> class.
    /// </summary>
    /// <param name="database">The database instance to execute the query on.</param>
    /// <param name="table">The name of the table to create.</param>
    /// <exception cref="ArgumentNullException">Thrown when the database is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the table name is invalid.</exception>
    public CreateTableQuery(MySQL database, string table)
    {
        Validate(table, ValidateType.Table);

        _database = database ?? throw new ArgumentNullException(nameof(database));
        _table = table;
    }

    /// <summary>
    /// Adds a column definition to the table creation query.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <param name="type">The data type of the column.</param>
    /// <param name="typeValue">Optional type value (e.g., length for varchar).</param>
    /// <param name="notNull">Specifies whether the column is NOT NULL.</param>
    /// <param name="autoIncrement">Specifies whether the column is AUTO_INCREMENT.</param>
    /// <param name="unique">Specifies whether the column is UNIQUE.</param>
    /// <param name="primaryKey">Specifies whether the column is the PRIMARY KEY.</param>
    /// <param name="defaultValue">Specifies a default value for the column.</param>
    /// <returns>The <see cref="CreateTableQuery"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when the column name or type value is invalid.</exception>
    public CreateTableQuery AddColumn(
        string name,
        DataTypes type,
        string? typeValue = null,
        bool notNull = false,
        bool autoIncrement = false,
        bool unique = false,
        bool primaryKey = false,
        string? defaultValue = null)
    {
        Validate(name, ValidateType.Column);

        if (typeValue != null)
        {
            Validate(typeValue, ValidateType.TypeValue);
        }

        string columnDef = $"{name} {type}" +
                        (typeValue != null ? $"({typeValue})" : "") +
                        (notNull ? " NOT NULL" : "") +
                        (primaryKey ? " PRIMARY KEY" : "") +
                        (autoIncrement ? " AUTO_INCREMENT" : "") +
                        (unique ? " UNIQUE" : "") +
                        (defaultValue != null ? $" DEFAULT '{defaultValue.Replace("'", "''")}'" : "");
        _columns.Add(columnDef);
        return this;
    }

    /// <summary>
    /// Executes the table creation query asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no columns are defined for the table.</exception>
    public async Task ExecuteAsync()
    {
        if (_columns.Count == 0)
        {
            throw new InvalidOperationException("No columns defined for table creation.");
        }

        string query = $"CREATE TABLE IF NOT EXISTS {_table} ({string.Join(", ", _columns)});";
        await _database.ExecuteNonQueryAsync(query);
    }
}