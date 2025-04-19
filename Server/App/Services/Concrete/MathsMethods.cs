namespace App.Services.Concrete;

public class MathsMethods
{
    public static double CosineSimilarity(double[] a, double[] b)
    {
        if (a.Length != b.Length) throw new ArgumentException("Vectors must be same length");
        double dot = 0, magA = 0, magB = 0;
        for (int i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            magA += a[i] * a[i];
            magB += b[i] * b[i];
        }
        return dot / (Math.Sqrt(magA) * Math.Sqrt(magB) + 1e-9);
    }

    public static double EuclideanDistance(double[] a, double[] b)
    {
        if (a.Length != b.Length) throw new ArgumentException("Vectors must be same length");
        double sum = 0;
        for (int i = 0; i < a.Length; i++) sum += Math.Pow(a[i] - b[i], 2);
        return Math.Sqrt(sum);
    }
}