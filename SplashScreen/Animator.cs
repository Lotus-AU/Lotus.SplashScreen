using Raylib_cs;

namespace Lotus.SplashScreen;

public static class Animator
{
    public static Texture2D[] BeanFrames { get; set; } = [];
    private static float travelProgress;

    public static void DrawBean()
    {
        if (BeanFrames.Length == 0) return;
        
        float secondsPerCrossing = 4f;
        float framesPerSecond = 18f;

        travelProgress = (travelProgress + Raylib.GetFrameTime() / secondsPerCrossing) % 1f;
        int frameIndex = (int)(Raylib.GetTime() * framesPerSecond) % BeanFrames.Length;
        Texture2D frame = BeanFrames[frameIndex];
        
        float beanHeight = 60;
        float beanWidth = beanHeight * frame.Width / frame.Height;

        int barWidth = 854;
        int barHeight = 50;
        int barX = (LotusSplashScreen.SplashWidth - barWidth) / 2;
        int barY = LotusSplashScreen.SplashHeight - barHeight - 25;

        float left = barX + 10;
        float right = barX + barWidth - 10 - beanWidth;

        float x = left + (right - left) * travelProgress;
        float y = barY - beanHeight + 5;

        Utilities.DrawImage(frame, x, y, beanWidth, beanHeight);
    }
}