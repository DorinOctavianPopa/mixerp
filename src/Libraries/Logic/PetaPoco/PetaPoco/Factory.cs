﻿using System.Collections.Generic;
using System.Web.UI.WebControls;
using MixERP.Net.Framework;
using MixERP.Net.i18n;
using MixERP.Net.i18n.Resources;
using Npgsql;
using System.Data.SqlClient;

namespace PetaPoco
{
    public sealed class Factory
    {
        public const string ProviderName = "System.Data.SqlClient"; //change provider to Microsoft SQL Server
        public static string MetaDatabase = ConfigurationHelper.GetDbServerParameter("MetaDatabase");

        public static IEnumerable<T> Get<T>(string catalog, string sql, params object[] args)
        {
            try
            {
                using (Database db = new Database(GetConnectionString(catalog), ProviderName))
                {
                    return db.Query<T>(sql, args);
                }
            }
            catch (NpgsqlException ex)
            {
                if (ex.Code.StartsWith("P"))
                {
                    string errorMessage = GetDBErrorResource(ex);
                    throw new MixERPException(errorMessage, ex);
                }

                throw new MixERPException(ex.Message, ex);
            }
        }

		public static IEnumerable<T> Get<T>(string catalog, string sql,bool isStoredProcedure, params SqlParameter[] args)
		{
			try
			{
				using (Database db = new Database(GetConnectionString(catalog), ProviderName))
				{
					if(isStoredProcedure)
						return db.QuerySP<T>(sql, args);
					return db.Query<T>(sql, args);
				}
			}
			catch (SqlException ex)
			{
				if (ex.Message.StartsWith("P"))
				{
					throw new MixERPException(ex.Message, ex);
				}

				throw new MixERPException(ex.Message, ex);
			}
		}

		public static IEnumerable<T> Get<T>(string catalog, Sql sql)
        {
            try
            {
                using (Database db = new Database(GetConnectionString(catalog), ProviderName))
                {
                    return db.Query<T>(sql);
                }
            }
            catch (NpgsqlException ex)
            {
                if (ex.Code.StartsWith("P"))
                {
                    string errorMessage = GetDBErrorResource(ex);
                    throw new MixERPException(errorMessage, ex);
                }

                throw new MixERPException(ex.Message, ex);
            }
        }

        public static object Insert(string catalog, object poco, string tableName = "", string primaryKeyName = "", bool autoIncrement = true)
        {
            try
            {
                using (Database db = new Database(GetConnectionString(catalog), ProviderName))
                {
                    if (!string.IsNullOrWhiteSpace(tableName) && !string.IsNullOrWhiteSpace(primaryKeyName))
                    {
                        return db.Insert(tableName, primaryKeyName, autoIncrement, poco);
                    }

                    return db.Insert(poco);
                }
            }
            catch (NpgsqlException ex)
            {
                if (ex.Code.StartsWith("P"))
                {
                    string errorMessage = GetDBErrorResource(ex);
                    throw new MixERPException(errorMessage, ex);
                }

                throw new MixERPException(ex.Message, ex);
            }
        }

        public static object Update(string catalog, object poco, object primaryKeyValue, string tableName = "", string primaryKeyName = "")
        {
            try
            {
                using (Database db = new Database(GetConnectionString(catalog), ProviderName))
                {
                    if (!string.IsNullOrWhiteSpace(tableName) && !string.IsNullOrWhiteSpace(primaryKeyName))
                    {
                        return db.Update(tableName, primaryKeyName, poco, primaryKeyValue);
                    }

                    return db.Update(poco, primaryKeyValue);
                }
            }
            catch (NpgsqlException ex)
            {
                if (ex.Code.StartsWith("P"))
                {
                    string errorMessage = GetDBErrorResource(ex);
                    throw new MixERPException(errorMessage, ex);
                }

                throw new MixERPException(ex.Message, ex);
            }
        }

        public static T Scalar<T>(string catalog, string sql, params object[] args)
        {
            try
            {
                using (Database db = new Database(GetConnectionString(catalog), ProviderName))
                {
                    return db.ExecuteScalar<T>(sql, args);
                }
            }
            catch (NpgsqlException ex)
            {
                if (ex.Code.StartsWith("P"))
                {
                    string errorMessage = GetDBErrorResource(ex);
                    throw new MixERPException(errorMessage, ex);
                }

                throw new MixERPException(ex.Message, ex);
            }
        }

        public static T Scalar<T>(string catalog, Sql sql)
        {
            try
            {
                using (Database db = new Database(GetConnectionString(catalog), ProviderName))
                {
                    return db.ExecuteScalar<T>(sql);
                }
            }
            catch (NpgsqlException ex)
            {
                if (ex.Code.StartsWith("P"))
                {
                    string errorMessage = GetDBErrorResource(ex);
                    throw new MixERPException(errorMessage, ex);
                }

                throw new MixERPException(ex.Message, ex);
            }
        }

        public static void NonQuery(string catalog, string sql, params object[] args)
        {
            try
            {
                using (Database db = new Database(GetConnectionString(catalog), ProviderName))
                {
                    db.Execute(sql, args);
                }
            }
            catch (NpgsqlException ex)
            {
                if (ex.Code.StartsWith("P"))
                {
                    string errorMessage = GetDBErrorResource(ex);
                    throw new MixERPException(errorMessage, ex);
                }

                throw new MixERPException(ex.Message, ex);
            }
        }

        public static string GetDBErrorResource(NpgsqlException ex)
        {
            string message = ResourceManager.TryGetResourceFromCache("DbErrors", ex.Code);

            if (string.IsNullOrWhiteSpace(message) || message == ex.Code)
            {
                return ex.Message;
            }

            return message;
        }

        public static string GetConnectionString(string catalog = "")
        {
            string database = ConfigurationHelper.GetDbServerParameter("Database");

            if (!string.IsNullOrWhiteSpace(catalog))
            {
                database = catalog;
            }

            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = ConfigurationHelper.GetDbServerParameter("Server"),
                Database = database,
                UserName = ConfigurationHelper.GetDbServerParameter("UserId"),
                Password = ConfigurationHelper.GetDbServerParameter("Password"),
                Port = int.Parse(ConfigurationHelper.GetDbServerParameter("Port")),
                SyncNotification = true,
                Pooling = true,
                SSL = true,
                SslMode = SslMode.Prefer,
                MinPoolSize = 10,
                MaxPoolSize = 100,
                ApplicationName = "MixERP"
            };
				
				//Microsoft SQL Server Connection String Builder
				if (ProviderName.Contains("SqlClient"))
				{
					var SQLconnectionStringBuilder = new SqlConnectionStringBuilder();
					SQLconnectionStringBuilder.DataSource = ConfigurationHelper.GetDbServerParameter("Server");
					SQLconnectionStringBuilder.InitialCatalog = database;
					string integratedSecurity = ConfigurationHelper.GetDbServerParameter("IntegratedSecurity");
					SQLconnectionStringBuilder.IntegratedSecurity = (integratedSecurity == "yes") ? true : false;
					if(!SQLconnectionStringBuilder.IntegratedSecurity)
					{
						SQLconnectionStringBuilder.UserID = ConfigurationHelper.GetDbServerParameter("UserId");
						SQLconnectionStringBuilder.Password = ConfigurationHelper.GetDbServerParameter("Password");
					}
					return SQLconnectionStringBuilder.ConnectionString;
				}
				return connectionStringBuilder.ConnectionString;
        }
    }

	public sealed class MicrosoftSQLFactory
	{
		public const string ProviderName = "System.Data.SqlClient"; //change provider to Microsoft SQL Server
		public static string MetaDatabase = ConfigurationHelper.GetDbServerParameter("MetaDatabase");

		public static IEnumerable<T> Get<T>(string catalog, string sql, bool isStoredProcedure, params object[] args)
		{
			try
			{
				using (Database db = new Database(GetConnectionString(catalog), ProviderName))
				{
					return db.QuerySP<T>(sql, args);
				}
			}
			catch (NpgsqlException ex)
			{
				if (ex.Code.StartsWith("P"))
				{
					string errorMessage = GetDBErrorResource(ex);
					throw new MixERPException(errorMessage, ex);
				}

				throw new MixERPException(ex.Message, ex);
			}
		}

		public static IEnumerable<T> GetMicrosoftSQL<T>(string catalog, string sql, params SqlParameter[] args)
		{
			try
			{
				using (Database db = new Database(GetConnectionString(catalog), ProviderName))
				{
					return db.Query<T>(sql, args);
				}
			}
			catch (SqlException ex)
			{
				if (ex.Message.StartsWith("P"))
				{
					throw new MixERPException(ex.Message, ex);
				}

				throw new MixERPException(ex.Message, ex);
			}
		}

		public static IEnumerable<T> Get<T>(string catalog, Sql sql)
		{
			try
			{
				using (Database db = new Database(GetConnectionString(catalog), ProviderName))
				{
					return db.Query<T>(sql);
				}
			}
			catch (NpgsqlException ex)
			{
				if (ex.Code.StartsWith("P"))
				{
					string errorMessage = GetDBErrorResource(ex);
					throw new MixERPException(errorMessage, ex);
				}

				throw new MixERPException(ex.Message, ex);
			}
		}

		public static object Insert(string catalog, object poco, string tableName = "", string primaryKeyName = "", bool autoIncrement = true)
		{
			try
			{
				using (Database db = new Database(GetConnectionString(catalog), ProviderName))
				{
					if (!string.IsNullOrWhiteSpace(tableName) && !string.IsNullOrWhiteSpace(primaryKeyName))
					{
						return db.Insert(tableName, primaryKeyName, autoIncrement, poco);
					}

					return db.Insert(poco);
				}
			}
			catch (NpgsqlException ex)
			{
				if (ex.Code.StartsWith("P"))
				{
					string errorMessage = GetDBErrorResource(ex);
					throw new MixERPException(errorMessage, ex);
				}

				throw new MixERPException(ex.Message, ex);
			}
		}

		public static object Update(string catalog, object poco, object primaryKeyValue, string tableName = "", string primaryKeyName = "")
		{
			try
			{
				using (Database db = new Database(GetConnectionString(catalog), ProviderName))
				{
					if (!string.IsNullOrWhiteSpace(tableName) && !string.IsNullOrWhiteSpace(primaryKeyName))
					{
						return db.Update(tableName, primaryKeyName, poco, primaryKeyValue);
					}

					return db.Update(poco, primaryKeyValue);
				}
			}
			catch (NpgsqlException ex)
			{
				if (ex.Code.StartsWith("P"))
				{
					string errorMessage = GetDBErrorResource(ex);
					throw new MixERPException(errorMessage, ex);
				}

				throw new MixERPException(ex.Message, ex);
			}
		}

		public static T Scalar<T>(string catalog, string sql, params object[] args)
		{
			try
			{
				using (Database db = new Database(GetConnectionString(catalog), ProviderName))
				{
					return db.ExecuteScalar<T>(sql, args);
				}
			}
			catch (NpgsqlException ex)
			{
				if (ex.Code.StartsWith("P"))
				{
					string errorMessage = GetDBErrorResource(ex);
					throw new MixERPException(errorMessage, ex);
				}

				throw new MixERPException(ex.Message, ex);
			}
		}

		public static T Scalar<T>(string catalog, Sql sql)
		{
			try
			{
				using (Database db = new Database(GetConnectionString(catalog), ProviderName))
				{
					return db.ExecuteScalar<T>(sql);
				}
			}
			catch (NpgsqlException ex)
			{
				if (ex.Code.StartsWith("P"))
				{
					string errorMessage = GetDBErrorResource(ex);
					throw new MixERPException(errorMessage, ex);
				}

				throw new MixERPException(ex.Message, ex);
			}
		}

		public static void NonQuery(string catalog, string sql, params object[] args)
		{
			try
			{
				using (Database db = new Database(GetConnectionString(catalog), ProviderName))
				{
					db.Execute(sql, args);
				}
			}
			catch (NpgsqlException ex)
			{
				if (ex.Code.StartsWith("P"))
				{
					string errorMessage = GetDBErrorResource(ex);
					throw new MixERPException(errorMessage, ex);
				}

				throw new MixERPException(ex.Message, ex);
			}
		}

		public static string GetDBErrorResource(NpgsqlException ex)
		{
			string message = ResourceManager.TryGetResourceFromCache("DbErrors", ex.Code);

			if (string.IsNullOrWhiteSpace(message) || message == ex.Code)
			{
				return ex.Message;
			}

			return message;
		}

		public static string GetConnectionString(string catalog = "")
		{
			string database = ConfigurationHelper.GetDbServerParameter("Database");

			if (!string.IsNullOrWhiteSpace(catalog))
			{
				database = catalog;
			}

			var connectionStringBuilder = new NpgsqlConnectionStringBuilder
			{
				Host = ConfigurationHelper.GetDbServerParameter("Server"),
				Database = database,
				UserName = ConfigurationHelper.GetDbServerParameter("UserId"),
				Password = ConfigurationHelper.GetDbServerParameter("Password"),
				Port = int.Parse(ConfigurationHelper.GetDbServerParameter("Port")),
				SyncNotification = true,
				Pooling = true,
				SSL = true,
				SslMode = SslMode.Prefer,
				MinPoolSize = 10,
				MaxPoolSize = 100,
				ApplicationName = "MixERP"
			};

			//Microsoft SQL Server Connection String Builder
			if (ProviderName.Contains("SqlClient"))
			{
				var SQLconnectionStringBuilder = new SqlConnectionStringBuilder();
				SQLconnectionStringBuilder.DataSource = ConfigurationHelper.GetDbServerParameter("Server");
				SQLconnectionStringBuilder.InitialCatalog = database;
				string integratedSecurity = ConfigurationHelper.GetDbServerParameter("IntegratedSecurity");
				SQLconnectionStringBuilder.IntegratedSecurity = (integratedSecurity == "yes") ? true : false;
				if (!SQLconnectionStringBuilder.IntegratedSecurity)
				{
					SQLconnectionStringBuilder.UserID = ConfigurationHelper.GetDbServerParameter("UserId");
					SQLconnectionStringBuilder.Password = ConfigurationHelper.GetDbServerParameter("Password");
				}
				return SQLconnectionStringBuilder.ConnectionString;
			}
			return connectionStringBuilder.ConnectionString;
		}
	}
}