using System.Diagnostics;
using Raylib_cs;

namespace Lotus.SplashScreen;

public static class LotusSplashScreen
{
    public const int SplashWidth = 925;
    public const int SplashHeight = 550;

    public static string GameDir = string.Empty;
    
    [STAThread]
    public static void Main(params string[] args)
    {
        Raylib.SetConfigFlags(ConfigFlags.UndecoratedWindow | ConfigFlags.TransparentWindow | ConfigFlags.TopmostWindow);
        Raylib.InitWindow(SplashWidth, SplashHeight, "Project Lotus Splash Screen");
        Raylib.SetTargetFPS(60);
        Raylib.InitAudioDevice();
        
        double startedAt = Raylib.GetTime();
        bool connectedToGame = args.Length > 0;

        try
        {
            Textures.Load();
            bool wasWebsiteHovered = false;
            bool wasDiscordHovered = false;
            bool wasOpenGameDirHovered = false;

            if (connectedToGame)
            {
                ThreadLogReader.Start();
                SetGameDir(args);
            }

            while (!Raylib.WindowShouldClose())
            {
                ThreadLogReader.Update();
                
                if (connectedToGame && (ThreadLogReader.Closed || StateManager.ShouldClose)) break;
                
                #if DEBUG
                if (!connectedToGame)
                    CheckDebugKeybinds();
                #endif
                
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Blank);

                Utilities.DrawImage(Textures.SplashBackground, 0, 0, SplashWidth, SplashHeight);

                #if !DEBUG
                if (!connectedToGame)
                {
                    Raylib.DrawText("This is a splash screen for Project Lotus. It is not meant to be run directly.\nThe process will exit in 10s, please open Among Us.", 80, 225, 20, Color.Red);
                    Raylib.EndDrawing();
                    if (Raylib.GetTime() - startedAt >= 10)
                        break;

                    continue;
                }
                #endif
                
                Raylib.DrawTexture(Textures.Logo, SplashWidth - Textures.Logo.Width - 100, 80, Color.White);

                Utilities.DrawButton(Textures.WebsiteInactive, Textures.WebsiteActive, new Rectangle(30, 30, 100, 100), "https://lotusau.top", ref wasWebsiteHovered);
                Utilities.DrawButton(Textures.DiscordInactive, Textures.DiscordActive, new Rectangle(130, 30, 100, 100), "https://discord.gg/projectlotus", ref wasDiscordHovered);
                if (Directory.Exists(GameDir)) Utilities.DrawButton(Textures.OpenGameDirInactive, Textures.OpenGameDirActive, new Rectangle(230, 30, 100, 100), GameDir, ref wasOpenGameDirHovered);
                
                Utilities.DrawCheckpoints();
                Utilities.DrawLoadingBar(StateManager.GetLoadingBarProgress());
                Animator.DrawBean();

                Raylib.EndDrawing();
            }
        }
        finally
        {
            Textures.Unload();
            Raylib.CloseAudioDevice();
            Raylib.CloseWindow();
        }
    }
    
    private static void CheckDebugKeybinds()
    {
        #if !DEBUG
        return;
        #endif
        float change = 0.5f * Raylib.GetFrameTime();

        if (Raylib.IsKeyDown(KeyboardKey.Up))
            StateManager.SetLoadingBarProgress(
                StateManager.GetLoadingBarProgress() + change);

        if (Raylib.IsKeyDown(KeyboardKey.Down))
            StateManager.SetLoadingBarProgress(
                StateManager.GetLoadingBarProgress() - change);

        int step = 0;

        if (Raylib.IsKeyPressed(KeyboardKey.Right)) step = 1;
        if (Raylib.IsKeyPressed(KeyboardKey.Left)) step = -1;
        if (step == 0) return;

        int index = Math.Clamp((int)StateManager.CurrentState + step, (int)SplashScreenState.ChainloaderStarted, (int)SplashScreenState.Done);
        var state = (SplashScreenState)index;
        StateManager.SetDebugState(state);
        
        int processed = state switch
        {
            SplashScreenState.ChainloaderStarted => 0,
            SplashScreenState.LoadingPlugins => 1,
            SplashScreenState.LoadingVF => 2,
            SplashScreenState.LoadingLotus => 3,
            SplashScreenState.LoadingAddons => 4,
            SplashScreenState.Done => 5,
            _ => throw new ArgumentOutOfRangeException()
        };

        StateManager.SetPluginProgress(processed, 2);
    }
    
    public static void SetGameDir(string[] args)
    {
        var pid = int.Parse(args.Last());
        var gameProcess = Process.GetProcessById(pid);
        
        if (gameProcess == null)
            throw new Exception("Could not find Among Us process. Make sure to launch the game first.");

        GameDir = Path.GetDirectoryName(gameProcess.MainModule?.FileName) ?? throw new Exception("Could not determine Among Us directory.");
    }
}