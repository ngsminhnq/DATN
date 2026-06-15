using System;
using HRemployee.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HRemployee.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260407040420_UpdateSeedData4Accounts")]
    partial class UpdateSeedData4Accounts
    {

        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.2").HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("HRemployee.Entities.Attendance", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CheckInTime").HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("CheckOutTime").HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("CreatedAt").HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("Date").HasColumnType("timestamp with time zone");

                    b.Property<int>("EmployeeId").HasColumnType("integer");

                    b.Property<string>("Note").HasColumnType("text");

                    b.Property<int>("Status").HasColumnType("integer");

                    b.Property<decimal?>("WorkingDays").HasColumnType("numeric");

                    b.Property<decimal?>("WorkingHours").HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId", "Date").IsUnique();

                    b.ToTable("Attendances");
                });

            modelBuilder.Entity("HRemployee.Entities.Contract", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ContractCode").IsRequired().HasColumnType("text");

                    b.Property<int>("ContractType").HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt").HasColumnType("timestamp with time zone");

                    b.Property<int>("EmployeeId").HasColumnType("integer");

                    b.Property<DateTime>("EndDate").HasColumnType("timestamp with time zone");

                    b.Property<string>("Note").HasColumnType("text");

                    b.Property<int>("SalaryPercent").HasColumnType("integer");

                    b.Property<DateTime>("StartDate").HasColumnType("timestamp with time zone");

                    b.Property<int>("Status").HasColumnType("integer");

                    b.Property<DateTime?>("TerminatedAt").HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("ContractCode").IsUnique();

                    b.HasIndex("EmployeeId");

                    b.ToTable("Contracts");
                });

            modelBuilder.Entity("HRemployee.Entities.Department", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Code").IsRequired().HasColumnType("text");

                    b.Property<DateTime>("CreatedAt").HasColumnType("timestamp with time zone");

                    b.Property<string>("Description").HasColumnType("text");

                    b.Property<bool>("IsDeleted").HasColumnType("boolean");

                    b.Property<string>("Name").IsRequired().HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Code").IsUnique();

                    b.ToTable("Departments");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Code = "BGD",
                            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            IsDeleted = false,
                            Name = "Ban Giám đốc"
                        },
                        new
                        {
                            Id = 2,
                            Code = "HR",
                            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            IsDeleted = false,
                            Name = "Phòng Nhân sự"
                        },
                        new
                        {
                            Id = 3, Code = "IT", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, Name = "Phòng Kỹ thuật" });
                });

            modelBuilder.Entity("HRemployee.Entities.Employee", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Address").HasColumnType("text");

                    b.Property<string>("Avatar").HasColumnType("text");

                    b.Property<DateTime>("CreatedAt").HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DateOfBirth").HasColumnType("timestamp with time zone");

                    b.Property<int>("DepartmentId").HasColumnType("integer");

                    b.Property<string>("Email").IsRequired().HasColumnType("text");

                    b.Property<string>("EmployeeCode").IsRequired().HasColumnType("text");

                    b.Property<string>("FullName").IsRequired().HasColumnType("text");

                    b.Property<string>("Gender").HasColumnType("text");

                    b.Property<DateTime>("HireDate").HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsDeleted").HasColumnType("boolean");

                    b.Property<int?>("ManagerId").HasColumnType("integer");

                    b.Property<string>("Phone").HasColumnType("text");

                    b.Property<int>("PositionId").HasColumnType("integer");

                    b.Property<int>("Status").HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedAt").HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("Email").IsUnique();

                    b.HasIndex("EmployeeCode").IsUnique();

                    b.HasIndex("ManagerId");

                    b.HasIndex("PositionId");

                    b.ToTable("Employees");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            DepartmentId = 1,
                            Email = "director@company.com",
                            EmployeeCode = "NV001",
                            FullName = "Giám Đốc",
                            HireDate = new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            IsDeleted = false,
                            PositionId = 1,
                            Status = 1
                        },
                        new
                        {
                            Id = 2,
                            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            DepartmentId = 2,
                            Email = "blockmanager@company.com",
                            EmployeeCode = "NV002",
                            FullName = "Giám Đốc Khối",
                            HireDate = new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            IsDeleted = false,
                            PositionId = 2,
                            Status = 1
                        },
                        new
                        {
                            Id = 3,
                            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            DepartmentId = 2,
                            Email = "centermanager@company.com",
                            EmployeeCode = "NV003",
                            FullName = "Trưởng Ban",
                            HireDate = new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            IsDeleted = false,
                            PositionId = 2,
                            Status = 1
                        },
                        new
                        {
                            Id = 4, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), DepartmentId = 3, Email = "employee@company.com", EmployeeCode = "NV004", FullName = "Nhân Viên", HireDate = new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, PositionId = 3, Status = 1 });
                });

            modelBuilder.Entity("HRemployee.Entities.LeaveRequest", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("ApprovedAt").HasColumnType("timestamp with time zone");

                    b.Property<int?>("ApprovedById").HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt").HasColumnType("timestamp with time zone");

                    b.Property<int>("EmployeeId").HasColumnType("integer");

                    b.Property<DateTime>("FromDate").HasColumnType("timestamp with time zone");

                    b.Property<int>("LeaveTypeId").HasColumnType("integer");

                    b.Property<string>("Reason").IsRequired().HasColumnType("text");

                    b.Property<string>("RejectReason").HasColumnType("text");

                    b.Property<int>("Status").HasColumnType("integer");

                    b.Property<DateTime>("ToDate").HasColumnType("timestamp with time zone");

                    b.Property<decimal>("TotalDays").HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("ApprovedById");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("LeaveTypeId");

                    b.ToTable("LeaveRequests");
                });

            modelBuilder.Entity("HRemployee.Entities.LeaveType", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description").HasColumnType("text");

                    b.Property<bool>("IsPaid").HasColumnType("boolean");

                    b.Property<string>("Name").IsRequired().HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name").IsUnique();

                    b.ToTable("LeaveTypes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "12 ngày/năm",
                            IsPaid = true,
                            Name = "Nghỉ phép năm"
                        },
                        new
                        {
                            Id = 2,
                            Description = "Có giấy bác sĩ",
                            IsPaid = false,
                            Name = "Nghỉ bệnh"
                        },
                        new
                        {
                            Id = 3,
                            Description = "3 ngày",
                            IsPaid = true,
                            Name = "Nghỉ cưới"
                        },
                        new
                        {
                            Id = 4, Description = "3 ngày", IsPaid = true, Name = "Nghỉ tang" });
                });

            modelBuilder.Entity("HRemployee.Entities.Position", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Code").IsRequired().HasColumnType("text");

                    b.Property<DateTime>("CreatedAt").HasColumnType("timestamp with time zone");

                    b.Property<string>("Description").HasColumnType("text");

                    b.Property<bool>("IsDeleted").HasColumnType("boolean");

                    b.Property<string>("Name").IsRequired().HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Code").IsUnique();

                    b.ToTable("Positions");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Code = "GD",
                            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            IsDeleted = false,
                            Name = "Giám đốc"
                        },
                        new
                        {
                            Id = 2,
                            Code = "TP",
                            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            IsDeleted = false,
                            Name = "Trưởng phòng"
                        },
                        new
                        {
                            Id = 3, Code = "NV", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false, Name = "Nhân viên" });
                });

            modelBuilder.Entity("HRemployee.Entities.RefreshToken", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt").HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ExpiryDate").HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsRevoked").HasColumnType("boolean");

                    b.Property<string>("Token").IsRequired().HasColumnType("text");

                    b.Property<int>("UserId").HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Token").IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("HRemployee.Entities.Role", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name").IsRequired().HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Director"
                        },
                        new
                        {
                            Id = 2,
                            Name = "BlockManager"
                        },
                        new
                        {
                            Id = 3,
                            Name = "CenterManager"
                        },
                        new
                        {
                            Id = 4, Name = "Employee" });
                });

            modelBuilder.Entity("HRemployee.Entities.SalaryConfig", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Allowance").HasColumnType("numeric");

                    b.Property<decimal>("BaseSalary").HasColumnType("numeric");

                    b.Property<DateTime>("CreatedAt").HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("EffectiveDate").HasColumnType("timestamp with time zone");

                    b.Property<int>("EmployeeId").HasColumnType("integer");

                    b.Property<bool>("IsActive").HasColumnType("boolean");

                    b.Property<decimal>("KpiBonus").HasColumnType("numeric");

                    b.Property<string>("Note").HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId");

                    b.ToTable("SalaryConfigs");
                });

            modelBuilder.Entity("HRemployee.Entities.SalaryRecord", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Allowance").HasColumnType("numeric");

                    b.Property<decimal>("BaseSalary").HasColumnType("numeric");

                    b.Property<DateTime>("CreatedAt").HasColumnType("timestamp with time zone");

                    b.Property<int>("EmployeeId").HasColumnType("integer");

                    b.Property<decimal>("GrossSalary").HasColumnType("numeric");

                    b.Property<decimal>("InsuranceDeduction").HasColumnType("numeric");

                    b.Property<decimal>("KpiBonus").HasColumnType("numeric");

                    b.Property<decimal>("KpiCoefficient").HasColumnType("numeric");

                    b.Property<int>("Month").HasColumnType("integer");

                    b.Property<decimal>("NetSalary").HasColumnType("numeric");

                    b.Property<string>("Note").HasColumnType("text");

                    b.Property<int>("SalaryPercent").HasColumnType("integer");

                    b.Property<int>("StandardDays").HasColumnType("integer");

                    b.Property<int>("Status").HasColumnType("integer");

                    b.Property<decimal>("TaxDeduction").HasColumnType("numeric");

                    b.Property<decimal>("WorkingDays").HasColumnType("numeric");

                    b.Property<int>("Year").HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId", "Month", "Year").IsUnique();

                    b.ToTable("SalaryRecords");
                });

            modelBuilder.Entity("HRemployee.Entities.User", b =>
                {
                    b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt").HasColumnType("timestamp with time zone");

                    b.Property<string>("Email").IsRequired().HasColumnType("text");

                    b.Property<int?>("EmployeeId").HasColumnType("integer");

                    b.Property<bool>("IsActive").HasColumnType("boolean");

                    b.Property<string>("Password").IsRequired().HasColumnType("text");

                    b.Property<int>("RoleId").HasColumnType("integer");

                    b.Property<string>("Username").IsRequired().HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Email").IsUnique();

                    b.HasIndex("EmployeeId").IsUnique();

                    b.HasIndex("RoleId");

                    b.HasIndex("Username").IsUnique();

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            Email = "director@company.com",
                            EmployeeId = 1,
                            IsActive = true,
                            Password = "$2a$11$/3VEyfoi2uk.gSuxwneHqOeoUVZcYptrjWQkMqQJ3hDimSvck.c4K",
                            RoleId = 1,
                            Username = "director"
                        },
                        new
                        {
                            Id = 2,
                            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            Email = "blockmanager@company.com",
                            EmployeeId = 2,
                            IsActive = true,
                            Password = "$2a$11$/3VEyfoi2uk.gSuxwneHqOeoUVZcYptrjWQkMqQJ3hDimSvck.c4K",
                            RoleId = 2,
                            Username = "blockmanager"
                        },
                        new
                        {
                            Id = 3,
                            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            Email = "centermanager@company.com",
                            EmployeeId = 3,
                            IsActive = true,
                            Password = "$2a$11$/3VEyfoi2uk.gSuxwneHqOeoUVZcYptrjWQkMqQJ3hDimSvck.c4K",
                            RoleId = 3,
                            Username = "centermanager"
                        },
                        new
                        {
                            Id = 4, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), Email = "employee@company.com", EmployeeId = 4, IsActive = true, Password = "$2a$11$/3VEyfoi2uk.gSuxwneHqOeoUVZcYptrjWQkMqQJ3hDimSvck.c4K", RoleId = 4, Username = "employee" });
                });

            modelBuilder.Entity("HRemployee.Entities.Attendance", b =>
                {
                    b.HasOne("HRemployee.Entities.Employee", "Employee").WithMany("Attendances").HasForeignKey("EmployeeId").OnDelete(DeleteBehavior.Cascade).IsRequired();

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("HRemployee.Entities.Contract", b =>
                {
                    b.HasOne("HRemployee.Entities.Employee", "Employee").WithMany("Contracts").HasForeignKey("EmployeeId").OnDelete(DeleteBehavior.Cascade).IsRequired();

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("HRemployee.Entities.Employee", b =>
                {
                    b.HasOne("HRemployee.Entities.Department", "Department").WithMany("Employees").HasForeignKey("DepartmentId").OnDelete(DeleteBehavior.Cascade).IsRequired();

                    b.HasOne("HRemployee.Entities.Employee", "Manager").WithMany("Subordinates").HasForeignKey("ManagerId").OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("HRemployee.Entities.Position", "Position").WithMany("Employees").HasForeignKey("PositionId").OnDelete(DeleteBehavior.Cascade).IsRequired();

                    b.Navigation("Department");

                    b.Navigation("Manager");

                    b.Navigation("Position");
                });

            modelBuilder.Entity("HRemployee.Entities.LeaveRequest", b =>
                {
                    b.HasOne("HRemployee.Entities.Employee", "ApprovedBy").WithMany("ApprovedLeaveRequests").HasForeignKey("ApprovedById").OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("HRemployee.Entities.Employee", "Employee").WithMany("LeaveRequests").HasForeignKey("EmployeeId").OnDelete(DeleteBehavior.Restrict).IsRequired();

                    b.HasOne("HRemployee.Entities.LeaveType", "LeaveType").WithMany("LeaveRequests").HasForeignKey("LeaveTypeId").OnDelete(DeleteBehavior.Cascade).IsRequired();

                    b.Navigation("ApprovedBy");

                    b.Navigation("Employee");

                    b.Navigation("LeaveType");
                });

            modelBuilder.Entity("HRemployee.Entities.RefreshToken", b =>
                {
                    b.HasOne("HRemployee.Entities.User", "User").WithMany("RefreshTokens").HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade).IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("HRemployee.Entities.SalaryConfig", b =>
                {
                    b.HasOne("HRemployee.Entities.Employee", "Employee").WithMany("SalaryConfigs").HasForeignKey("EmployeeId").OnDelete(DeleteBehavior.Cascade).IsRequired();

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("HRemployee.Entities.SalaryRecord", b =>
                {
                    b.HasOne("HRemployee.Entities.Employee", "Employee").WithMany("SalaryRecords").HasForeignKey("EmployeeId").OnDelete(DeleteBehavior.Cascade).IsRequired();

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("HRemployee.Entities.User", b =>
                {
                    b.HasOne("HRemployee.Entities.Employee", "Employee").WithOne("User").HasForeignKey("HRemployee.Entities.User", "EmployeeId").OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("HRemployee.Entities.Role", "Role").WithMany("Users").HasForeignKey("RoleId").OnDelete(DeleteBehavior.Cascade).IsRequired();

                    b.Navigation("Employee");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("HRemployee.Entities.Department", b =>
                {
                    b.Navigation("Employees");
                });

            modelBuilder.Entity("HRemployee.Entities.Employee", b =>
                {
                    b.Navigation("ApprovedLeaveRequests");

                    b.Navigation("Attendances");

                    b.Navigation("Contracts");

                    b.Navigation("LeaveRequests");

                    b.Navigation("SalaryConfigs");

                    b.Navigation("SalaryRecords");

                    b.Navigation("Subordinates");

                    b.Navigation("User").IsRequired();
                });

            modelBuilder.Entity("HRemployee.Entities.LeaveType", b =>
                {
                    b.Navigation("LeaveRequests");
                });

            modelBuilder.Entity("HRemployee.Entities.Position", b =>
                {
                    b.Navigation("Employees");
                });

            modelBuilder.Entity("HRemployee.Entities.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("HRemployee.Entities.User", b =>
                {
                    b.Navigation("RefreshTokens");
                });
#pragma warning restore 612, 618
        }
    }
}