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
        
        SetTargetFPS(60);

        Camera3D camera;
        camera.position = new Vector3(10.0f, 10.0f, 10.0f);
        camera.target = new Vector3(0.0f, 0.0f, 0.0f);
        camera.up = new Vector3(0.0f, 1.0f, 0.0f);
        camera.fovy = 45.0f;
        camera.projection = CameraProjection.CAMERA_PERSPECTIVE;

        SetCameraMode(camera, CameraMode.CAMERA_FREE);

        Model dancer = LoadModel("resources/models/dance.iqm");
        uint animsCount = 0;
        var anims = LoadModelAnimations("resources/models/dance.iqm", ref animsCount);
        int animFrameCounter = 0;
        Texture2D dancerTexture = LoadTexture("resources/models/danceTexture.png");
        SetMaterialTexture(ref dancer, 0, MaterialMapIndex.MATERIAL_MAP_ALBEDO, ref dancerTexture);


        while (!WindowShouldClose())
        {
            UpdateCamera(ref camera);
            
            animFrameCounter++;
            UpdateModelAnimation(dancer, anims[0], animFrameCounter);
            if (animFrameCounter >= anims[0].frameCount)
                animFrameCounter = 0;
            
            BeginDrawing();
            ClearBackground(Color.RAYWHITE);

            BeginMode3D(camera);

            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    DrawModel(dancer, new Vector3(x*10, 0, y*10), 0.05f, Color.WHITE);
                }
            }

            EndMode3D();

            DrawFPS(10, 10);

            EndDrawing();
        }

        CloseWindow();
    }
}
}