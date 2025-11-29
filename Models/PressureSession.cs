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
        
        /// <summary>
        /// The actual 32x32 pressure matrix stored as JSON
        /// Contains all 1,024 pressure values (32 rows × 32 columns)
        /// Example: "[[0,0,61,35,...],[0,0,23,0,...],...]"
        /// 
        /// WHY JSON?
        /// - Easy to store variable-sized data
   
        public string MatrixJson { get; set; } = string.Empty;
        
        // ============================================
        // PRE-CALCULATED METRICS (Saves time!)
        // ============================================
        
        /// <summary>
        /// Peak Pressure Index (mmHg)
        /// Highest pressure in regions with 10+ pixels
        /// Example: 185
        /// </summary>
        public int PeakPressure { get; set; }
        
        /// <summary>
        /// Contact Area Percentage
        /// How much of the mat has pressure (0-100%)
        /// Example: 42.5 means 42.5% of mat is in contact
        /// </summary>
        public decimal ContactAreaPercent { get; set; }
        
        /// <summary>
        /// Coefficient of Variation
        /// Measures pressure distribution (StdDev / Mean × 100)
        /// Low value = even pressure (good)
        /// High value = concentrated pressure (risky)
        /// Example: 28.3
        /// </summary>
        public decimal CoefficientOfVariation { get; set; }
        
        /// <summary>
        /// Risk Score (1-10)
        /// Calculated from peak pressure and contact area
        /// 1-3 = Low risk (green)
        /// 4-6 = Medium risk (yellow)
        /// 7-10 = High risk (red)
        /// </summary>
        public int RiskScore { get; set; }
        
        /// <summary>
        /// When was this data imported into our system?
        /// Auto-set to current time
        /// </summary>
        public DateTime ImportedAt { get; set; } = DateTime.UtcNow;
    }
    
    // ============================================
    // IN-MEMORY CLASS: Holds 32x32 pressure data
    // ============================================
    
    /// <summary>
    /// Represents a 32x32 grid of pressure values
    /// This is NOT stored in database directly
    /// It's used for calculations and then converted to JSON
    /// 
    /// THINK OF IT LIKE:
    /// A temporary workspace where we process the CSV data
    /// </summary>
    public class PressureMatrix
    {
        /// <summary>
        /// The 32x32 array of pressure values
        /// Data[row, column] = pressure value (1-255)
        /// 
        /// Example:
        /// Data[0, 0] = 0     (top-left, no pressure)
        /// Data[10, 10] = 85  (center area, medium pressure)
        /// Data[31, 31] = 0   (bottom-right, no pressure)
        /// </summary>
        public int[,] Data { get; set; } = new int[32, 32];
        
        /// <summary>
        /// MAIN METHOD: Calculate all 4 metrics at once
        /// This is called after loading CSV data
        /// 
        /// FLOW:
        /// 1. Load CSV → PressureMatrix
        /// 2. Call CalculateMetrics()
        /// 3. Returns: Peak, Contact%, CV, Risk Score
        /// 4. Save to database
        /// </summary>
        public PressureMetrics CalculateMetrics()
        {
            var metrics = new PressureMetrics();
            
            // Calculate each metric
            metrics.PeakPressure = CalculatePeakPressureIndex();
            metrics.ContactAreaPercent = CalculateContactAreaPercent();
            metrics.CoefficientOfVariation = CalculateCoefficientOfVariation();
            metrics.RiskScore = CalculateRiskScore(metrics.PeakPressure, metrics.ContactAreaPercent);
            
            return metrics;
        }
        
        // ============================================
        // METRIC 1: Peak Pressure Index
        // ============================================
        
        /// <summary>
        /// Find the highest pressure value, but ONLY from regions with 10+ pixels
        /// 
        /// WHY EXCLUDE SMALL REGIONS?
        /// - Single high pixels could be sensor noise
        /// - We care about SUSTAINED pressure areas
        /// 
        /// ALGORITHM:
        /// 1. Find all contiguous pressure regions (using flood-fill)
        /// 2. Filter out regions smaller than 10 pixels
        /// 3. Return the highest pressure from valid regions
        /// 
        /// EXAMPLE:
        /// Region A: 3 pixels, max pressure = 200  ← IGNORED (too small)
        /// Region B: 15 pixels, max pressure = 150 ← VALID
        /// Region C: 20 pixels, max pressure = 180 ← VALID
        /// Peak Pressure Index = 180
        /// </summary>
        private int CalculatePeakPressureIndex()
        {
            // Step 1: Find all pressure regions
            var regions = FindPressureRegions();
            
            // Step 2: Keep only regions with 10+ pixels
            var validRegions = regions.Where(r => r.PixelCount >= 10).ToList();
            
            // Step 3: If no valid regions, return 0
            if (validRegions.Count == 0)
            {
                return 0;
            }
            
            // Step 4: Return highest pressure from valid regions
            return validRegions.Max(r => r.MaxPressure);
        }
        
        /// <summary>
        /// Find all contiguous pressure regions in the matrix
        /// Uses flood-fill algorithm (like paint bucket in image editors)
        /// 
        /// HOW IT WORKS:
        /// - Start at a pixel with pressure > 10
        /// - Spread to all connected pixels (up, down, left, right)
        /// - Mark them as one "region"
        /// - Count pixels and track max pressure in that region
        /// 
        /// VISUAL EXAMPLE:
        /// 0  0  0  0  0
        /// 0 60 65 70  0  ← Region 1 (3 pixels, max=70)
        /// 0  0  0  0  0
        /// 0 50 55  0  0  ← Region 2 (2 pixels, max=55)
        /// 
        /// Returns: [Region1, Region2]
        /// </summary>
        private List<PressureRegion> FindPressureRegions()
        {
            // Track which pixels we've already visited
            bool[,] visited = new bool[32, 32];
            
            // List to store all regions we find
            var regions = new List<PressureRegion>();
            
            // Threshold: pixels above this value are "pressure"
            int pressureThreshold = 10;
            
            // Loop through entire matrix
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    // Found an unvisited pixel with pressure?
                    if (!visited[i, j] && Data[i, j] > pressureThreshold)
                    {
                        // Use flood-fill to find entire region
                        var region = FloodFillRegion(i, j, visited, pressureThreshold);
                        regions.Add(region);
                    }
                }
            }
            
            return regions;
        }
        
        /// <summary>
        /// Flood-fill algorithm to find a contiguous pressure region
        /// Similar to how a "fill" tool works in paint programs
        /// 
        /// ALGORITHM:
        /// 1. Start at (startRow, startCol)
        /// 2. Add it to a stack
        /// 3. While stack not empty:
        ///    a. Pop a pixel
        ///    b. Mark it as visited
        ///    c. Add its 4 neighbors (up, down, left, right) to stack
        /// 4. Count pixels and track max pressure
        /// 
        /// EXAMPLE:
        /// Start at pixel (1,1) with value 60
        /// → Check up (0,1), down (2,1), left (1,0), right (1,2)
        /// → If they have pressure, add to stack
        /// → Repeat until no more connected pixels
        /// </summary>
        private PressureRegion FloodFillRegion(int startRow, int startCol, bool[,] visited, int threshold)
        {
            var region = new PressureRegion();
            
            // Stack to track pixels to process
            var stack = new Stack<(int row, int col)>();
            stack.Push((startRow, startCol));
            
            while (stack.Count > 0)
            {
                var (row, col) = stack.Pop();
                
                // Skip if out of bounds
                if (row < 0 || row >= 32 || col < 0 || col >= 32)
                    continue;
                
                // Skip if already visited or no pressure
                if (visited[row, col] || Data[row, col] <= threshold)
                    continue;
                
                // Mark as visited
                visited[row, col] = true;
                
                // Increment pixel count for this region
                region.PixelCount++;
                
                // Update max pressure if this pixel is higher
                if (Data[row, col] > region.MaxPressure)
                {
                    region.MaxPressure = Data[row, col];
                }
                
                // Add 4 neighbors to stack (up, down, left, right)
                stack.Push((row - 1, col)); // Up
                stack.Push((row + 1, col)); // Down
                stack.Push((row, col - 1)); // Left
                stack.Push((row, col + 1)); // Right
            }
            
            return region;
        }
        
        // ============================================
        // METRIC 2: Contact Area Percentage
        // ============================================
        
        /// <summary>
        /// Calculate what percentage of the mat has pressure on it
        /// 
        /// FORMULA:
        /// ContactArea% = (PixelsWithPressure / TotalPixels) × 100
        /// 
        /// HOW IT WORKS:
        /// - Total pixels = 32 × 32 = 1,024
        /// - Count how many pixels have value > 1 (pressure applied)
        /// - Divide and convert to percentage
        /// 
        /// EXAMPLE:
        /// - 434 pixels have pressure > 1
        /// - Contact Area = (434 / 1024) × 100 = 42.4%
        /// 
        /// WHAT IT MEANS:
        /// - Low % (10-30%): Person sitting on small area → concentrated pressure
        /// - High % (50-80%): Person well-distributed → good pressure spread
        /// </summary>
        private decimal CalculateContactAreaPercent()
        {
            int pixelsWithContact = 0;
            
            // Value of 1 = no pressure (default/zero-force)
            // Anything above 1 = pressure applied
            int lowerThreshold = 1;
            
            // Count pixels with pressure
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
            
            // Calculate percentage
            // Example: 434 pixels / 1024 total = 0.424 × 100 = 42.4%
            decimal contactPercent = (decimal)pixelsWithContact / 1024m * 100m;
            
            // Round to 1 decimal place
            return Math.Round(contactPercent, 1);
        }
        
        // ============================================
        // METRIC 3: Coefficient of Variation
        // ============================================
        
        /// <summary>
        /// Measures how spread out the pressure values are
        /// 
        /// FORMULA:
        /// CV = (Standard Deviation / Mean) × 100
        /// 
        /// WHAT IT MEANS:
        /// - Low CV (10-20%): Pressure evenly distributed (GOOD!)
        /// - High CV (40-60%): Pressure concentrated in spots (RISKY!)
        /// 
        /// STEP-BY-STEP EXAMPLE:
        /// Values: [50, 55, 60, 65, 70]
        /// 
        /// Step 1: Calculate Mean
        /// Mean = (50 + 55 + 60 + 65 + 70) / 5 = 60
        /// 
        /// Step 2: Calculate differences from mean
        /// [50-60, 55-60, 60-60, 65-60, 70-60]
        /// = [-10, -5, 0, 5, 10]
        /// 
        /// Step 3: Square the differences
        /// [100, 25, 0, 25, 100]
        /// 
        /// Step 4: Calculate variance (average of squared differences)
        /// Variance = (100 + 25 + 0 + 25 + 100) / 5 = 50
        /// 
        /// Step 5: Standard Deviation (square root of variance)
        /// StdDev = √50 = 7.07
        /// 
        /// Step 6: Coefficient of Variation
        /// CV = (7.07 / 60) × 100 = 11.8%
        /// </summary>
        private decimal CalculateCoefficientOfVariation()
        {
            // Step 1: Collect all pressure values (exclude zero-force pixels)
            var values = new List<int>();
            
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    if (Data[i, j] > 1) // Exclude default zero-force value
                    {
                        values.Add(Data[i, j]);
                    }
                }
            }
            
            // If no pressure values, CV = 0
            if (values.Count == 0) return 0;
            
            // Step 2: Calculate Mean (average)
            double mean = values.Average();
            
            // Avoid division by zero
            if (mean == 0) return 0;
            
            // Step 3: Calculate sum of squared differences
            double sumSquaredDifferences = 0;
            foreach (var value in values)
            {
                double difference = value - mean;
                sumSquaredDifferences += difference * difference;
            }
            
            // Step 4: Calculate Variance
            double variance = sumSquaredDifferences / values.Count;
            
            // Step 5: Calculate Standard Deviation
            double standardDeviation = Math.Sqrt(variance);
            
            // Step 6: Calculate Coefficient of Variation
            double coefficientOfVariation = (standardDeviation / mean) * 100;
            
            // Round to 1 decimal place
            return Math.Round((decimal)coefficientOfVariation, 1);
        }
        
        // ============================================
        // METRIC 4: Risk Score (1-10)
        // ============================================
        
        /// <summary>
        /// Calculate overall risk of pressure ulcer
        /// Combines peak pressure and contact area
        /// 
        /// SCORING SYSTEM:
        /// High Peak Pressure = More points (bad)
        /// Low Contact Area = More points (bad)
        /// 
        /// Total points → Risk score (1-10)
        /// 
        /// EXAMPLE CALCULATION:
        /// Peak Pressure = 185 mmHg  → +4 points (very high)
        /// Contact Area = 25%        → +4 points (very low/concentrated)
        /// Total = 8 points          → Risk Score = 8/10 (HIGH RISK)
        /// </summary>
        private int CalculateRiskScore(int peakPressure, decimal contactArea)
        {
            int score = 0;
            
            // Add points based on peak pressure
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
            
            // Ensure score stays between 1-10
            return Math.Max(1, Math.Min(10, score));
        }
    }
    
    // ============================================
    // HELPER CLASS: Represents one pressure region
    // ============================================
    
    /// <summary>
    /// Stores info about a contiguous pressure region
    /// Used during Peak Pressure Index calculation
    /// </summary>
    public class PressureRegion
    {
        /// <summary>
        /// How many pixels are in this region?
        /// Example: 15 pixels
        /// </summary>
        public int PixelCount { get; set; }
        
        /// <summary>
        /// What's the highest pressure in this region?
        /// Example: 185 mmHg
        /// </summary>
        public int MaxPressure { get; set; }
    }
    
    // ============================================
    // RESULT CLASS: Holds calculated metrics
    // ============================================
    
    /// <summary>
    /// Simple class to hold all 4 calculated metrics
    /// Returned by CalculateMetrics()
    /// </summary>
    public class PressureMetrics
    {
        public int PeakPressure { get; set; }
        public decimal ContactAreaPercent { get; set; }
        public decimal CoefficientOfVariation { get; set; }
        public int RiskScore { get; set; }
    }
}