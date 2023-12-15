using BufferApi.Buffer;
using BufferApi.DB;
using BufferApi.Jobs;
using BufferApi.Models;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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


app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // Дополнительные маршруты
    endpoints.MapControllerRoute(
        name: "customEndpoint",
        pattern: "custom/{controller=Home}/{action=Index}/{id?}");
});


app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();

app.MapControllers();




app.Run();
