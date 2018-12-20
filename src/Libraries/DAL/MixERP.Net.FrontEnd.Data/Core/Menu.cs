using System.Collections.Generic;
using PetaPoco;

namespace MixERP.Net.FrontEnd.Data.Core
{
    public static class Menu
    {
        public static IEnumerable<Entities.Core.Menu> GetMenuCollection(string catalog, int officeId, int userId,
            string culture)
        {
            string sql = "SELECT * FROM policy.get_menu(@0::integer, @1::integer, @2::text) ORDER BY menu_id;";
			if(Factory.ProviderName.ToLower().Contains("sqlclient"))
			{
				sql = "SELECT * FROM policy.get_menu(@0, @1, @2) ORDER BY menu_id;";
			}
            return Factory.Get<Entities.Core.Menu>(catalog, sql, userId, officeId, culture);
        }
    }
}