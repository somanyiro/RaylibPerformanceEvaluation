using System;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Tests
{
public static class Logger
{
    static string information;
    static List<float> frames = new List<float>();

    public static void RecordFrame(float frameTime)
    {
        frames.Add(frameTime);
    }

    public static void SetInformation(string label, string model, int numberOfModels, float animationSpeed)
    {
        information = $"\n{label}\nmodel: {model}\nnumber of models: {numberOfModels}\nanimation speed (fps): {animationSpeed}\n";
    }

    public static void Save()
    {
        if (frames.Count < 2) return;

        CleanFrames();

        using (StreamWriter sw = new StreamWriter("logs.txt", true))
        {
            sw.WriteLine(information);

            float average = frames.Average();
            float best = frames.Min();
            float worst = frames.Max();
            
            List<float> ordered = frames.OrderBy(x => x).ToList();
            double middle = (frames.Count - 1) / 2.0;
            float median = (ordered[(int)(middle)] + ordered[(int)(middle + 0.5)]) / 2;

            sw.WriteLine($"Median: {Math.Round(median*1000, 2)}ms {Math.Round(1f/median, 2)}fps");
            sw.WriteLine($"Average: {Math.Round(average*1000, 2)}ms {Math.Round(1f/average, 2)}fps");
            sw.WriteLine($"Best: {Math.Round(best*1000, 2)}ms {Math.Round(1f/best, 2)}fps");
            sw.WriteLine($"Worst: {Math.Round(worst*1000, 2)}ms {Math.Round(1f/worst, 2)}fps");

            /*
            sw.WriteLine($"\nlisting frames\n");

            foreach (float f in frames)
            {
                sw.WriteLine($"{f*1000}ms {1f/f}fps");
            }
            */

            ClearFrames();

            sw.Close();
        }
    }

    public static void ClearFrames()
    {
        frames.Clear();
    }

    static void CleanFrames()
    {
        List<float> ordered = frames.OrderBy(x => x).ToList();
        double middle = (frames.Count - 1) / 2.0;
        float median = (ordered[(int)(middle)] + ordered[(int)(middle + 0.5)]) / 2;

        List<float> cleanedFrames = new List<float>();
        foreach (float frame in frames)
        {
            if (frame > median*3 || frame < median/3) continue;

            cleanedFrames.Add(frame);
        }

        frames = cleanedFrames;
    }


}
}

