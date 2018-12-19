using MixERP.Net.ApplicationState;
using System.Collections.ObjectModel;
using System.Linq;

namespace MixERP.Net.WebControls.Common.Helpers
{
    public static class DatePersister
    {
		public const string ProviderName = "SqlClient";
		public static FrequencyDates GetFrequencyDates(string catalog, int officeId)
        {
            Collection<FrequencyDates> applicationDates = Dates.GetFrequencyDates(catalog);
            bool persist = false;

            if (applicationDates == null || applicationDates.Count.Equals(0))
            {
					if(ProviderName== "SqlClient")
						applicationDates = Data.Frequency.MicrosoftSQLGetFrequencyDates(catalog);
					else
						applicationDates = Data.Frequency.GetFrequencyDates(catalog);
                persist = true;
            }
            else
            {
                for (int i = 0; i < applicationDates.Count; i++)
                {
                    if (applicationDates[i].NewDayStarted)
                    {
                        int modelOfficeId = applicationDates[i].OfficeId;

                        applicationDates.Remove(applicationDates[i]);
                        applicationDates.Add(Data.Frequency.GetFrequencyDates(catalog, modelOfficeId));
                        persist = true;
                    }
                }
            }

            if (persist)
            {
                Dates.SetApplicationDates(catalog, applicationDates);
            }

            return applicationDates.FirstOrDefault(c => c.OfficeId.Equals(officeId));
        }
    }
}