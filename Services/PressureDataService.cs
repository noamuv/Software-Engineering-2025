using Software_Engineering_2025.Models;
using System;
using System.IO;
using System.Linq;

namespace Software_Engineering_2025.Services
{
    public class PressureDataService
    {
        // Parse a single CSV file into a 32x32 matrix
        public PressureMatrix ParseCsvFile(string filePath)
        {
            var matrix = new PressureMatrix();
            
            try
            {
                var lines = File.ReadAllLines(filePath);
                
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing CSV: {ex.Message}");
            }
            
            return matrix;
        }
        
        // Get the latest pressure data for a patient
        public PressureMatrix GetLatestPressureData(Guid patientId)
        {
        
            return GenerateSampleData();
        }
        
        // Generate sample data for testing
        private PressureMatrix GenerateSampleData()
        {
            var matrix = new PressureMatrix();
            var random = new Random();
            
            // Create a realistic pressure distribution
            // Higher pressure in the center (where patient sits)
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    // Distance from center
                    double distanceFromCenter = Math.Sqrt(Math.Pow(i - 16, 2) + Math.Pow(j - 16, 2));
                    
                    if (distanceFromCenter < 8)
                    {
                        // High pressure in center
                        matrix.Data[i, j] = random.Next(80, 120);
                    }
                    else if (distanceFromCenter < 12)
                    {
                        // Medium pressure
                        matrix.Data[i, j] = random.Next(40, 80);
                    }
                    else if (distanceFromCenter < 16)
                    {
                        // Low pressure
                        matrix.Data[i, j] = random.Next(10, 40);
                    }
                    else
                    {
                        // No contact
                        matrix.Data[i, j] = 1;
                    }
                }
            }
            
            return matrix;
        }
    }
}