using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using TheExpertise_Emp.Data;
using TheExpertise_Emp.Entities;
using TheExpertise_Emp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger for API documentation (optional but useful for testing)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<EmailService>();

// Add DbContext for SQL Server connection
builder.Services.AddDbContext<EmployeeDocumentDBSContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EmployeeDocumentDBSConnection"))
    
);
builder.Services.AddDbContext<EmployeeDocumentDbsContextNew>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EmployeeDocumentDBSConnection"))
);

// Add CORS configuration to allow React app to make requests
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // ระบุ URL ของ React frontend
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // อนุญาตให้ส่งคำขอพร้อม credentials เช่น cookies
    });
});

// Add Session support
builder.Services.AddDistributedMemoryCache(); // ใช้ Memory Cache สำหรับ Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // ตั้งเวลา Session หมดอายุ
    options.Cookie.HttpOnly = true; // ป้องกันการเข้าถึง Session ผ่าน JavaScript
    options.Cookie.IsEssential = true; // ใช้แม้ไม่มีการอนุญาต cookies
});

var app = builder.Build();

// Enable Swagger for API documentation (optional but useful for testing)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

// Use CORS to allow cross-origin requests from the React app
app.UseCors("AllowReactApp");

// Enable serving static files from wwwroot and uploads folder
app.UseStaticFiles(); // ให้บริการไฟล์จาก wwwroot (default)

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads")), // ที่อยู่โฟลเดอร์ uploads
    RequestPath = "/uploads" // พาธที่ใช้ในการเรียกไฟล์
});

// Enable session middleware
app.UseSession();

// Add middleware for HTTPS redirection and authorization
app.UseHttpsRedirection();
app.UseAuthorization();

// Map controllers
app.MapControllers();

app.Run();
