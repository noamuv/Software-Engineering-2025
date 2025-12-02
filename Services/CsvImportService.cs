// This service handles importing pressure data from CSV files
using Software_Engineering_2025.Models;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Globalization;

namespace Software_Engineering_2025.Services
{
    public class CsvImportService
    {
        private readonly ApplicationDbContext _context;

        public CsvImportService(ApplicationDbContext context)
        {
            _context = context;
        }

        // For each patient all their CSV files in a folder
        public void ImportCsvFolder(string folderPath)
        {
            var csvFiles = Directory.GetFiles(folderPath, "*.csv");
            
            Console.WriteLine($"Found {csvFiles.Length} CSV files to import");

            foreach (var csvFile in csvFiles)
            {
                try
                {
                    ImportSingleCsv(csvFile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error importing {Path.GetFileName(csvFile)}: {ex.Message}");
                }
            }
        }

        // Import a single CSV file
        public void ImportSingleCsv(string filePath)
        {
            // Parse filename: "1c0fd777_20251011.csv"
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var parts = fileName.Split('_');
            
            if (parts.Length != 2)
            {
                Console.WriteLine($"Skipping invalid filename: {fileName}");
                return;
            }

            string patientUserId = parts[0];
            string dateString = parts[1];

            // Parse date (YYYYMMDD)
            if (!DateTime.TryParseExact(dateString, "yyyyMMdd", 
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime recordedDate))
            {
                Console.WriteLine($"Invalid date format in {fileName}");
                return;
            }

            // Check if already imported
            if (_context.PressureSessions.Any(p => 
                p.PatientUserId == patientUserId && 
                p.RecordedDate.Date == recordedDate.Date))
            {
                Console.WriteLine($"Already imported: {fileName}");
                return;
            }

            // Parse CSV into matrix
            var matrix = ParseCsvToMatrix(filePath);
            
            // Calculate metrics
            var metrics = matrix.CalculateMetrics();

            // Create session record
            var session = new PressureSession
            {
                Id = Guid.NewGuid(),
                PatientUserId = patientUserId,
                RecordedDate = recordedDate,
                MatrixJson = SerializeMatrix(matrix.Data),
                PeakPressure = metrics.PeakPressure,
                ContactAreaPercent = metrics.ContactAreaPercent,
                CoefficientOfVariation = metrics.CoefficientOfVariation,
                RiskScore = metrics.RiskScore,
                ImportedAt = DateTime.UtcNow
            };

            _context.PressureSessions.Add(session);
            _context.SaveChanges();

            Console.WriteLine($"✓ Imported {fileName}: Peak={metrics.PeakPressure}, Contact={metrics.ContactAreaPercent:F1}%, Risk={metrics.RiskScore}");
        }

        // This method reads the CSV and converts it to a PressureMatrix
        private PressureMatrix ParseCsvToMatrix(string filePath)
        {
            var matrix = new PressureMatrix();
            var lines = File.ReadAllLines(filePath);

            // This assumes CSV has at least 32 lines and 32 values per line
            for (int row = 0; row < Math.Min(32, lines.Length); row++)
            {
                var values = lines[row].Split(',');

                for (int col = 0; col < Math.Min(32, values.Length); col++)
                {
                    if (int.TryParse(values[col].Trim(), out int pressure))
                    {
                        matrix.Data[row, col] = pressure;
                    }
                }
            }

            return matrix;
        }

        // This method serializes the 32x32 matrix to JSON
        private string SerializeMatrix(int[,] matrix)
        {
            // Converts 2D array to jagged array for JSON serialization
            int[][] jaggedArray = new int[32][];
            for (int i = 0; i < 32; i++)
            {
                jaggedArray[i] = new int[32];
                for (int j = 0; j < 32; j++)
                {
                    jaggedArray[i][j] = matrix[i, j];
                }
            }

            return JsonSerializer.Serialize(jaggedArray);
        }

        // This Deserializes JSON to matrix
        public int[,] DeserializeMatrix(string json)
        {
            var jaggedArray = JsonSerializer.Deserialize<int[][]>(json);
            int[,] matrix = new int[32, 32];

            if (jaggedArray != null)
            {
                for (int i = 0; i < 32; i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        matrix[i, j] = jaggedArray[i][j];
                    }
                }
            }

            return matrix;
        }
    }
}