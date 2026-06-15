using HRemployee.DataContext;
using HRemployee.Entities;
using HRemployee.Helper;
using HRemployee.Middlewares;
using HRemployee.PayLoad.Converter;
using HRemployee.PayLoad.DTO;
using HRemployee.PayLoad.Response;
using HRemployee.Service.Implement;
using HRemployee.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new OpenApiInfo { Title = "HR Management API", Version = "v1" });
    x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập: Bearer {token}", Name = "Authorization", In = ParameterLocation.Header, Type = SecuritySchemeType.ApiKey, Scheme = "Bearer" });
    x.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                Scheme = "oauth2", Name = "Bearer", In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, ValidateAudience = false, ValidateIssuer = false, IssuerSigningKey = new SymmetricSecurityKey( Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:SecretKey").Value!)) };
});

builder.Services.AddScoped<ResponseBase>();
builder.Services.AddScoped<ResponseObject<DTO_Login>>();
builder.Services.AddScoped<ResponseObject<DTO_Token>>();
builder.Services.AddScoped<ResponseObject<object>>();
builder.Services.AddScoped<ResponseObject<DTO_Department>>();
builder.Services.AddScoped<ResponseObject<List<DTO_Department>>>();
builder.Services.AddScoped<ResponseObject<PagedResult<DTO_Department>>>();
builder.Services.AddScoped<ResponseObject<DTO_Position>>();
builder.Services.AddScoped<ResponseObject<List<DTO_Position>>>();
builder.Services.AddScoped<ResponseObject<PagedResult<DTO_Position>>>();
builder.Services.AddScoped<ResponseObject<DTO_Employee>>();
builder.Services.AddScoped<ResponseObject<PagedResult<DTO_Employee>>>();
builder.Services.AddScoped<ResponseObject<DTO_Contract>>();
builder.Services.AddScoped<ResponseObject<List<DTO_Contract>>>();
builder.Services.AddScoped<ResponseObject<PagedResult<DTO_Contract>>>();
builder.Services.AddScoped<ResponseObject<DTO_Attendance>>();
builder.Services.AddScoped<ResponseObject<List<DTO_Attendance>>>();
builder.Services.AddScoped<ResponseObject<DTO_AttendanceSummary>>();

builder.Services.AddScoped<ResponseObject<DTO_LeaveRequest>>();
builder.Services.AddScoped<ResponseObject<List<DTO_LeaveRequest>>>();
builder.Services.AddScoped<ResponseObject<PagedResult<DTO_LeaveRequest>>>();

builder.Services.AddScoped<ResponseObject<DTO_LeaveType>>();
builder.Services.AddScoped<ResponseObject<List<DTO_LeaveType>>>();

builder.Services.AddScoped<ResponseObject<DTO_SalaryConfig>>();
builder.Services.AddScoped<ResponseObject<List<DTO_SalaryConfig>>>();
builder.Services.AddScoped<ResponseObject<PagedResult<DTO_SalaryConfig>>>();

builder.Services.AddScoped<ResponseObject<DTO_SalaryRecord>>();
builder.Services.AddScoped<ResponseObject<List<DTO_SalaryRecord>>>();
builder.Services.AddScoped<ResponseObject<PagedResult<DTO_SalaryRecord>>>();

builder.Services.AddScoped<ResponseObject<DTO_StatEmployee>>();
builder.Services.AddScoped<ResponseObject<DTO_StatAttendance>>();
builder.Services.AddScoped<ResponseObject<DTO_StatSalary>>();

builder.Services.AddScoped<CloudinaryService>();

builder.Services.AddScoped<Converter_Login>();
builder.Services.AddScoped<Converter_Department>();
builder.Services.AddScoped<Converter_Position>();
builder.Services.AddScoped<Converter_Employee>();
builder.Services.AddScoped<Converter_Contract>();
builder.Services.AddScoped<Converter_Attendance>();
builder.Services.AddScoped<Converter_LeaveRequest>();
builder.Services.AddScoped<Converter_SalaryConfig>();
builder.Services.AddScoped<Converter_SalaryRecord>();

builder.Services.AddScoped<IService_Authentic, Service_Authentic>();
builder.Services.AddScoped<IService_Department, Service_Department>();
builder.Services.AddScoped<IService_Position, Service_Position>();
builder.Services.AddScoped<IService_Employee, Service_Employee>();
builder.Services.AddScoped<IService_Contract, Service_Contract>();
builder.Services.AddScoped<IService_Attendance, Service_Attendance>();
builder.Services.AddScoped<IService_LeaveRequest, Service_LeaveRequest>();
builder.Services.AddScoped<IService_SalaryConfig, Service_SalaryConfig>();
builder.Services.AddScoped<IService_SalaryRecord, Service_SalaryRecord>();
builder.Services.AddScoped<IService_Statistics, Service_Statistics>();
builder.Services.AddScoped<IService_LeaveType, Service_LeaveType>();

builder.Services.AddControllers().AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new HRemployee.Helper.DateTimeConverter());
        opts.JsonSerializerOptions.Converters.Add(new HRemployee.Helper.NullableDateTimeConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Logging.AddConsole();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<TokenValidationMiddleware>();
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();