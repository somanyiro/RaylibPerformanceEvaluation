using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Tests
{
public class AnimationScene
{
    static float modelSeparation = 5;
    static float stabilizeTime = 10;
    static float testDuration = 20;
    static List<Dancer> dancers = new List<Dancer>();
    static string[] models = {
        "dancer_low",
        "dancer_mid",
        "dancer_high"
    };
    static string[] textures = {
        "danceTexture_256",
        "danceTexture_512",
        "danceTexture_1024",
        "danceTexture_2048",
        "danceTexture_4096"
    };
    static List<Config> testConfigs = new List<Config>() {
        //new Config("test name", 1, 25, 24)
    };

    public static void Test()
    {
        CreateTestConfigs();//modifiy this function if you have a lot of tests to set up
        
        InitWindow(1280, 720, "Animation test");
        
        //SetTargetFPS(60);

        Camera3D camera;
        camera.position = new Vector3(20.0f, 30.0f, 20.0f);
        camera.target = new Vector3(0.0f, 3.0f, 0.0f);
        camera.up = new Vector3(0.0f, 1.0f, 0.0f);
        camera.fovy = 60.0f;
        camera.projection = CameraProjection.CAMERA_PERSPECTIVE;

        SetCameraMode(camera, CameraMode.CAMERA_ORBITAL);
        
        Timer stabilizeTimer = new Timer(stabilizeTime, true); //wait for the framerate to stabilize
        Timer testDurationTimer = new Timer(testDuration, true); //move onto the next test

        SetUp(testConfigs[0]);
        int currentConfig = 0;

        bool timerFrameSkip = false; //variable needed to make the timers skip the frame where the models are being loaded

        while (!WindowShouldClose())
        {
            float delta = GetFrameTime();

            if (timerFrameSkip)
            {
                timerFrameSkip = false;
            }
            else
            {
                stabilizeTimer.AdvanceTimer(delta);
                testDurationTimer.AdvanceTimer(delta);
            }

            if (stabilizeTimer.finishedThisFrame)
                Console.WriteLine("Beginning frame logging");

            if (stabilizeTimer.finished)
                Logger.RecordFrame(delta);

            if (testDurationTimer.finishedThisFrame)
            {
                Logger.RecordTestResult(testConfigs[currentConfig]);
                dancers.Clear();
                if (currentConfig == testConfigs.Count-1) break;
                currentConfig++;
                SetUp(testConfigs[currentConfig]);
                stabilizeTimer.Reset();
                testDurationTimer.Reset();
                timerFrameSkip = true;
            }

            UpdateCamera(ref camera);
            
            BeginDrawing();
            ClearBackground(Color.RAYWHITE);

            BeginMode3D(camera);
            
            foreach (var item in dancers)
            {
                item.AdvanceAnimation(delta);
                item.DrawDancer();
            }
            
            EndMode3D();

            DrawFPS(10, 10);

            EndDrawing();
        }

        //Logger.Save(Config.Variable.ModelCount, Config.Variable.ModelId); //for testing performance change with number of models
        //Logger.Save(Config.Variable.AnimationSpeed, Config.Variable.ModelId); //for testing change with animation speed
        Logger.Save(Config.Variable.TextureId, Config.Variable.ModelId); //for testing texture resolutions
        CloseWindow();
    }

    static void SetUp(Config config)
    {
        int gridSize = (int)MathF.Ceiling(MathF.Sqrt(config.modelCount));

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (dancers.Count == config.modelCount) break;

                dancers.Add(new Dancer(
                    "resources/models/"+models[config.modelId]+".iqm",
                    "resources/models/"+textures[config.textureId]+".png",
                    config.animationSpeed,
                    new Vector3(i*modelSeparation - (float)gridSize*modelSeparation/2f, 0, j*modelSeparation - (float)gridSize*modelSeparation/2f),
                    0.05f,
                    0.1f * (i+j)));
            }
        }

        Console.WriteLine($"model id: {config.modelId}\nmodel count: {config.modelCount}\nanimation speed: {config.animationSpeed}\ntexture id: {config.textureId}");
    }

    static void CreateTestConfigs()
    {

        //general performance test
        /*
        for (int i = 1; i <= 50; i++)
        {
            testConfigs.Add(new Config("low vertex", 0, i, 24, 4));
        }
        for (int i = 1; i <= 50; i++)
        {
            testConfigs.Add(new Config("mid vertex", 1, i, 24, 4));
        }
        for (int i = 1; i <= 50; i++)
        {
            testConfigs.Add(new Config("high vertex", 2, i, 24, 4));
        }
        */
        //animation speed test
        /*
        for (int i = 1; i <= 20; i++)
        {
            testConfigs.Add(new Config("low vertex", 0, 10, 6 * i, 4));
        }
        for (int i = 1; i <= 20; i++)
        {
            testConfigs.Add(new Config("mid vertex", 1, 10, 6 * i, 4));
        }
        for (int i = 1; i <= 20; i++)
        {
            testConfigs.Add(new Config("high vertex", 2, 10, 6 * i, 4));
        }
        */
        //texture resolution test
        for (int i = 0; i <= 4; i++)
        {
            testConfigs.Add(new Config("low vertex", 0, 10, 24, i));
        }
        for (int i = 0; i <= 4; i++)
        {
            testConfigs.Add(new Config("mid vertex", 1, 10, 24, i));
        }
        for (int i = 0; i <= 4; i++)
        {
            testConfigs.Add(new Config("high vertex", 2, 10, 24, i));
        }
    }

}

public struct Config
{
    public enum Variable
    {
        ModelId,
        ModelCount,
        AnimationSpeed,
        TextureId
    }

    public Config(string label, int modelId, int modelCount, int animationSpeed, int textureId)
    {
        this.label = label;
        this.modelId = modelId;
        this.modelCount = modelCount;
        this.animationSpeed = animationSpeed;
        this.textureId = textureId;
    }
    public int GetValue(Variable variable)
    {
        switch (variable)
        {
            case Variable.ModelId:
                return modelId;
            case Variable.ModelCount:
                return modelCount;
            case Variable.AnimationSpeed:
                return animationSpeed;
            case Variable.TextureId:
                return textureId;
            default:
                break;
        }
        return 0;
    }
    public static string GetName(Variable variable)
    {
        switch (variable)
        {
            case Variable.ModelId:
                return "modelId";
            case Variable.ModelCount:
                return "modelCount";
            case Variable.AnimationSpeed:
                return "animationSpeed";
            case Variable.TextureId:
                return "textureId";
            default:
                break;
        }
        return "-";
    }
    public string label;
    public int modelId;
    public int modelCount;
    public int animationSpeed;
    public int textureId;
}
}