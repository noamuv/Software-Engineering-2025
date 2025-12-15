// This service handles importing pressure data from CSV files
using Software_Engineering_2025.Models;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Globalization;

namespace Software_Engineering_2025.Services
{
    // Service for importing CSV files containing pressure data
    public class CsvImportService
    {
        // Database context
        /* This service requires ApplicationDbContext to interact with the database 
        because it needs to check for existing records and save new pressure session data.*/

        private readonly ApplicationDbContext _context;

        public CsvImportService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Import all CSV files from a folder

        public void ImportCsvFolder(string folderPath)
        {
            // Get all CSV files in the specified folder
            var csvFiles = Directory.GetFiles(folderPath, "*.csv");
            
            Console.WriteLine($"Found {csvFiles.Length} CSV files to import");

            // Process each file
            foreach (var csvFile in csvFiles)
            {
                // Import single CSV file
                try
                {
                    ImportSingleCsv(csvFile);
                }
                // Catch any exceptions to prevent one bad file from stopping the whole import
                catch (Exception ex)
                {
                    Console.WriteLine($"Error importing {Path.GetFileName(csvFile)}: {ex.Message}");
                }
            }
        }

        // Import a single CSV file
        public void ImportSingleCsv(string filePath)
        {
            // Extract patient ID and date from filename
            // Expected format: PatientUserId_YYYYMMDD.csv
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var parts = fileName.Split('_');
            
            // Validate filename format
            if (parts.Length != 2)
            {
                Console.WriteLine($"Skipping invalid filename: {fileName}");
                return;
            }
            
            // Extract patient ID and date
            // Example: "patient123_20231015" -> patientUserId = "patient123", dateString = "20231015"
            string patientUserId = parts[0];
            string dateString = parts[1];

            // Parse date (YYYYMMDD)
            if (!DateTime.TryParseExact(dateString, "yyyyMMdd", 
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime recordedDate))
            {
                Console.WriteLine($"Invalid date format in {fileName}");
                return;
            }

            /* Check if already imported
            If a session for this patient and date already exists, skip import
             This prevents duplicate entries
             */
            if (_context.PressureSessions.Any(p => 
                p.PatientUserId == patientUserId && 
                p.RecordedDate.Date == recordedDate.Date))
            {
                Console.WriteLine($"Already imported: {fileName}");
                return;
            }

            // Parse CSV into matrix
            var matrix = ParseCsvToMatrix(filePath);
            /* The for loop iterates through each element of the 32x32 matrix
             to determine the maximum pressure value present in the data.*/
            int maxValueInMatrix = 0;
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    if (matrix.Data[i, j] > maxValueInMatrix)
                    {
                        maxValueInMatrix = matrix.Data[i, j];
                    }
                }
            }
            
            // Using the matrix, metrics are calculated by calling the CalculateMetrics method
            var metrics = matrix.CalculateMetrics();

            // Debug output
            Console.WriteLine($"{fileName}: Calculated peak pressure = {metrics.PeakPressure}");

            // Create session record
            var session = new PressureSession
            {
                Id = Guid.NewGuid(),      // Unique identifier
                PatientUserId = patientUserId,  // Patient ID from filename
                RecordedDate = recordedDate,     // Date from filename
                MatrixJson = SerializeMatrix(matrix.Data),   // Serialize matrix to JSON
                PeakPressure = metrics.PeakPressure,       // Calculated peak pressure
                ContactAreaPercent = metrics.ContactAreaPercent,   // Calculated contact area percentage
                CoefficientOfVariation = metrics.CoefficientOfVariation,  // Calculated coefficient of variation
                RiskScore = metrics.RiskScore,        // Calculated risk score
                ImportedAt = DateTime.UtcNow       // Timestamp of import
            };
            // Save to database
            _context.PressureSessions.Add(session);
            _context.SaveChanges();

            Console.WriteLine($"✓ Imported {fileName}: Peak={metrics.PeakPressure}, Contact={metrics.ContactAreaPercent:F1}%, Risk={metrics.RiskScore}");
        }

        // This method reads the CSV and converts it to a PressureMatrix
        // It also handles capping values above 255
        // It is converted  to a matrix becuase the PressureMatrix class contains useful methods for analysis such as CalculateMetrics
        // a matrix representation is more suitable for these operations than a flat JSON string
        private PressureMatrix ParseCsvToMatrix(string filePath)
        {
            /* Initialize a new PressureMatrix
             Read all lines from the CSV file */
            var matrix = new PressureMatrix();
            var lines = File.ReadAllLines(filePath);

            int invalidValues = 0;
            // This assumes CSV has at least 32 lines and 32 values per line
            for (int row = 0; row < Math.Min(32, lines.Length); row++)
            {
                // Assumes comma as delimiter
                var values = lines[row].Split(',');

                /* Uses only the first 32 values in each line
                 Parses each value and populates the matrix */
                for (int col = 0; col < Math.Min(32, values.Length); col++)
                {
                    if (int.TryParse(values[col].Trim(), out int pressure))
                    {
                        //Cap pressure values to a max of 255
                        if (pressure > 255)
                        {
                            invalidValues++;
                            pressure = 255;
                        }
                        matrix.Data[row, col] = pressure;
                    }
                }
            }

            if (invalidValues > 0)
            {
                Console.WriteLine($"Warning: {invalidValues} pressure values exceeded 255 and were capped.");
            }

            return matrix;
        }

        // This method serializes the 32x32 matrix to JSON
        // Why serialize? Storing as JSON simplifies database storage and retrieval
        private string SerializeMatrix(int[,] matrix)
        {
            // Converts 2D array to jagged array for JSON serialization
            int[][] jaggedArray = new int[32][];
            for (int i = 0; i < 32; i++)
            {
                /* Initialize each row of the jagged array
                 Copy values from the 2D array to the jagged array */
                jaggedArray[i] = new int[32];
                for (int j = 0; j < 32; j++)
                {
                    jaggedArray[i][j] = matrix[i, j];
                }
            }

            return JsonSerializer.Serialize(jaggedArray);
        }

        /*This Deserializes JSON to matrix because it may be needed elsewhere
         why deserialize after serialize? To enable further analysis 
         or processing of the pressure data after retrieval from the database */
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