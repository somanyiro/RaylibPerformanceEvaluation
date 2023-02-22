using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Tests
{
public class SponzaScene
{
    public static void Test()
        {
            // Initialization
            //--------------------------------------------------------------------------------------
            const int screenWidth = 1280;
            const int screenHeight = 720;

            InitWindow(screenWidth, screenHeight, "raylib [core] example - 3d camera free");

            // Define the camera to look into our 3d world
            Camera3D camera;
            camera.position = new Vector3(10.0f, 10.0f, 10.0f); // Camera3D position
            camera.target = new Vector3(0.0f, 0.0f, 0.0f);      // Camera3D looking at point
            camera.up = new Vector3(0.0f, 1.0f, 0.0f);          // Camera3D up vector (rotation towards target)
            camera.fovy = 45.0f;                                // Camera3D field-of-view Y
            camera.projection = CameraProjection.CAMERA_PERSPECTIVE;             // Camera3D mode type

            SetCameraMode(camera, CameraMode.CAMERA_FREE); // Set a free camera mode
            
            Model sponza = LoadModel("resources/models/sponza.gltf");

            SetTargetFPS(60);                   // Set our game to run at 60 frames-per-second
            //--------------------------------------------------------------------------------------

            // Main game loop
            while (!WindowShouldClose()) // Detect window close button or ESC key
            {
                // Update
                //----------------------------------------------------------------------------------
                UpdateCamera(ref camera);          // Update camera

                if (IsKeyDown(KeyboardKey.KEY_Z))
                    camera.target = new Vector3(0.0f, 0.0f, 0.0f);
                //----------------------------------------------------------------------------------

                // Draw
                //----------------------------------------------------------------------------------
                BeginDrawing();
                ClearBackground(Color.RAYWHITE);

                BeginMode3D(camera);

                DrawModel(sponza, Vector3.Zero, 0.1f, Color.WHITE);

                EndMode3D();
                
                DrawFPS(10,10);

                EndDrawing();
                //----------------------------------------------------------------------------------
            }

            // De-Initialization
            //--------------------------------------------------------------------------------------
            CloseWindow();        // Close window and OpenGL context
            //--------------------------------------------------------------------------------------

        }
}
}
