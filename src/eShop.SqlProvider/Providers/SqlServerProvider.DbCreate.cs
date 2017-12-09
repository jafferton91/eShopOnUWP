using System;
using System.IO;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace eShop.SqlProvider
{
    public partial class SqlServerProvider
    {
        const string CurrentVersion = "1.0";

        const string QUERY_EXISTSDB = "SELECT count(*) FROM sys.Databases WHERE name = @DbName";
        const string QUERY_VERSION = "SELECT [Current] FROM [Version]";

        public bool DatabaseExists()
        {
            var cnnStringBuilder = new SqlConnectionStringBuilder(ConnectionString);
            var dbName = cnnStringBuilder.InitialCatalog;
            cnnStringBuilder.InitialCatalog = "master";
            var masterConnectionString = cnnStringBuilder.ConnectionString;

            using (var cnn = new SqlConnection(masterConnectionString))
            {
                cnn.Open();
                using (var cmd = new SqlCommand(QUERY_EXISTSDB, cnn))
                {
                    var param = new SqlParameter("DbName", dbName);
                    cmd.Parameters.Add(param);
                    return (int)cmd.ExecuteScalar() == 1;
                }
            }
        }

        public bool IsLastVersion()
        {
            return GetVersion() == CurrentVersion;
        }

        public string GetVersion()
        {
            try
            {
                using (var cnn = new SqlConnection(ConnectionString))
                {
                    cnn.Open();
                    using (var cmd = new SqlCommand(QUERY_VERSION, cnn))
                    {
                        return cmd.ExecuteScalar() as String;
                    }
                }
            }
            catch
            {
                return String.Empty;
            }
        }

        public void CreateDatabase()
        {
            var cnnStringBuilder = new SqlConnectionStringBuilder(ConnectionString);
            var dbName = cnnStringBuilder.InitialCatalog;
            if (dbName == null)
            {
                throw new ArgumentNullException("Initial Catalog");
            }
            if (dbName.Equals("master", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Invalid Initial Catalog 'master'.");
            }
            cnnStringBuilder.InitialCatalog = "master";
            var masterConnectionString = cnnStringBuilder.ConnectionString;

            using (var cnn = new SqlConnection(masterConnectionString))
            {
                cnn.Open();
                foreach (var sqlLine in GetSqlScriptLines(dbName))
                {
                    using (var cmd = new SqlCommand(sqlLine, cnn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                using (var cmd = new SqlCommand($"INSERT INTO [Version] ([Current]) VALUES ('{CurrentVersion}')", cnn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private IEnumerable<string> GetSqlScriptLines(string dbName)
        {
            var sqlScript = GetSqlScript();
            sqlScript = sqlScript.Replace("[DATABASE_NAME]", dbName);
            using (var reader = new StringReader(sqlScript))
            {
                var sql = "";
                var line = reader.ReadLine();
                while (line != null)
                {
                    if (line.Trim() == "GO")
                    {
                        yield return sql;
                        sql = "";
                    }
                    else
                    {
                        sql += line;
                    }
                    line = reader.ReadLine();
                }
            }
        }

        private string GetSqlScript()
        {
            var stream = System.Reflection.Assembly.GetCallingAssembly().GetManifestResourceStream("eShop.SqlProvider.SqlScripts.CreateDb.sql");
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
