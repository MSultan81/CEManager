namespace CEManager
{
    using CEManager.Models;
    using System.Data.Entity;

    public class DBConnection : DbContext
    {
        public DBConnection() : base("Data Source=SULTAN;Initial Catalog=CEManager;Integrated Security=True")
        {
        }
        public DbSet<Company> Company { get; set; }
        public DbSet<Employee> Employee { get; set; }
    }
}
