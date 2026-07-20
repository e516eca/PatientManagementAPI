using System.Text.Json;

namespace PatientManagementAPI.utilities
{
    public class JsonValidator
    {
        public static bool IsValidJson(string jsonString)
        {
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                return false;
            }

            try
            {
                // JsonDocument.Parse throws a JsonException if the string is invalid JSON
                using (JsonDocument.Parse(jsonString))
                {
                    return true;
                }
            }
            catch (JsonException)
            {
                return false;
            }
        }
    }
}
