// This model defines the structure of pressure data readings
using System;
using System.Collections.Generic;
using System.Linq;

namespace Software_Engineering_2025.Models
{

    // Represents ONE pressure reading session from the sensor mat
    /// Each CSV file becomes ONE PressureSession record
    
    public class PressureSession
    {
        // Unique ID for this specific pressure reading
       
        public Guid Id { get; set; }

        // Links to the CSV filename prefix
        // Example: "1c0fd777" (from file: 1c0fd777_20251011.csv)
       
        public string PatientUserId { get; set; } = string.Empty;
        
        // Parsed from CSV filename: "20251011" → October 11, 2025
      
        public DateTime RecordedDate { get; set; }
        
    
        // The actual 32x32 pressure matrix stored as JSON because
        // JSON works well with EF Core and databases
       
        public string MatrixJson { get; set; } = string.Empty;
        
      
        // PRE-CALCULATED METRICS (Saves time!)
      
   
        // Highest pressure in regions with 10+ pixels as specified by Sensore
        public int PeakPressure { get; set; }
    
        // Measures how much of the mat has pressure (0-100%)
        public decimal ContactAreaPercent { get; set; }
        
        // Measures pressure distribution (StdDev / Mean × 100)
        public decimal CoefficientOfVariation { get; set; }
      
        // Calculated from peak pressure and contact area
        public int RiskScore { get; set; }
        
        // Timestamp when this data was imported into the system
        public DateTime ImportedAt { get; set; } = DateTime.UtcNow;
    }
    

    
    // This class contains methods to analyze the 32x32 pressure data
    public class PressureMatrix
    {
        //This holds the pressure values as a 32x32 integer array
        public int[,] Data { get; set; } = new int[32, 32];
       
        public PressureMetrics CalculateMetrics()
        {
            var metrics = new PressureMetrics();
            
            // Each method calculates one of the four key metrics
            metrics.PeakPressure = CalculatePeakPressureIndex();
            metrics.ContactAreaPercent = CalculateContactAreaPercent();
            metrics.CoefficientOfVariation = CalculateCoefficientOfVariation();
            metrics.RiskScore = CalculateRiskScore(metrics.PeakPressure, metrics.ContactAreaPercent);
            
            return metrics;
        }
        
     
        // This method calculates the Peak Pressure Index
        private int CalculatePeakPressureIndex()
        {
            // This finds all pressure regions
            var regions = FindPressureRegions();
            
            // This keeps only regions with 10+ pixels
            var validRegions = regions.Where(r => r.PixelCount >= 10).ToList();
            
            // If there are no valid regions, it returns 0
            if (validRegions.Count == 0)
            {
                return 0;
            }
            
            // This returns the highest pressure from valid regions
            return validRegions.Max(r => r.MaxPressure);
        }
        
        // This method finds all contiguous pressure regions in the matrix
        private List<PressureRegion> FindPressureRegions()
        {
            // Track which pixels we've already visited
            bool[,] visited = new bool[32, 32];
            
            // List to store all regions found
            var regions = new List<PressureRegion>();
            
            // This is the Threshold: pixels above this value are "pressure"
            int pressureThreshold = 10;
            
            // Loop through entire matrix
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    // Found an unvisited pixel with pressure?
                    if (!visited[i, j] && Data[i, j] > pressureThreshold)
                    {
                        // Flood-fill is an algorithm to find all connected pixels
                        var region = FloodFillRegion(i, j, visited, pressureThreshold);
                        regions.Add(region);
                    }
                }
            }
            
            return regions;
        }
        
        // Flood-fill algorithm to explore a pressure region
        private PressureRegion FloodFillRegion(int startRow, int startCol, bool[,] visited, int threshold)
        {
            var region = new PressureRegion();
            
            // stack is used for depth-first search
            // Each item is a tuple of (row, col)
            var stack = new Stack<(int row, int col)>();
            stack.Push((startRow, startCol));

            while (stack.Count > 0)
            {
                var (row, col) = stack.Pop();
                
                // This is used to check bounds
                if (row < 0 || row >= 32 || col < 0 || col >= 32)
                    continue;
                
                // This will skip if already visited or below threshold
                if (visited[row, col] || Data[row, col] <= threshold)
                    continue;
                
                // Mark as visited
                visited[row, col] = true;
                
                // This will Increment the pixel count for this region
                region.PixelCount++;
                
                // Updates the max pressure if the pixel is higher
                if (Data[row, col] > region.MaxPressure)
                {
                    region.MaxPressure = Data[row, col];
                }
                
                // This adds neighboring pixels to the stack
                stack.Push((row - 1, col)); // Up
                stack.Push((row + 1, col)); // Down
                stack.Push((row, col - 1)); // Left
                stack.Push((row, col + 1)); // Right
            }
            
            return region;
        }
        
        // This method calculates the Contact Area Percentage
        private decimal CalculateContactAreaPercent()
        {
            int pixelsWithContact = 0;
            
            // Value above which we consider "contact"
            int lowerThreshold = 0;
            
            // Counts pixels with pressure
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    if (Data[i, j] > lowerThreshold)
                    {
                        pixelsWithContact++;
                    }
                }
            }
            
            // Calculates percentage of contact area
            decimal contactPercent = (decimal)pixelsWithContact / 1024m * 100m;
            
            // Round to 1 decimal place
            return Math.Round(contactPercent, 1);
        }
        
      

        // This method calculates the Coefficient of Variation      
        private decimal CalculateCoefficientOfVariation()
        {
            //Collects all pressure values above threshold
            var values = new List<int>();

            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    if (Data[i, j] > 1) // Ignore very low pressures
                    {
                        values.Add(Data[i, j]);
                    }
                }
            }
            
            // If no pressure values, CV = 0
            if (values.Count == 0) return 0;
            
            // Calculates the Mean (average)
            double mean = values.Average();
            
            // If mean is 0, CV = 0 to avoid division by zero
            if (mean == 0) return 0;
            
            // Calculates the sum of squared differences
            double sumSquaredDifferences = 0;
            foreach (var value in values)
            {
                double difference = value - mean;
                sumSquaredDifferences += difference * difference;
            }
            
           //Calculates Variance
            double variance = sumSquaredDifferences / values.Count;
            
            //Calculates the Standard Deviation
            double standardDeviation = Math.Sqrt(variance);
            
            //Calculates the Coefficient of Variation
            double coefficientOfVariation = (standardDeviation / mean) * 100;
            
            // Round to 1 decimal place
            return Math.Round((decimal)coefficientOfVariation, 1);
        }
        

       // This method calculates the Risk Score based on peak pressure and contact area
        private int CalculateRiskScore(int peakPressure, decimal contactArea)
        {
            int score = 0;
            
            // This is an estimated scoring system
            if (peakPressure > 150) score += 4;      // Very high
            else if (peakPressure > 100) score += 3; // High
            else if (peakPressure > 80) score += 2;  // Medium
            else if (peakPressure > 60) score += 1;  // Low
            
            // Add points based on contact area (lower = worse)
            if (contactArea < 20) score += 5;        // Very concentrated
            else if (contactArea < 30) score += 4;   // Concentrated
            else if (contactArea < 40) score += 3;   // Somewhat concentrated
            else if (contactArea < 50) score += 2;   // Moderate
            else score += 1;                          // Well distributed
            
            // This clamps score between 1 and 10
            return Math.Max(1, Math.Min(10, score));
        }
    }
    

    // This class is used in the region-finding algorithm
    public class PressureRegion
    {
        public int PixelCount { get; set; }
        public int MaxPressure { get; set; }
    }
    
    // This class holds the calculated pressure metrics
    public class PressureMetrics
    {
        public int PeakPressure { get; set; }
        public decimal ContactAreaPercent { get; set; }
        public decimal CoefficientOfVariation { get; set; }
        public int RiskScore { get; set; }
    }
}