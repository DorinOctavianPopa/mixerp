namespace PetaPoco
{
    public class AccessValidator
    {
        public static bool Validate(IPolicy policy)
        {
            if (policy.LoginId == 0)
            {
                return false;
            }

            string sql = "SELECT * FROM policy.has_access(audit.get_user_id_by_login_id(@0), @1, @2);";
            var entity = policy.ObjectNamespace + "." + policy.ObjectName;
            int type = (int)policy.AccessType;
			bool result = false;

			if(Factory.ProviderName.ToLower().Contains("sqlclient"))
			{
				sql = "SELECT policy.has_access(audit.get_user_id_by_login_id(@0), @1, @2);";
				result = Factory.Scalar<bool>(policy.Catalog, sql, policy.LoginId, entity, type);
			}
			else
				result = Factory.Scalar<bool>(policy.Catalog, sql, policy.LoginId, entity, type);
            return result;
        }
    }
}