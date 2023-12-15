using BufferApi.Buffer;
using BufferApi.DB;
using BufferApi.Jobs;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//string rknetConnectingString = builder.Configuration.GetConnectionString("rknet");
//builder.Services.AddDbContext<RknetContext>(options => options.UseSqlServer(rknetConnectingString));
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<CalculatorLogRepository>()
                .AddSingleton<IJobFactory, SingletonJobFactory>()
                .AddSingleton<ISchedulerFactory, StdSchedulerFactory>()
                .AddSingleton<BufferJob>()
                .AddSingleton(new JobSchedule(
                    jobType: typeof(BufferJob),
                    cronExpression: "0 */10 * ? * *"));
builder.Services.AddHostedService<QuartzHostedService>();
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    // Дополнительный endpoint
    endpoints.MapControllerRoute(
        name: "customEndpoint",
        pattern: "custom/{controller=Home}/{action=Index}/{id?}");
});


app.Run();
