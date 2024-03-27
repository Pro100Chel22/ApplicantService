using UserService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

app.Services.AddAutoMigration();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// 1. ������� IBus
// 2. ������� ����� ��������� IServiceBus � �������� ��� �������� ���������
// 3. �������� ���������� � infrastructure
// 4. ������� ������ common � ������ �������� ��� �������
// 5. �������� ������������� ���������� � AuthService