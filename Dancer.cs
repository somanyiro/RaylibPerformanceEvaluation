using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;

namespace Tests
{
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

    Timer animDelayTimer;

    public Dancer(string modelPath, string texturePath, float animSpeed, Vector3 position, float scale, float animDelay = 0)
    {
        this.position = position;
        this.scale = scale;

        mesh = Raylib.LoadModel(modelPath);
        texture = AssetManager.LoadTexture(texturePath);

        SetMaterialTexture(ref mesh, 0, MaterialMapIndex.MATERIAL_MAP_ALBEDO, ref texture);

        var animsSpan = LoadModelAnimations(modelPath, ref numberOfAnims);

        foreach (var item in animsSpan)
        {
            animations.Add(item);
        }

        animFramesPerSec = animSpeed;

        animDelayTimer = new Timer(animDelay, true);
    }

    public void DrawDancer()
    {
        DrawModel(mesh, position, scale, Color.WHITE);
    }

    public void AdvanceAnimation(float delta)
    {
        animDelayTimer.AdvanceTimer(delta);
        if (!animDelayTimer.finished) return;

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