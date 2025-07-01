using System.Text.RegularExpressions;

namespace Plane_Crash_Visualization.Services
{
    public class AircraftTypeService
    {
        private readonly Dictionary<string, (string Manufacturer, string Model)> _specialCases;
        private readonly List<(Regex Pattern, string ManufacturerGroup, string ModelGroup)> _manufacturerPatterns;

        public AircraftTypeService()
        {
            _specialCases = InitializeSpecialCases();
            _manufacturerPatterns = InitializePatterns();
        }

        public (string? Manufacturer, string? Model) ExtractManufacturerAndModel(string? acType)
        {
            if (string.IsNullOrWhiteSpace(acType) || acType == "?")
                return (null, null);

            // Handle multiple aircraft (midair collisions) - take the first one
            if (acType.Contains(" / "))
            {
                acType = acType.Split(" / ")[0].Trim();
            }

            acType = acType.Trim();

            // Check special cases first
            if (_specialCases.ContainsKey(acType))
            {
                var specialCase = _specialCases[acType];
                return (specialCase.Manufacturer, specialCase.Model);
            }

            // Handle Zeppelin variants with parentheses
            if (acType.Contains("Zeppelin") && acType.Contains("(airship)"))
            {
                var zeppelinMatch = Regex.Match(acType, @"(.*Zeppelin.*?)\s*\(airship\)", RegexOptions.IgnoreCase);
                if (zeppelinMatch.Success)
                {
                    return ("Zeppelin", zeppelinMatch.Groups[1].Value);
                }
            }

            // Handle Schutte-Lanz airships
            if (acType.Contains("Schutte-Lanz"))
            {
                var slMatch = Regex.Match(acType, @"(Schutte-Lanz)\s+(S-L-\d+).*", RegexOptions.IgnoreCase);
                if (slMatch.Success)
                {
                    return (slMatch.Groups[1].Value, slMatch.Groups[2].Value);
                }
            }

            // Try manufacturer patterns
            foreach (var (pattern, manufacturerGroup, modelGroup) in _manufacturerPatterns)
            {
                var match = pattern.Match(acType);
                if (match.Success)
                {
                    var manufacturer = match.Groups[1].Value;
                    var model = match.Groups.Count > 3 && !string.IsNullOrEmpty(match.Groups[3].Value) 
                        ? match.Groups[3].Value 
                        : match.Groups[2].Value;
                    
                    return (manufacturer, model);
                }
            }

            // Fallback: split on first space if no pattern matches
            var parts = acType.Split(' ', 2);
            if (parts.Length >= 2)
            {
                return (parts[0], parts[1]);
            }

            return ("Unknown", acType);
        }

        private Dictionary<string, (string, string)> InitializeSpecialCases()
        {
            return new Dictionary<string, (string, string)>
            {
                {"Dirigible", ("Unknown", "Dirigible")},
                {"Airship", ("Unknown", "Airship")},
                {"Super Zeppelin (airship)", ("Zeppelin", "Super Zeppelin")},
                {"FD Type Dirigible", ("Unknown", "FD Type Dirigible")},
            };
        }

        private List<(Regex, string, string)> InitializePatterns()
        {
            return new List<(Regex, string, string)>
            {
                // Major manufacturers with specific patterns
                (new Regex(@"^(Boeing)\s+(B-)?(\d{3})(-\d+)?([A-Z]*).*", RegexOptions.IgnoreCase), "1", "3"), // Boeing 747-300 -> Boeing, 747
                (new Regex(@"^(Airbus)\s+(A\d{3}[A-Z]*)-?.*", RegexOptions.IgnoreCase), "1", "2"), // Airbus A320-111 -> Airbus, A320
                (new Regex(@"^(McDonnell Douglas)\s+(DC-\d+)(-\d+)?.*", RegexOptions.IgnoreCase), "1", "2"), // McDonnell Douglas DC-8-21 -> McDonnell Douglas, DC-8
                (new Regex(@"^(Lockheed)\s+(L-\d+[A-Z]*).*", RegexOptions.IgnoreCase), "1", "2"), // Lockheed L-749 -> Lockheed, L-749
                (new Regex(@"^(Douglas)\s+(DC-\d+)(-[A-Z0-9]+)?.*", RegexOptions.IgnoreCase), "1", "2"), // Douglas DC-3 -> Douglas, DC-3
                (new Regex(@"^(Cessna)\s+(\d+[A-Z]?).*", RegexOptions.IgnoreCase), "1", "2"), // Cessna 150 -> Cessna, 150

                // Generic manufacturer patterns
                (new Regex(@"^(De Havilland)\s+(DH[.-]?\d+[A-Z]?).*", RegexOptions.IgnoreCase), "1", "2"), // De Havilland DH-4 -> De Havilland, DH-4
                (new Regex(@"^(Curtiss)\s+([A-Z0-9-]+).*", RegexOptions.IgnoreCase), "1", "2"), // Curtiss R-4LM -> Curtiss, R-4LM
                (new Regex(@"^(Wright)\s+(.+)", RegexOptions.IgnoreCase), "1", "2"), // Wright Flyer III -> Wright, Flyer III
                (new Regex(@"^(Zeppelin)\s+(L-\d+).*", RegexOptions.IgnoreCase), "1", "2"), // Zeppelin L-1 -> Zeppelin, L-1
                (new Regex(@"^(Junkers)\s+([A-Z0-9-]+).*", RegexOptions.IgnoreCase), "1", "2"), // Junkers JL-6 -> Junkers, JL-6
                (new Regex(@"^(Caproni)\s+(.+)", RegexOptions.IgnoreCase), "1", "2"), // Caproni Ca.48 -> Caproni, Ca.48
                (new Regex(@"^(Farman)\s+(.+)", RegexOptions.IgnoreCase), "1", "2"), // Farman F-40 -> Farman, F-40
                (new Regex(@"^(Breguet)\s+(.+)", RegexOptions.IgnoreCase), "1", "2"), // Breguet 14 -> Breguet, 14
                (new Regex(@"^(Handley Page)\s+(.+)", RegexOptions.IgnoreCase), "1", "2"), // Handley Page HP-16 -> Handley Page, HP-16
                (new Regex(@"^(Armstrong-Whitworth)\s+(.+)", RegexOptions.IgnoreCase), "1", "2"), // Armstrong-Whitworth F-K-8 -> Armstrong-Whitworth, F-K-8
                (new Regex(@"^(Salmson)\s+(.+)", RegexOptions.IgnoreCase), "1", "2"), // Salmson 2-A-2 -> Salmson, 2-A-2
                (new Regex(@"^(Ford)\s+(.+)", RegexOptions.IgnoreCase), "1", "2"), // Ford 5-AT-B Tri-Motor -> Ford, 5-AT-B Tri-Motor
                (new Regex(@"^(Piper)\s+(.+)", RegexOptions.IgnoreCase), "1", "2"), // Piper aircraft
                (new Regex(@"^(Beechcraft)\s+(.+)", RegexOptions.IgnoreCase), "1", "2"), // Beechcraft aircraft
                (new Regex(@"^(Fokker)\s+(.+)", RegexOptions.IgnoreCase), "1", "2"), // Fokker aircraft
                (new Regex(@"^(Embraer)\s+(.+)", RegexOptions.IgnoreCase), "1", "2"), // Embraer aircraft
                (new Regex(@"^(ATR)\s+(.+)", RegexOptions.IgnoreCase), "1", "2"), // ATR aircraft
                (new Regex(@"^(Antonov)\s+(.+)", RegexOptions.IgnoreCase), "1", "2"), // Antonov aircraft
                (new Regex(@"^(Tupolev)\s+(.+)", RegexOptions.IgnoreCase), "1", "2"), // Tupolev aircraft
                (new Regex(@"^(Ilyushin)\s+(.+)", RegexOptions.IgnoreCase), "1", "2"), // Ilyushin aircraft
                (new Regex(@"^(Yakovlev)\s+(.+)", RegexOptions.IgnoreCase), "1", "2"), // Yakovlev aircraft
                (new Regex(@"^(Sukhoi)\s+(.+)", RegexOptions.IgnoreCase), "1", "2"), // Sukhoi aircraft
            };
        }

        public Dictionary<string, int> AnalyzeManufacturers(IEnumerable<string> aircraftTypes)
        {
            var manufacturerCounts = new Dictionary<string, int>();

            foreach (var acType in aircraftTypes.Where(t => !string.IsNullOrWhiteSpace(t)))
            {
                var (manufacturer, _) = ExtractManufacturerAndModel(acType);
                if (!string.IsNullOrWhiteSpace(manufacturer))
                {
                    manufacturerCounts[manufacturer] = manufacturerCounts.GetValueOrDefault(manufacturer, 0) + 1;
                }
            }

            return manufacturerCounts.OrderByDescending(kvp => kvp.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public void TestExtractionLogic()
        {
            var testCases = new[]
            {
                "Boeing B-747-300",
                "Boeing 707-123",
                "Boeing 40",
                "Airbus A320-111",
                "McDonnell Douglas DC-8-21",
                "De Havilland DH-4",
                "Cessna 150",
                "Wright Flyer III",
                "Zeppelin L-1 (airship)",
                "Lockheed L-749 / Cessna 140"
            };

            Console.WriteLine("=== Testing Aircraft Type Extraction Logic ===");
            foreach (var testCase in testCases)
            {
                var (manufacturer, model) = ExtractManufacturerAndModel(testCase);
                Console.WriteLine($"'{testCase}' -> Manufacturer: '{manufacturer}', Model: '{model}'");
            }
        }
    }
}
