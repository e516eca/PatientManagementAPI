namespace PatientManagementAPI.Options
{
    public class FhirServerOptions
    {
        public const string SectionName = "FhirServer";

        public required string BaseUrl { get; init; }

        public required string BearerToken { get; init; }

        public int TimeoutSeconds { get; init; } = 60;
    }
}

