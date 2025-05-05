namespace App.Services.RecommendationSystem.Test;

public class CosineSimilarityService : ICosineSimilarityService
{
    public float Compute(float[] a, float[] b)
    {
        double dot = 0, magA = 0, magB = 0;
        for (int i = 0; i < a.Length; i++)
        {
            dot  += a[i] * b[i];
            magA += a[i] * a[i];
            magB += b[i] * b[i];
        }
        if (magA == 0 || magB == 0) return 0f;
        return (float)(dot / (Math.Sqrt(magA) * Math.Sqrt(magB)));
    }
}