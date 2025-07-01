using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Plane_Crash_Visualization.Data;
using Plane_Crash_Visualization.Models;
using System.Globalization;

namespace Plane_Crash_Visualization.Data
{
    public class CsvImporter
    {
        private readonly CrashDbContext _context;
        public CsvImporter(CrashDbContext context)
        {
            _context = context;
        }

        public void ImportCrashesFromCsv(string csvFilePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HeaderValidated = null,
                MissingFieldFound = null
            };
            using var reader = new StreamReader(csvFilePath);
            using var csv = new CsvReader(reader, config);
            csv.Context.RegisterClassMap<CrashMap>();
            var records = csv.GetRecords<Crash>().ToList();
            
            // Only add records that don't already exist
            var newRecords = new List<Crash>();
            foreach (var record in records)
            {
                // Check if a crash with the same date, location, and operator already exists
                bool exists = _context.Crashes.Any(c => 
                    c.Date == record.Date && 
                    c.Location == record.Location && 
                    c.Operator == record.Operator &&
                    c.Flight == record.Flight);
                
                if (!exists)
                {
                    newRecords.Add(record);
                }
            }
            
            if (newRecords.Any())
            {
                _context.Crashes.AddRange(newRecords);
                _context.SaveChanges();
                Console.WriteLine($"Imported {newRecords.Count} new crash records.");
            }
            else
            {
                Console.WriteLine("No new crash records to import.");
            }
        }
    }

    public class LeadingIntConverter : CsvHelper.TypeConversion.ITypeConverter
    {
        public object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Trim() == "?") return null;
            var match = System.Text.RegularExpressions.Regex.Match(text, @"^\d+");
            if (match.Success && int.TryParse(match.Value, out int value))
                return value;
            return null;
        }
        public string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
        {
            return value?.ToString();
        }
    }

    public class StringNullConverter : CsvHelper.TypeConversion.ITypeConverter
    {
        public object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Trim() == "?") return null;
            return text.Trim();
        }
        public string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
        {
            return value?.ToString();
        }
    }

    public class DetailedCountConverter : CsvHelper.TypeConversion.ITypeConverter
    {
        private readonly string _targetType; // "total", "passengers", or "crew"

        public DetailedCountConverter(string targetType)
        {
            _targetType = targetType;
        }

        public object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Trim() == "?") return null;
            
            var input = text.Trim();
            
            // Extract total number (before parentheses)
            var totalMatch = System.Text.RegularExpressions.Regex.Match(input, @"^(\d+)");
            
            if (_targetType == "total")
            {
                if (totalMatch.Success && int.TryParse(totalMatch.Groups[1].Value, out int total))
                    return total;
                return null;
            }
            
            // Extract passengers and crew from parentheses
            var detailMatch = System.Text.RegularExpressions.Regex.Match(input, @"passengers:(\d+|\?)\s+crew:(\d+|\?)");
            
            if (detailMatch.Success)
            {
                if (_targetType == "passengers")
                {
                    var passengersStr = detailMatch.Groups[1].Value;
                    if (passengersStr != "?" && int.TryParse(passengersStr, out int passengers))
                        return passengers;
                }
                else if (_targetType == "crew")
                {
                    var crewStr = detailMatch.Groups[2].Value;
                    if (crewStr != "?" && int.TryParse(crewStr, out int crew))
                        return crew;
                }
            }
            
            return null;
        }

        public string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
        {
            return value?.ToString();
        }
    }

    public class DateTimeNullConverter : CsvHelper.TypeConversion.ITypeConverter
    {
        public object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Trim() == "?") return null;
            
            if (DateTime.TryParse(text.Trim(), out DateTime result))
                return result;
            
            return null;
        }
        public string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
        {
            return value?.ToString();
        }
    }

    public sealed class CrashMap : ClassMap<Crash>
    {
        public CrashMap()
        {
            Map(m => m.Date).Name("Date").TypeConverter<DateTimeNullConverter>();
            Map(m => m.Time).Name("Time").TypeConverter<LeadingIntConverter>();
            Map(m => m.Location).Name("Location").TypeConverter<StringNullConverter>();
            Map(m => m.Operator).Name("Operator").TypeConverter<StringNullConverter>();
            Map(m => m.Flight).Name("Flight").TypeConverter<StringNullConverter>();
            Map(m => m.Route).Name("Route").TypeConverter<StringNullConverter>();
            Map(m => m.AC_Type).Name("AC Type").TypeConverter<StringNullConverter>();
            Map(m => m.Registration).Name("Registration").TypeConverter<StringNullConverter>();
            Map(m => m.Cn_ln).Name("cn/ln").TypeConverter<StringNullConverter>();
            Map(m => m.Aboard).Name("Aboard").TypeConverter(new DetailedCountConverter("total"));
            Map(m => m.AboardPassengers).Name("Aboard").TypeConverter(new DetailedCountConverter("passengers"));
            Map(m => m.AboardCrew).Name("Aboard").TypeConverter(new DetailedCountConverter("crew"));
            Map(m => m.Fatalities).Name("Fatalities").TypeConverter(new DetailedCountConverter("total"));
            Map(m => m.FatalitiesPassengers).Name("Fatalities").TypeConverter(new DetailedCountConverter("passengers"));
            Map(m => m.FatalitiesCrew).Name("Fatalities").TypeConverter(new DetailedCountConverter("crew"));
            Map(m => m.Ground).Name("Ground").TypeConverter<LeadingIntConverter>();
            Map(m => m.Summary).Name("Summary").TypeConverter<StringNullConverter>();
        }
    }
}
