using Microsoft.EntityFrameworkCore;
using System.Linq;
using jlcsolutionscr.com.visitortracking.webapi.dataaccess.domain;

namespace jlcsolutionscr.com.visitortracking.webapi.dataaccess
{
    public class VisitorTrackingContext : DbContext
    {
        private readonly string _connectionString;
        public VisitorTrackingContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Branch>()
                .HasKey(c => new { c.CompanyId, c.Id });
            modelBuilder.Entity<Employee>()
                .HasKey(c => new { c.CompanyId, c.Id });
            modelBuilder.Entity<Service>()
                .HasKey(c => new { c.CompanyId, c.Id });
            modelBuilder.Entity<RolePerUser>()
                .HasKey(c => new { c.RoleId, c.UserId });
        }
        
        public DbSet<Company> CompanyRepository { get; set; }
        public DbSet<Branch> BranchRepository { get; set; }
        public DbSet<Customer> CustomerRepository { get; set; }
        public DbSet<Employee> EmployeeRepository { get; set; }
        public DbSet<Registry> RegistryRepository { get; set; }
        public DbSet<Activity> ActivityRepository { get; set; }
        public DbSet<User> UserRepository { get; set; }
        public DbSet<Service> ServiceRepository { get; set; }
        public DbSet<Role> RoleRepository { get; set; }
        public DbSet<RolePerUser> RolePerUserRepository { get; set; }
        public DbSet<Parameter> ParameterRepository { get; set; }
        public DbSet<AuthorizationEntry> AuthorizationEntryRepository { get; set; }


        public void ChangeNotify<TEntity>(TEntity entidad) where TEntity : class
        {
            Entry<TEntity>(entidad).State = EntityState.Modified;
        }

        public void RemoveNotify<TEntity>(TEntity entidad) where TEntity : class
        {
            Entry<TEntity>(entidad).State = EntityState.Deleted;
        }

        public void ExecuteProcedure(string procedureName, object[] objParameters)
        {
            Database.ExecuteSqlRaw("call " + procedureName, objParameters);
        }

        public void Commit()
        {
            SaveChanges();
        }

        public void RollBack()
        {
            var changedEntries = ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged).ToList();

            foreach (var entry in changedEntries.Where(x => x.State == EntityState.Modified))
            {
                entry.CurrentValues.SetValues(entry.OriginalValues);
                entry.State = EntityState.Unchanged;
            }

            foreach (var entry in changedEntries.Where(x => x.State == EntityState.Added))
            {
                entry.State = EntityState.Detached;
            }

            foreach (var entry in changedEntries.Where(x => x.State == EntityState.Deleted))
            {
                entry.State = EntityState.Unchanged;
            }
        }
    }
}