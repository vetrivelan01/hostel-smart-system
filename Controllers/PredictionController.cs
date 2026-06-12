using Hostelfoodsystem.Data;
using Hostelfoodsystem.DTOs;
using Hostelfoodsystem.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hostelfoodsystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PredictionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PredictionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("predict")]
        public async Task<IActionResult> GetPrediction([FromBody] PredictionRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.MealType))
            {
                return BadRequest("Invalid request parameters");
            }

            // Fetch historical meals of the same type to make a prediction
            var historicalMeals = await _context.MealRecords
                .Where(m => m.MealType.ToLower() == request.MealType.ToLower())
                .ToListAsync();

            double consumptionRate = 0.85; // Default baseline rate (85%)
            int avgPrepared = 150;
            int avgConsumed = 130;

            if (historicalMeals.Any())
            {
                avgPrepared = (int)historicalMeals.Average(m => m.PreparedCount);
                avgConsumed = (int)historicalMeals.Average(m => m.ConsumedCount);
                var totalPrepared = historicalMeals.Sum(m => m.PreparedCount);
                if (totalPrepared > 0)
                {
                    consumptionRate = (double)historicalMeals.Sum(m => m.ConsumedCount) / totalPrepared;
                }
            }

            // Estimate count based on expected attendance and average consumption rate
            int predictedConsumed = (int)Math.Round(request.ExpectedAttendance * consumptionRate);
            
            // Recommend preparing slightly more than predicted to ensure safety (e.g. 7% safety margin)
            int recommendedPrepared = (int)Math.Max(predictedConsumed + 10, (int)Math.Round(predictedConsumed * 1.07));
            int expectedWaste = recommendedPrepared - predictedConsumed;

            // Generate a confidence score based on the amount of historical data (more data = higher confidence)
            double confidenceScore = Math.Min(0.5 + (historicalMeals.Count * 0.05), 0.95);

            // Save the prediction to PredictionRecords database
            var predictionRecord = new PredictionRecord
            {
                PredictionDate = request.Date,
                MealType = request.MealType,
                PredictedCount = recommendedPrepared,
                ActualCount = null
            };

            _context.PredictionRecords.Add(predictionRecord);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                PredictionDate = request.Date.ToString("yyyy-MM-dd"),
                MealType = request.MealType,
                ExpectedAttendance = request.ExpectedAttendance,
                PredictedConsumedCount = predictedConsumed,
                RecommendedPreparedCount = recommendedPrepared,
                ExpectedWaste = expectedWaste,
                ConfidenceScore = Math.Round(confidenceScore, 2),
                HistoricalAveragePrepared = avgPrepared,
                HistoricalAverageConsumed = avgConsumed,
                Message = historicalMeals.Any() 
                    ? $"Prediction generated using {historicalMeals.Count} historical logs."
                    : "No historical logs found. Prediction generated using default mess baseline rules."
            });
        }
    }
}
