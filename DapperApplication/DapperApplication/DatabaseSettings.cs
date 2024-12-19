namespace DapperApplication;

public record DatabaseSettings
{
    public string ConnectionString { get; set; } =
        "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=postgres;";
}