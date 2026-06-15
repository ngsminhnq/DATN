using HRemployee.Entities;
using HRemployee.Enums;
using Microsoft.EntityFrameworkCore;

namespace HRemployee.DataContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasOne(e => e.Manager).WithMany(e => e.Subordinates).HasForeignKey(e => e.ManagerId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LeaveRequest>().HasOne(lr => lr.Employee).WithMany(e => e.LeaveRequests).HasForeignKey(lr => lr.EmployeeId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LeaveRequest>().HasOne(lr => lr.ApprovedBy).WithMany(e => e.ApprovedLeaveRequests).HasForeignKey(lr => lr.ApprovedById).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>().HasOne(u => u.Employee).WithOne(e => e.User).HasForeignKey<User>(u => u.EmployeeId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.EmployeeId).IsUnique();
            modelBuilder.Entity<RefreshToken>().HasIndex(r => r.Token).IsUnique();
            modelBuilder.Entity<Department>().HasIndex(d => d.Code).IsUnique();
            modelBuilder.Entity<Position>().HasIndex(p => p.Code).IsUnique();
            modelBuilder.Entity<Employee>().HasIndex(e => e.EmployeeCode).IsUnique();
            modelBuilder.Entity<Employee>().HasIndex(e => e.Email).IsUnique();
            modelBuilder.Entity<Contract>().HasIndex(c => c.ContractCode).IsUnique();
            modelBuilder.Entity<LeaveType>().HasIndex(lt => lt.Name).IsUnique();
            modelBuilder.Entity<Attendance>().HasIndex(a => new { a.EmployeeId, a.Date }).IsUnique();
            modelBuilder.Entity<SalaryRecord>().HasIndex(s => new { s.EmployeeId, s.Month, s.Year }).IsUnique();

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Director" }, new Role { Id = 2, Name = "BlockManager" }, new Role { Id = 3, Name = "CenterManager" }, new Role { Id = 4, Name = "Employee" });

            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "Ban Giám đốc", Code = "BGD", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Department { Id = 2, Name = "Phòng Nhân sự", Code = "HR", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Department { Id = 3, Name = "Phòng Kỹ thuật", Code = "IT", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );

            modelBuilder.Entity<Position>().HasData(
                new Position { Id = 1, Name = "Giám đốc", Code = "GD", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Position { Id = 2, Name = "Trưởng phòng", Code = "TP", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Position { Id = 3, Name = "Nhân viên", Code = "NV", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );

            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, EmployeeCode = "NV001", FullName = "Giám Đốc", Email = "director@company.com", HireDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), Status = EmployeeStatusEnum.Active, DepartmentId = 1, PositionId = 1, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Employee { Id = 2, EmployeeCode = "NV002", FullName = "Giám Đốc Khối", Email = "blockmanager@company.com", HireDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), Status = EmployeeStatusEnum.Active, DepartmentId = 2, PositionId = 2, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Employee { Id = 3, EmployeeCode = "NV003", FullName = "Trưởng Ban", Email = "centermanager@company.com", HireDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), Status = EmployeeStatusEnum.Active, DepartmentId = 2, PositionId = 2, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Employee { Id = 4, EmployeeCode = "NV004", FullName = "Nhân Viên", Email = "employee@company.com", HireDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), Status = EmployeeStatusEnum.Active, DepartmentId = 3, PositionId = 3, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );

            var hashedPassword = "$2a$11$/3VEyfoi2uk.gSuxwneHqOeoUVZcYptrjWQkMqQJ3hDimSvck.c4K";
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "director", Email = "director@company.com", Password = hashedPassword, IsActive = true, RoleId = 1, EmployeeId = 1, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new User { Id = 2, Username = "blockmanager", Email = "blockmanager@company.com", Password = hashedPassword, IsActive = true, RoleId = 2, EmployeeId = 2, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new User { Id = 3, Username = "centermanager", Email = "centermanager@company.com", Password = hashedPassword, IsActive = true, RoleId = 3, EmployeeId = 3, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new User { Id = 4, Username = "employee", Email = "employee@company.com", Password = hashedPassword, IsActive = true, RoleId = 4, EmployeeId = 4, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );

            modelBuilder.Entity<LeaveType>().HasData(
                new LeaveType { Id = 1, Name = "Nghỉ phép năm", IsPaid = true, Description = "12 ngày/năm" },
                new LeaveType { Id = 2, Name = "Nghỉ bệnh", IsPaid = false, Description = "Có giấy bác sĩ" },
                new LeaveType { Id = 3, Name = "Nghỉ cưới", IsPaid = true, Description = "3 ngày" },
                new LeaveType { Id = 4, Name = "Nghỉ tang", IsPaid = true, Description = "3 ngày" }
            );

            modelBuilder.Entity<Contract>().HasData(
                new Contract { Id = 1, ContractCode = "HD001", ContractType = ContractTypeEnum.ChinhThuc, SalaryPercent = 100, StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), EndDate = new DateTime(2028, 1, 1, 0, 0, 0, DateTimeKind.Utc), Status = ContractStatusEnum.Active, EmployeeId = 1, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Contract { Id = 2, ContractCode = "HD002", ContractType = ContractTypeEnum.ChinhThuc, SalaryPercent = 100, StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), EndDate = new DateTime(2028, 1, 1, 0, 0, 0, DateTimeKind.Utc), Status = ContractStatusEnum.Active, EmployeeId = 2, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Contract { Id = 3, ContractCode = "HD003", ContractType = ContractTypeEnum.ChinhThuc, SalaryPercent = 100, StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), EndDate = new DateTime(2028, 1, 1, 0, 0, 0, DateTimeKind.Utc), Status = ContractStatusEnum.Active, EmployeeId = 3, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Contract { Id = 4, ContractCode = "HD004", ContractType = ContractTypeEnum.ChinhThuc, SalaryPercent = 100, StartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), EndDate = new DateTime(2028, 1, 1, 0, 0, 0, DateTimeKind.Utc), Status = ContractStatusEnum.Active, EmployeeId = 4, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<SalaryConfig> SalaryConfigs { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<SalaryRecord> SalaryRecords { get; set; }
    }
}