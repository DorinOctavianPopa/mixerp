using System;
using PetaPoco;

namespace MixERP.Net.Entities.Public.Meta
{
    [TableName("public.sales_quotation_validation")]
    [PrimaryKey("validation_id", autoIncrement = false)]
    [ExplicitColumns]
    public class SalesQuotationValidation : PetaPocoDB.Record<SalesQuotationValidation>, IPoco
    {
        [Column("validation_id")]
        public string ValidationId { get; set; }

        [Column("tran_id")]
        public long TranId { get; set; }

        [Column("catalog")]
        public string Catalog { get; set; }

        [Column("valid_till")]
        public DateTime ValidTill { get; set; }

        [Column("added_on")]
        public DateTime AddedOn { get; set; }

        [Column("accepted")]
        public bool Accepted { get; set; }

        [Column("accepted_on")]
        public DateTime? AcceptedOn { get; set; }

        public static void CreateTable()
        {
            string sql = @"DO
                                $$
                                BEGIN
                                    IF NOT EXISTS (
                                        SELECT 1 
                                        FROM   pg_catalog.pg_class c
                                        JOIN   pg_catalog.pg_namespace n ON n.oid = c.relnamespace
                                        WHERE  n.nspname = 'public'
                                        AND    c.relname = 'sales_quotation_validation'
                                        AND    c.relkind = 'r'
                                    ) THEN
                                        CREATE TABLE public.sales_quotation_validation
                                        (
                                            validation_id           text NOT NULL PRIMARY KEY,
                                            tran_id                 bigint NOT NULL,
                                            catalog                 text NOT NULL,
                                            added_on                TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT(NOW()),
                                            valid_till              TIMESTAMP WITH TIME ZONE NOT NULL,
                                            accepted                boolean NOT NULL DEFAULT(false),
                                            accepted_on             TIMESTAMP WITH TIME ZONE,
                                                                    CONSTRAINT sales_quotation_validation_unq
                                                                    UNIQUE(tran_id, catalog)
                                        );
                                    END IF;
                                END
                                $$
                                LANGUAGE plpgsql;
                                ";
				//Microsoft SQL Server
				if (Factory.ProviderName.Contains("SqlClient"))
				{
					sql = @"IF NOT  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sales_quotation_validation]') AND type in (N'U'))
									BEGIN
										CREATE TABLE [dbo].[sales_quotation_validation]
																(
																validation_id           varchar(50) NOT NULL PRIMARY KEY,
																tran_id                 bigint NOT NULL,
																catalog                 varchar(50) NOT NULL,
																added_on                DATETIME NOT NULL DEFAULT(GETDATE()),
																valid_till              DATETIME NOT NULL,
																accepted                bit NOT NULL DEFAULT(0),
																accepted_on             DATETIME
																);

										CREATE UNIQUE NONCLUSTERED INDEX IX_sales_quotation_validation ON dbo.sales_quotation_validation
										(
											tran_id,
											catalog
										) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

									END";
				}
				Factory.NonQuery(Factory.MetaDatabase, sql);
        }
    }
}