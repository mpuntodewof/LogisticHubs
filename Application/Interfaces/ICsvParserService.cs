namespace Application.Interfaces
{
    public interface ICsvParserService
    {
        Task<List<Dictionary<string, string>>> ParseAsync(Stream csvStream);
        List<string> GetHeaders(Stream csvStream);
    }
}
