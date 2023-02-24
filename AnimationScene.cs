using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Tests
{
public class AnimationScene
{

    public static void Test()
    {
        InitWindow(1280, 720, "Animation test");
        
        //SetTargetFPS(60);

        Camera3D camera;
        camera.position = new Vector3(10.0f, 10.0f, 10.0f);
        camera.target = new Vector3(0.0f, 0.0f, 0.0f);
        camera.up = new Vector3(0.0f, 1.0f, 0.0f);
        camera.fovy = 45.0f;
        camera.projection = CameraProjection.CAMERA_PERSPECTIVE;

        SetCameraMode(camera, CameraMode.CAMERA_FREE);

        int gridSize = 3;
        float separation = 5;
        List<Dancer> dancers = new List<Dancer>();

        for (int i = 0; i < gridSize; i++)
        
        {
            for (int j = 0; j < gridSize; j++)
            {
                dancers.Add(new Dancer(
                    "resources/models/dancer_mid.iqm",
                    "resources/models/danceTexture.png",
                    24,
                    new Vector3(i*separation, 0, j*separation),
                    0.05f));
            }
        }



        while (!WindowShouldClose())
        {
            UpdateCamera(ref camera);

            float delta = GetFrameTime();
            
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

        mesh = LoadModel(modelPath);
        texture = LoadTexture(texturePath);
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