namespace App.Services.RecommendationSystem.Test;

public interface ICosineSimilarityService
{
    float Compute(float[] a, float[] b);
}