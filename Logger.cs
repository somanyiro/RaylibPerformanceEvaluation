using System;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Tests
{
public static class Logger
{
    static string logOutput;
    static List<float> frames = new List<float>();

    public static void RecordFrame(float frameTime)
    {
        frames.Add(frameTime);
    }

    public static void Log()
    {

    }

    public static void Save()
    {
        CleanFrames();

        using (StreamWriter sw = new StreamWriter("logs.txt"))
        {
            float average = frames.Average();
            float best = frames.Min();
            float worst = frames.Max();
            
            List<float> ordered = frames.OrderBy(x => x).ToList();
            double middle = (frames.Count - 1) / 2.0;
            float median = (ordered[(int)(middle)] + ordered[(int)(middle + 0.5)]) / 2;

            sw.WriteLine($"Median frame time was: {median*1000}ms {1f/median}fps");
            sw.WriteLine($"Average frame time was: {average*1000}ms {1f/average}fps");
            sw.WriteLine($"Best frame time was: {best*1000}ms {1f/best}fps");
            sw.WriteLine($"Worst frame time was: {worst*1000}ms {1f/worst}fps");

            sw.WriteLine($"\nlisting frames\n");

            foreach (float f in frames)
            {
                sw.WriteLine($"{f*1000}ms {1f/f}fps");
            }

            sw.Close();
        }
    }

    static void CleanFrames()
    {
        List<float> ordered = frames.OrderBy(x => x).ToList();
        double middle = (frames.Count - 1) / 2.0;
        float median = (ordered[(int)(middle)] + ordered[(int)(middle + 0.5)]) / 2;

        List<float> cleanedFrames = new List<float>();
        foreach (float frame in frames)
        {
            if (frame > median*5 || frame < median/5) continue;

            cleanedFrames.Add(frame);
        }

        frames = cleanedFrames;
    }


}
}

