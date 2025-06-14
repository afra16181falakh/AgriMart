using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Data.SqlClient; // Make sure you have this using statement
using System.Text.RegularExpressions; // For splitting by GO

namespace AgriMartAPI.Data // Or your actual namespace
{
    public static class DatabaseMigrator
    {
        public static void ApplyMigrations(string connectionString, string scriptsFolderPath)
        {
            // 1. Parse the target database name from the connectionString
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
            string targetDatabaseName = builder.InitialCatalog;
            string serverInstance = builder.DataSource;

            // 2. Create a connection string to the 'master' database on the same server
            SqlConnectionStringBuilder masterConnectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = serverInstance,
                UserID = builder.UserID,
                Password = builder.Password,
                InitialCatalog = "master",
                Encrypt = builder.Encrypt,
                TrustServerCertificate = builder.TrustServerCertificate
            };
            string masterConnectionString = masterConnectionStringBuilder.ConnectionString;

            try
            {
                Console.WriteLine($"Ensuring database '{targetDatabaseName}' exists on server '{serverInstance}'...");
                using (var masterConnection = new SqlConnection(masterConnectionString))
                {
                    masterConnection.Open();
                    using (var command = masterConnection.CreateCommand())
                    {
                        // Sanitize database name for use in dynamic SQL (basic protection)
                        string safeTargetDatabaseName = targetDatabaseName.Replace("'", "''").Replace("]", "]]");
                        command.CommandText = $"IF DB_ID(N'{safeTargetDatabaseName}') IS NULL CREATE DATABASE [{safeTargetDatabaseName}]";
                        command.ExecuteNonQuery();
                        Console.WriteLine($"Database '{targetDatabaseName}' ensured (created if it didn't exist).");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"FATAL ERROR: Could not ensure database '{targetDatabaseName}' exists: {ex.Message}");
                Console.ResetColor();
                throw; // Rethrow as this is critical
            }

            // NOW that the database is guaranteed to exist, proceed with migrations
            // using the original 'connectionString' which targets 'targetDatabaseName'

            Console.WriteLine("Starting ADO.NET database migration process...");
            var appliedScripts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                appliedScripts = GetAppliedScripts(connectionString);
            }
            catch (SqlException ex)
            {
                // A common case is SchemaVersions table not existing yet.
                // We'll check for specific error messages or conditions if possible,
                // otherwise, we assume it's okay to proceed as if no scripts were applied.
                // Error 208: Invalid object name 'dbo.SchemaVersions'.
                if (ex.Number == 208 && ex.Message.Contains("SchemaVersions", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("'SchemaVersions' table not found (Error 208). Assuming no scripts applied yet. The first SQL script should create it.");
                }
                else
                {
                    // For other SQL errors during GetAppliedScripts, it's more problematic
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"SQL Error fetching applied scripts: {ex.Message} (Number: {ex.Number})");
                    Console.ResetColor();
                    throw; // Rethrow more serious SQL errors
                }
            }
            catch (Exception ex) // Catch other potential non-SQL DB issues during GetAppliedScripts
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error fetching applied scripts: {ex.Message}");
                Console.ResetColor();
                throw; // Rethrow
            }

            if (!Directory.Exists(scriptsFolderPath))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Warning: Scripts folder not found at '{scriptsFolderPath}'. Skipping migration script application.");
                Console.ResetColor();
                return;
            }

            var scriptFiles = Directory.GetFiles(scriptsFolderPath, "*.sql")
                                       .OrderBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase) // Ensure scripts are ordered by name
                                       .ToList();

            if (!scriptFiles.Any())
            {
                Console.WriteLine("No .sql script files found in the scripts folder.");
                return;
            }

            Console.WriteLine($"Found {scriptFiles.Count} script files. Checking against {appliedScripts.Count} applied scripts.");

            foreach (var scriptFile in scriptFiles)
            {
                var scriptName = Path.GetFileName(scriptFile);
                if (appliedScripts.Contains(scriptName))
                {
                    // Console.WriteLine($"Skipping already applied script: {scriptName}");
                    continue;
                }

                Console.WriteLine($"Applying script: {scriptName}...");
                try
                {
                    string scriptContent = File.ReadAllText(scriptFile);
                    ExecuteSqlScript(connectionString, scriptContent, scriptName); // Pass scriptName for logging
                    RecordScriptAsApplied(connectionString, scriptName);
                    Console.WriteLine($"Successfully applied script: {scriptName}.");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error applying script {scriptName}: {ex.Message}");
                    Console.WriteLine(ex.ToString()); // Full stack trace for debugging
                    Console.ResetColor();
                    Console.WriteLine("Halting further migrations due to error.");
                    throw; // Critical error, stop the application
                }
            }
            Console.WriteLine("ADO.NET database migration process completed successfully.");
        }

        private static HashSet<string> GetAppliedScripts(string connectionString)
        {
            var scripts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // Check if SchemaVersions table exists before trying to query it
                bool schemaVersionsTableExists = false;
                using (var commandCheck = connection.CreateCommand())
                {
                    // Check for user table 'U'
                    commandCheck.CommandText = "SELECT CASE WHEN OBJECT_ID('dbo.SchemaVersions', 'U') IS NOT NULL THEN 1 ELSE 0 END";
                    try
                    {
                        var result = commandCheck.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                             schemaVersionsTableExists = Convert.ToInt32(result) == 1;
                        }
                    }
                    catch (SqlException ex)
                    {
                         // This might happen if the database itself is in a weird state, but the connection was made.
                         // For example, if the user doesn't have permission to query system objects.
                         // We'll let the main query attempt fail and be caught by ApplyMigrations.
                         Console.WriteLine($"Warning: Could not reliably check for SchemaVersions table existence. Proceeding with query attempt. Error: {ex.Message}");
                    }
                }

                if (schemaVersionsTableExists)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT ScriptName FROM dbo.SchemaVersions";
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                scripts.Add(reader.GetString(0));
                            }
                        }
                    }
                    Console.WriteLine($"Found {scripts.Count} applied scripts from SchemaVersions table.");
                }
                else
                {
                    // This specific message will be shown if the table doesn't exist.
                    // The catch in ApplyMigrations for SqlException with Error 208 will handle this flow.
                    Console.WriteLine("'SchemaVersions' table does not exist. Assuming 0 scripts applied.");
                }
            }
            return scripts;
        }

        private static void ExecuteSqlScript(string connectionString, string scriptContent, string scriptNameForLogging)
        {
            // Split script by "GO" statements (common batch separator)
            // Regex to split by "GO" on its own line, case-insensitive, considering potential surrounding whitespace
            var batches = Regex.Split(scriptContent, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction()) // Execute each script file in a transaction
                {
                    try
                    {
                        foreach (var batch in batches)
                        {
                            if (string.IsNullOrWhiteSpace(batch)) continue;

                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;
                                command.CommandText = batch;
                                command.CommandTimeout = 120; // Extend timeout for potentially long scripts
                                command.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception rbEx)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine($"CRITICAL: Exception during transaction rollback for script {scriptNameForLogging}: {rbEx.Message}");
                            Console.ResetColor();
                        }
                        throw; // Re-throw original exception
                    }
                }
            }
        }

        private static void RecordScriptAsApplied(string connectionString, string scriptName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    // Use parameters to prevent SQL injection, though scriptName is from file system here
                    command.CommandText = "IF NOT EXISTS (SELECT 1 FROM dbo.SchemaVersions WHERE ScriptName = @ScriptName) " +
                                          "INSERT INTO dbo.SchemaVersions (ScriptName, AppliedDate) VALUES (@ScriptName, GETDATE())";
                    command.Parameters.AddWithValue("@ScriptName", scriptName);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}