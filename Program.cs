using Raylib_cs;
using static Raylib_cs.Raylib;
using Tests;
static class Program
{
    public static int Main()
    {
        Tests.AnimationScene.Test("dancer_low", 10, 24, 10);
        
        return 0;
    }
}