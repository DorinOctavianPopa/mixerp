using MixERP.Net.Common;
using MixERP.Net.Common.Helpers;
using Npgsql;
using System.Data.SqlClient;

namespace MixERP.Net.DbFactory
{
    public static class DbConnection
    {
		static public string DBProvider = ConfigurationHelper.GetDbServerParameter("Provider").ToLower();
		public static string GetConnectionString(string catalog)
        {
            CatalogHelper.ValidateCatalog(catalog);

            string host = ConfigurationHelper.GetDbServerParameter("Server");
            string database = ConfigurationHelper.GetDbServerParameter("Database");

            if (!string.IsNullOrWhiteSpace(catalog))
            {
                database = catalog;
            }

            string userId = ConfigurationHelper.GetDbServerParameter("UserId");
            string password = ConfigurationHelper.GetDbServerParameter("Password");
            int port = Conversion.TryCastInteger(ConfigurationHelper.GetDbServerParameter("Port"));
			bool integratedSecurity = ConfigurationHelper.GetDbServerParameter("IntegratedSecurity").ToLower() == "yes" ? true : false;
			if(DBProvider == "sqlclient")
				return GetSQLConnectionString(host, database, userId, password, integratedSecurity);
			return GetConnectionString(host, database, userId, password, port);
        }

        public static string GetConnectionString(string host, string database, string username, string password, int port)
        {
            CatalogHelper.ValidateCatalog(database);

            NpgsqlConnectionStringBuilder connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = host,
                Database = database,
                UserName = username,
                Password = password,
                Port = port,
                SyncNotification = true,
                Pooling = true,
                SSL = true,
                SslMode = SslMode.Prefer,
                MinPoolSize = 10,
                MaxPoolSize = 100,
                ApplicationName = "MixERP"
            };

            return connectionStringBuilder.ConnectionString;
        }


		/// <summary>
		/// Return SQL Connection String for Microsoft SQL Server
		/// </summary>
		/// <param name="host"></param>
		/// <param name="database"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="integratedSecurity"></param>
		/// <returns></returns>
		private static string GetSQLConnectionString(string host, string database, string username, string password,
			bool integratedSecurity)
		{
			SqlConnectionStringBuilder SQLconnectionStringBuilder = new SqlConnectionStringBuilder();
			SQLconnectionStringBuilder.DataSource = host;
			SQLconnectionStringBuilder.InitialCatalog = database;
			SQLconnectionStringBuilder.IntegratedSecurity = integratedSecurity;
			if (!SQLconnectionStringBuilder.IntegratedSecurity)
			{
				SQLconnectionStringBuilder.UserID = username;
				SQLconnectionStringBuilder.Password = password;
			}
			SQLconnectionStringBuilder.ApplicationName = "MixERP";
			return SQLconnectionStringBuilder.ConnectionString;
		}
	}
}