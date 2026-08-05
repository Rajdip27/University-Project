namespace UniversityProject.Application.Extensions;

public static class BarcodeGenerator
{
    public static string Generate()
    {
        return $"PRD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
    }
}
