using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MixERP.Net.Entities.Office;
using MixERP.Net.Framework;
using Npgsql;
using PetaPoco;
using System.Data.SqlClient;

namespace MixERP.Net.FrontEnd.Data.Office
{
    public static class Offices
    {
        public static IEnumerable<DbGetOfficesResult> GetOffices(string catalog)
        {
            return Factory.Get<DbGetOfficesResult>(catalog, "SELECT * FROM office.get_offices();");
        }

        public static void SaveOffice(string catalog, string regionalDataFile, string officeCode, string officeName,
            string nickName,
            DateTime registrationDate, string currencyCode, string currencySymbol, string currencyName,
            string hundredthName, string fiscalYearCode,
            string fiscalYearName, DateTime startsFrom, DateTime endsOn,
            bool salesTaxIsVat, bool hasStateSalesTax, bool hasCountySalesTax,
            int quotationValidDays, decimal incomeTaxRate, int weekStartDay, DateTime transactionStartDate,
            bool isPerpetual, string valuationMethod, string logo,
            string adminName, string username, string password)
        {
            try
            {
                using (Database db = new Database(Factory.GetConnectionString(catalog), Factory.ProviderName))
                {
                    using (Transaction transaction = db.GetTransaction())
                    {
                        string sql = File.ReadAllText(regionalDataFile, Encoding.UTF8);
								try
								{
									db.Execute(sql);
								}
								catch (Exception ex)
								{
									//do nothing
									if(!Factory.ProviderName.ToLower().Contains("sqlclient"))
										throw ex;
								}


								if (Factory.ProviderName.ToLower().Contains("sqlclient"))
								{
									sql = "office.add_office";
									
									db.ExecuteProcedure(sql, new SqlParameter("office_code",officeCode), new SqlParameter("office_name",officeName),
										 new SqlParameter("nick_name",nickName), new SqlParameter("registration_date",registrationDate), new SqlParameter("currency_code",currencyCode),
										 new SqlParameter("currency_symbol",currencySymbol), new SqlParameter("currency_name",currencyName),
										 new SqlParameter("hundredth_name",hundredthName), new SqlParameter("fiscal_year_code",fiscalYearCode), new SqlParameter("fiscal_year_name",fiscalYearName),
										 new SqlParameter("starts_from",startsFrom),
										 new SqlParameter("ends_on",endsOn),
										 new SqlParameter("sales_tax_is_vat",salesTaxIsVat), new SqlParameter("has_state_sales_tax",hasStateSalesTax), new SqlParameter("has_county_sales_tax",hasCountySalesTax),
										 new SqlParameter("quotation_valid_days",quotationValidDays),
										 new SqlParameter("income_tax_rate",incomeTaxRate), new SqlParameter("week_start_day",weekStartDay), new SqlParameter("transaction_start_date",transactionStartDate), 
										 new SqlParameter("is_perpetual",isPerpetual), new SqlParameter("inv_valuation_method",valuationMethod), new SqlParameter("logo_file",logo),
										 new SqlParameter("admin_name",adminName),
										 new SqlParameter("user_name",username), new SqlParameter("password",password));
								}
								else
								{
									sql =
										 "SELECT * FROM office.add_office(@0::varchar(12), @1::varchar(150), @2::varchar(50), @3::date, @4::varchar(12), @5::varchar(12), @6::varchar(48), @7::varchar(48), @8::varchar(12), @9::varchar(50), @10::date,@11::date, @12::boolean, @13::boolean, @14::boolean, @15::integer, @16::numeric, @17::integer, @18::date, @19::boolean, @20::character varying(5), @21::text, @22::varchar(100), @23::varchar(50), @24::varchar(48));";
									db.Execute(sql, officeCode, officeName, nickName, registrationDate, currencyCode,
										 currencySymbol, currencyName, hundredthName, fiscalYearCode, fiscalYearName, startsFrom,
										 endsOn,
										 salesTaxIsVat, hasStateSalesTax, hasCountySalesTax, quotationValidDays,
										 incomeTaxRate, weekStartDay, transactionStartDate, isPerpetual, valuationMethod, logo,
										 adminName,
										 username, password);
								}
                        transaction.Complete();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                if (ex.Code.StartsWith("P"))
                {
                    string errorMessage = Factory.GetDBErrorResource(ex);
                    throw new MixERPException(errorMessage, ex);
                }

                throw;
            }
        }
    }
}