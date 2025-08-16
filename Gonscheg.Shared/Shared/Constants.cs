namespace Gonscheg.Shared.Shared;

public static class Constants
{
    public static readonly string ConnectionString =
        $"Host={EnvironmentVariables.DBHost};Database=Gonscheg;Username={EnvironmentVariables.DBUser};Password={EnvironmentVariables.DBPass};SSL Mode=Disable;";
}