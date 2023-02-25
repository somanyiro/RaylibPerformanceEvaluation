using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Tests
{
public class AnimationScene
{

    static float modelSeparation = 5;
    static float stabilizeTime = 5;
    static float testDuration = 10;
    static List<Dancer> dancers;
    static Config[] testConfigs = {
        new Config("dancer_low", 5, 24),
        new Config("dancer_mid", 5, 24),
        new Config("dancer_high", 5, 24)
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
                Logger.Save();
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

        CloseWindow();
    }

    static void SetUp(Config config)
    {
        dancers = new List<Dancer>();

        for (int i = 0; i < config.gridSize; i++)
        {
            for (int j = 0; j < config.gridSize; j++)
            {
                dancers.Add(new Dancer(
                    "resources/models/"+config.modelPath+".iqm",
                    "resources/models/danceTexture.png",
                    config.animationSpeed,
                    new Vector3(i*modelSeparation - (float)config.gridSize*modelSeparation/2f, 0, j*modelSeparation - (float)config.gridSize*modelSeparation/2f),
                    0.05f));
            }
        }

        Logger.SetInformation(config.modelPath, config.gridSize*config.gridSize, config.animationSpeed);
    }

}

struct Config
{
    public Config(string modelPath, int gridSize, float animationSpeed)
    {
        this.modelPath = modelPath;
        this.gridSize = gridSize;
        this.animationSpeed = animationSpeed;
    }

    public string modelPath;
    public int gridSize;
    public float animationSpeed;
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