using Microsoft.EntityFrameworkCore;
using byteflow_server.Models;
using System.Text.Json;
namespace byteflow_server.DataAccess
{
    public class ByteFlowDbContext : DbContext
    {
        public ByteFlowDbContext(DbContextOptions<ByteFlowDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<LeaveRequest> Leaves { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            var valueComparer = new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<string>>(
                (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c != null ? c.ToList() : new List<string>()
            );

            modelBuilder.Entity<Role>()
                .Property(r => r.PermissionArray)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
                )
                .Metadata.SetValueComparer(valueComparer);

            // Configure LeaveRequest foreign keys
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.Reviewer)
                .WithMany()
                .HasForeignKey(lr => lr.ReviewedBy)
                .OnDelete(DeleteBehavior.NoAction); // Prevent cascading delete for reviewedBy

            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.LeaveTaker)
                .WithMany()
                .HasForeignKey(lr => lr.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
