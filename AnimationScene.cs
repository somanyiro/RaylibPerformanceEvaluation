using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Tests
{
public class AnimationScene
{

    static float modelSeparation = 5;
    static float stabilizeTime = 1;
    static float testDuration = 5;
    static List<Dancer> dancers;
    static string[] models = {
        "dancer_low",
        "dancer_mid",
        "dancer_high"
    };
    static Config[] testConfigs = {
        new Config("low vertex count model", 0, 16, 24),
        new Config("low vertex count model", 0, 64, 24),
        new Config("mid vertex count model", 1, 16, 24),
        new Config("mid vertex count model", 1, 64, 24),
        new Config("high vertex count model", 2, 16, 24),
        new Config("high vertex count model", 2, 64, 24),
        /*
        new Config("low vertex count model", 0, 1, 24),
        new Config("low vertex count model", 0, 4, 24),
        new Config("low vertex count model", 0, 8, 24),
        new Config("low vertex count model", 0, 16, 24),
        new Config("low vertex count model", 0, 32, 24),
        new Config("low vertex count model", 0, 64, 24),
        new Config("low vertex count model", 0, 128, 24),

        new Config("mid vertex count model", 1, 1, 24),
        new Config("mid vertex count model", 1, 4, 24),
        new Config("mid vertex count model", 1, 8, 24),
        new Config("mid vertex count model", 1, 16, 24),
        new Config("mid vertex count model", 1, 32, 24),
        new Config("mid vertex count model", 1, 64, 24),
        new Config("mid vertex count model", 1, 128, 24),

        new Config("high vertex count model", 2, 1, 24),
        new Config("high vertex count model", 2, 4, 24),
        new Config("high vertex count model", 2, 8, 24),
        new Config("high vertex count model", 2, 16, 24),
        new Config("high vertex count model", 2, 32, 24),
        new Config("high vertex count model", 2, 64, 24),
        new Config("high vertex count model", 2, 128, 24)
        */
    };

    public static void Test()
    {
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

        while (!WindowShouldClose())
        {
            float delta = GetFrameTime();

            stabilizeTimer.AdvanceTimer(delta);
            testDurationTimer.AdvanceTimer(delta);

            if (stabilizeTimer.finishedThisFrame)
                Console.WriteLine("Beginning frame logging");

            if (stabilizeTimer.finished)
                Logger.RecordFrame(delta);

            if (testDurationTimer.finishedThisFrame)
            {
                Logger.RecordTestResult(testConfigs[currentConfig]);
                if (currentConfig == testConfigs.Length-1) break;
                currentConfig++;
                SetUp(testConfigs[currentConfig]);
                stabilizeTimer.Reset();
                testDurationTimer.Reset();
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

        Logger.Save(Config.Variable.ModelCount, Config.Variable.ModelId);
        CloseWindow();
    }

    static void SetUp(Config config)
    {
        dancers = new List<Dancer>();

        int gridSize = (int)MathF.Ceiling(MathF.Sqrt(config.modelCount));

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (dancers.Count == config.modelCount) break;

                dancers.Add(new Dancer(
                    "resources/models/"+models[config.modelId]+".iqm",
                    "resources/models/danceTexture.png",
                    config.animationSpeed,
                    new Vector3(i*modelSeparation - (float)gridSize*modelSeparation/2f, 0, j*modelSeparation - (float)gridSize*modelSeparation/2f),
                    0.05f));
            }
        }
    }

}

public struct Config
{
    public enum Variable
    {
        ModelId,
        ModelCount,
        AnimationSpeed
    }

    public Config(string label, int modelId, int modelCount, int animationSpeed)
    {
        this.label = label;
        this.modelId = modelId;
        this.modelCount = modelCount;
        this.animationSpeed = animationSpeed;
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
            default:
                break;
        }
        return "-";
    }
    public string label;
    public int modelId;
    public int modelCount;
    public int animationSpeed;
}

class Dancer
{
    Model mesh;
    float animFramesPerSec = 1;
    uint numberOfAnims = 0;
    int currentAnimFrame = 0;
    float timeSinceLastAnimFrame = 0;
    Texture2D texture;
    List<ModelAnimation> animations = new List<ModelAnimation>();
    Vector3 position = Vector3.Zero;
    float scale = 1;

    public Dancer(string modelPath, string texturePath, float animSpeed, Vector3 position, float scale)
    {
        this.position = position;
        this.scale = scale;

        mesh = AssetManager.LoadModel(modelPath);
        texture = AssetManager.LoadTexture(texturePath);
        SetMaterialTexture(ref mesh, 0, MaterialMapIndex.MATERIAL_MAP_ALBEDO, ref texture);

        var animsSpan = LoadModelAnimations(modelPath, ref numberOfAnims);
        foreach (var item in animsSpan)
        {
            animations.Add(item);
        }

        animFramesPerSec = animSpeed;
    }

    public void DrawDancer()
    {
        DrawModel(mesh, position, scale, Color.WHITE);
    }

    public void AdvanceAnimation(float delta)
    {
        timeSinceLastAnimFrame += delta;
        bool shouldUpdate = false;

        while (timeSinceLastAnimFrame > 1 / animFramesPerSec)
        {
            shouldUpdate = true;
            timeSinceLastAnimFrame -= 1 / animFramesPerSec;
            currentAnimFrame++;
            if (currentAnimFrame >= animations[0].frameCount)
                currentAnimFrame = 0;
        } 

        if (shouldUpdate) UpdateModelAnimation(mesh, animations[0], currentAnimFrame);

    }

}

}