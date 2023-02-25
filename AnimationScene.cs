using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Tests
{
public class AnimationScene
{

    static float separation = 5;
    static float stabilizeTime = 5;
    static float testDuration = 10;
    static List<Dancer> dancers;
    static Timer stabilizeTimer = new Timer(stabilizeTime, false); //wait for the framerate to stabilize
    static Timer testDurationTimer = new Timer(testDuration, false); //move onto the next test

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
        
        SetUp("dancer_low", 2, 24);

        while (!WindowShouldClose())
        {
            UpdateCamera(ref camera);

            float delta = GetFrameTime();

            stabilizeTimer.AdvanceTimer(delta);
            testDurationTimer.AdvanceTimer(delta);

            if (stabilizeTimer.finishedThisFrame)
                Console.WriteLine("Beginning frame logging");

            if (stabilizeTimer.finished)
                Logger.RecordFrame(delta);

            if (testDurationTimer.finishedThisFrame)
                break;

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

        Logger.Save();

        CloseWindow();
    }

    static void SetUp(string modelPath, int gridSize, int animationSpeed)
    {
        dancers = new List<Dancer>();

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                dancers.Add(new Dancer(
                    "resources/models/"+modelPath+".iqm",
                    "resources/models/danceTexture.png",
                    animationSpeed,
                    new Vector3(i*separation - (float)gridSize*separation/2f, 0, j*separation - (float)gridSize*separation/2f),
                    0.05f));
            }
        }

        stabilizeTimer.Reset();
        testDurationTimer.Reset();

        Logger.SetInformation(modelPath, gridSize*gridSize, animationSpeed);
    }

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