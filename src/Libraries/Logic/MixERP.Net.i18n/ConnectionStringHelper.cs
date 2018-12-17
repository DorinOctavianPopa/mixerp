using Npgsql;
using System.Configuration;
using System.Web.Hosting;
using System.Data.SqlClient;

namespace MixERP.Net.i18n
{
    internal static class ConnectionStringHelper
    {

		static public string DBProvider  = GetDbServerParameter("Provider").ToLower();
		static public string ConnectionString = string.Empty;

		public static string GetConnectionString()
		{
			if (ConnectionString == string.Empty)
			{
				string host = GetDbServerParameter("Server");
				string database = GetDbServerParameter("Database");
				string userId = GetDbServerParameter("UserId");
				string password = GetDbServerParameter("Password");
				int port = int.Parse(GetDbServerParameter("Port"));

				ConnectionString =  GetConnectionString(host, database, userId, password, port);
			}

			return ConnectionString;
		}

		public static string GetSqlConnectionString()
		{
			if (ConnectionString == string.Empty)
			{
				string host = GetDbServerParameter("Server");
				string database = GetDbServerParameter("Database");
				string userId = GetDbServerParameter("UserId");
				string password = GetDbServerParameter("Password");
				bool integratedSecurity = GetDbServerParameter("IntegratedSecurity").ToLower() == "yes" ? true : false;

				ConnectionString = GetSQLConnectionString(host, database, userId, password, integratedSecurity); 
			}

			return ConnectionString;
		}

		private static string GetDbServerParameter(string keyName)
        {
            return GetConfigurationValue("DbServerConfigFileLocation", keyName);
        }

        private static string GetConfigurationValue(string configFileName, string sectionName)
        {
            if (configFileName == null)
            {
                return string.Empty;
            }

            string path = HostingEnvironment.MapPath(ConfigurationManager.AppSettings[configFileName]);

            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap {ExeConfigFilename = path};
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap,
                ConfigurationUserLevel.None);
            AppSettingsSection section = config.GetSection("appSettings") as AppSettingsSection;

            if (section != null)
            {
                if (section.Settings[sectionName] != null)
                {
                    return section.Settings[sectionName].Value;
                }
            }

            return string.Empty;
        }

        private static string GetConnectionString(string host, string database, string username, string password,
            int port)
        {
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