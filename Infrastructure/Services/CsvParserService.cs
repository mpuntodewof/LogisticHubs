using Application.Interfaces;

namespace Infrastructure.Services
{
    public class CsvParserService : ICsvParserService
    {
        public async Task<List<Dictionary<string, string>>> ParseAsync(Stream csvStream)
        {
            var rows = new List<Dictionary<string, string>>();
            using var reader = new StreamReader(csvStream);

            var headerLine = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(headerLine))
                return rows;

            var headers = ParseCsvLine(headerLine);

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var values = ParseCsvLine(line);
                var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                for (int i = 0; i < headers.Count && i < values.Count; i++)
                {
                    row[headers[i].Trim()] = values[i].Trim();
                }

                rows.Add(row);
            }

            return rows;
        }

        public List<string> GetHeaders(Stream csvStream)
        {
            using var reader = new StreamReader(csvStream, leaveOpen: true);
            var headerLine = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(headerLine))
                return new List<string>();

            csvStream.Position = 0;
            return ParseCsvLine(headerLine).Select(h => h.Trim()).ToList();
        }

        private static List<string> ParseCsvLine(string line)
        {
            var fields = new List<string>();
            bool inQuotes = false;
            var current = new System.Text.StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++; // Skip escaped quote
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    fields.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            fields.Add(current.ToString());
            return fields;
        }
    }
}
