using System.Collections.Generic;
using MixERP.Net.Entities.Transactions;
using PetaPoco;

namespace MixERP.Net.Core.Modules.Sales.Data.Reports
{
    public static class SalesByMonth
    {
        public static IEnumerable<DbGetSalesByOfficesResult> GetSalesByOffice(string catalog, int officeId)
        {
            string sql = "SELECT * FROM transactions.get_sales_by_offices(@0::integer, 1000);";
			if(Factory.ProviderName.ToLower().Contains("sqlclient"))
				sql = "SELECT * FROM transactions.get_sales_by_office(@0, 1000);";
			return Factory.Get<DbGetSalesByOfficesResult>(catalog, sql, officeId);
        }

        public static IEnumerable<DbGetSalesByOfficesResult> GetSalesByOffice(string catalog)
        {
            const string sql = "SELECT * FROM transactions.get_sales_by_offices(1000);";
            return Factory.Get<DbGetSalesByOfficesResult>(catalog, sql);
        }
    }
}