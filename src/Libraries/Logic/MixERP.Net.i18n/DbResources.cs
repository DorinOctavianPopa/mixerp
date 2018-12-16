using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Data.SqlClient;

namespace MixERP.Net.i18n
{
    internal static class DbResources
    {
        internal static IDictionary<string, string> GetLocalizedResources()
        {
				if (ConnectionStringHelper.DBProvider == "sqlclient")
					return GetSqlLocalizedResources();

				return GetNpgLocalizedResources();
        }

		internal static IDictionary<string, string> GetNpgLocalizedResources()
		{
			Dictionary<string, string> resources = new Dictionary<string, string>();
			const string sql = "SELECT * FROM localization.localized_resource_view;";
			using (NpgsqlCommand command = new NpgsqlCommand(sql))
			{
				using (DataTable table = GetDataTable(command))
				{
					if (table == null || table.Rows == null)
					{
						return resources;
					}

					foreach (DataRow row in table.Rows)
					{
						string key = row[0].ToString();
						string value = row[1].ToString();

						resources.Add(key, value);
					}
				}
			}

			return resources;
		}


		internal static IDictionary<string, string> GetSqlLocalizedResources()
		{
			Dictionary<string, string> resources = new Dictionary<string, string>();
			const string sql = "SELECT * FROM localization.localized_resource_view;";
			using (SqlCommand command = new SqlCommand(sql))
			{
				using (DataTable table = GetDataTable(command))
				{
					if (table == null || table.Rows == null)
					{
						return resources;
					}

					foreach (DataRow row in table.Rows)
					{
						string key = row[0].ToString();
						string value = row[1].ToString();

						resources.Add(key, value);
					}
				}
			}

			return resources;
		}

		internal static IEnumerable<LocalizedResource> GetLocalizationTable(string language)
        {
            List<LocalizedResource> resources = new List<LocalizedResource>();
            const string sql = "SELECT * FROM localization.get_localization_table(@Language) WHERE COALESCE(key, '') != '';";

            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionStringHelper.GetConnectionString()))
            {
                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Language", language);

                    using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command))
                    {
                        using (DataTable dataTable = new DataTable())
                        {
                            dataTable.Locale = CultureManager.GetCurrent();
                            adapter.Fill(dataTable);

                            if (dataTable.Rows.Count > 0)
                            {
                                foreach (DataRow row in dataTable.Rows)
                                {
                                    LocalizedResource resource = new LocalizedResource();

                                    resource.Id = long.Parse(row["id"].ToString());
                                    resource.ResourceClass = row["resource_class"].ToString();
                                    resource.Key = row["key"].ToString();
                                    resource.Original = row["original"].ToString();
                                    resource.Translated = row["translated"].ToString();

                                    resources.Add(resource);
                                }
                            }
                        }
                    }
                }
            }

            return resources;
        }

        private static DataTable GetDataTable(NpgsqlCommand command)
        {
            string connectionString = ConnectionStringHelper.GetConnectionString();

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                command.Connection = connection;

                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command))
                {
                    using (DataTable dataTable = new DataTable())
                    {
                        dataTable.Locale = CultureManager.GetCurrent();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }
		/// <summary>
		/// return data for Microsoft SQL Command 
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		private static DataTable GetDataTable(SqlCommand command)
		{
			string connectionString = ConnectionStringHelper.GetSqlConnectionString();

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				command.Connection = connection;

				using (SqlDataAdapter adapter = new SqlDataAdapter(command))
				{
					using (DataTable dataTable = new DataTable())
					{
						dataTable.Locale = CultureManager.GetCurrent();
						adapter.Fill(dataTable);
						return dataTable;
					}
				}
			}
		}

	}
}