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
    IsReadOnlyFunc = (DashboardContext context) => true, //����DashboardΪReadonly������ͨ��Dashboard��������
    AppPath = "/swagger/index.html" //����Back To Site��URL
});

app.RegisterJobs();

app.Run();
