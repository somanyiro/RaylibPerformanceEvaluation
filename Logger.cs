using System;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Tests
{
public static class Logger
{
    static List<float> frames = new List<float>();
    static List<TestResult> results = new List<TestResult>();

    public static void RecordFrame(float frameTime)
    {
        frames.Add(frameTime);
    }

    public static void RecordTestResult(Config config)
    {
        if (frames.Count < 2) throw new Exception("Not Enough frames were recorded.");

        CleanFrames();

        float average = frames.Average();

        List<float> ordered = frames.OrderBy(x => x).ToList();
        double middle = (frames.Count - 1) / 2.0;
        float median = (ordered[(int)(middle)] + ordered[(int)(middle + 0.5)]) / 2;

        results.Add(new TestResult(
            config,
            (float)Math.Round(1f/average, 4),
            (float)Math.Round(1f/median, 4),
            (float)Math.Round(average*1000, 4),
            (float)Math.Round(median*1000, 4)
        ));

        ClearFrames();
    }

    public static void Save(Config.Variable columnVariable, Config.Variable rowVariable)
    {
        List<int> allColumns = new List<int>();
        List<int> allRows = new List<int>();
        foreach (TestResult result in results)
        {
            if (!allColumns.Contains(result.config.GetValue(columnVariable)))
                allColumns.Add(result.config.GetValue(columnVariable));

            if (!allRows.Contains(result.config.GetValue(rowVariable)))
                allRows.Add(result.config.GetValue(rowVariable));
        }
        allColumns.Sort();
        allRows.Sort();

        using (StreamWriter sw = new StreamWriter("logs.txt"))
        {
            sw.Write($"{Config.GetName(rowVariable)}\\{Config.GetName(columnVariable)};");
            foreach (int column in allColumns)
            {
                sw.Write(column+";");
            }
            sw.Write("\n");
            foreach (int row in allRows)
            {
                sw.Write(row+";");
                foreach (int column in allColumns)
                {
                    try
                    {
                        TestResult result = results.Single(
                            x => x.config.GetValue(columnVariable) == column 
                            && x.config.GetValue(rowVariable) == row);
                        sw.Write(result.averageFrameTime+";");
                        //this is messy and inefficient but it only runs once and I can't spend more time on it
                    }
                    catch (System.Exception e)
                    {
                        Console.WriteLine($"{e.Message} at column {column} and row {row}");
                        sw.Write("error;");
                    }
                }
                sw.Write("\n");
            }

            sw.Close();
        }
    }

    public static void ClearFrames()
    {
        frames.Clear();
    }

    static void CleanFrames()//raylib does a frame skip from once in a while, which I'm removing from the results here
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

struct TestResult
{
    public TestResult(Config config, float averageFps, float medianFps, float averageFrameTime, float medianFrameTime)
    {
        this.config = config;
        this.averageFps = averageFps;
        this.medianFps = medianFps;
        this.averageFrameTime = averageFrameTime;
        this.medianFrameTime = medianFrameTime;
    }

    public Config config;
    public float averageFps;
    public float medianFps;
    public float averageFrameTime;
    public float medianFrameTime;
}

}

