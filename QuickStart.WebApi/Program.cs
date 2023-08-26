using Hangfire.Dashboard;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Hangfire
builder.Services.AddHangfire(builder.Configuration.GetConnectionString("HangfireConnection")!);
builder.Services.AddBackgroundJobService();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseHangfireDashboard("/jobs", options: new DashboardOptions
{
    IsReadOnlyFunc = (DashboardContext context) => true, //设置Dashboard为Readonly，不能通过Dashboard触发任务
    AppPath = "/swagger/index.html" //设置Back To Site的URL
});

app.RegisterJobs();

app.Run();
