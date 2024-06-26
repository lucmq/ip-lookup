namespace IpLookup.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.RegisterServices();

        var app = builder.Build();
        app.RegisterMiddlewares();
        app.RegisterEndpoints();

        app.Run();
    }
}