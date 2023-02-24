using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Tests
{
public static class AssetManager
{
    static Dictionary<string, Model> cachedModels = new Dictionary<string, Model>();
    static Dictionary<string, Texture2D> cachedTextures = new Dictionary<string, Texture2D>();

    public static Model LoadModel(string path)
    {
        if (cachedModels.ContainsKey(path))
            return cachedModels[path];
        
        Model model = Raylib.LoadModel(path);
        cachedModels.Add(path, model);
        return model;
    }

    public static Texture2D LoadTexture(string path)
    {
        if (cachedTextures.ContainsKey(path))
            return cachedTextures[path];
        
        Texture2D texture = Raylib.LoadTexture(path);
        cachedTextures.Add(path, texture);
        return texture;
    }


}
}