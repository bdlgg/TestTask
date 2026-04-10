using System;
using System.Globalization;
using System.IO;

namespace TestTask;

public static class LogProcessor
{
    public static void ProcessFile(string inputPath, string outputPath, string problemsPath)
    {
        using StreamWriter outputWriter = new StreamWriter(outputPath);
        using StreamWriter problemsWriter = new StreamWriter(problemsPath);

        foreach (string? rawLine in File.ReadLines(inputPath))
        {
            string line = rawLine ?? string.Empty;
            if (TryParseLine(line, out LogEntry? entry))
            {
                outputWriter.WriteLine(FormatOutput(entry));
            }
            else
            {
                problemsWriter.WriteLine(line);
            }
        }
    }

    public static bool TryParseLine(string line, out LogEntry? entry)
    {
        entry = null;
        if (string.IsNullOrWhiteSpace(line)) 
            return false;

        if (line.Contains('|'))
            return TryParseFormat2(line, out entry);
        return TryParseFormat1(line, out entry);
    }

    private static bool TryParseFormat1(string line, out LogEntry? entry)
    {
        entry = null;
        string[] parts = line.Split(' ', 4, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 4)
            return false;
        string datePart = parts[0];
        string timePart = parts[1];
        string levelPart = parts[2];
        string messagePart = parts[3];
        if (!DateTime.TryParseExact(
                datePart,
                "dd.MM.yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime parsedDate))
        {
            return false;
        }

        string? normalizedLevel = NormalizeLevel(levelPart);
        if (normalizedLevel is null)
            return false;
        entry = new LogEntry
        {
            Date = parsedDate.ToString("dd-MM-yyyy"),
            Time = timePart,
            Level = normalizedLevel,
            Method = "DEFAULT",
            Message = messagePart
        };
        return true;
    }

    private static bool TryParseFormat2(string line, out LogEntry? entry)
    {
        entry = null;
        string[] parts = line.Split('|');
        if (parts.Length < 5)
            return false;
        string dateTimePart = parts[0].Trim();
        string levelPart = parts[1].Trim();
        string methodPart = parts[3].Trim();
        string messagePart = parts[4].Trim();

        string[] dateTimeTokens = dateTimePart.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        if (dateTimeTokens.Length != 2)
            return false;
        string datePart = dateTimeTokens[0];
        string timePart = dateTimeTokens[1];

        if (!DateTime.TryParseExact(
                datePart,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime parsedDate))
        {
            return false;
        }

        string? normalizedLevel = NormalizeLevel(levelPart);
        if (normalizedLevel is null)
            return false;
        if (string.IsNullOrWhiteSpace(methodPart))
            methodPart = "DEFAULT";
        entry = new LogEntry
        {
            Date = parsedDate.ToString("dd-MM-yyyy"),
            Time = timePart,
            Level = normalizedLevel,
            Method = methodPart,
            Message = messagePart
        };
        return true;
    }

    private static string? NormalizeLevel(string level)
    {
        return level.ToUpperInvariant() switch
        {
            "INFORMATION" => "INFO",
            "INFO" => "INFO",
            "WARNING" => "WARN",
            "WARN" => "WARN",
            "ERROR" => "ERROR",
            "DEBUG" => "DEBUG",
            _ => null
        };
    }

    private static string FormatOutput(LogEntry entry)
    {
        return $"{entry.Date}\t{entry.Time}\t{entry.Level}\t{entry.Method}\t{entry.Message}";
    }
}