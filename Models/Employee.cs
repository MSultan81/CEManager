namespace CEManager.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        public string Position { get; set; }

        public bool IsManager { get; set; }

        public int Status { get; set; }

    }
}
