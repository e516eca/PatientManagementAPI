namespace PatientManagementAPI.Services
{
    internal class PaginatedResult
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<object> Patients { get; set; }
        public int TotalCount { get; set; }
        public bool HasNextPage { get; set; }
        public object NextUrl { get; set; }
    }
}