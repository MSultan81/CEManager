namespace CEManager.Models
{
    public class Company
    {
        public int CompanyId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Website { get; set; }

        public DateTime AddedDate { get; set; }

        public int NumberOfEmployees { get; set; }

        public int ManagerId { get; set; }

        public int Status { get; set; }

    }
}
