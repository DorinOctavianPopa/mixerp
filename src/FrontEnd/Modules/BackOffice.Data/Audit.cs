using System.Linq;
using MixERP.Net.Entities.Audit;
using PetaPoco;
using System.Data.SqlClient;

namespace MixERP.Net.Core.Modules.BackOffice.Data
{
    public class Audit
    {
        public static DbGetOfficeInformationModelResult GetOfficeInformationModel(string catalog, int userId)
        {
            string sql = "SELECT * FROM audit.get_office_information_model(@0::integer);";
			if (Factory.ProviderName.ToLower().Contains("sqlclient"))
			{
				sql = "audit.get_office_information_model";
				return Factory.Get<DbGetOfficeInformationModelResult>(catalog, sql, true, new SqlParameter("user_id",userId)).FirstOrDefault();
			}
			return Factory.Get<DbGetOfficeInformationModelResult>(catalog, sql, userId).FirstOrDefault();
        }
    }
}