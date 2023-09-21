
using OpenTK.Windowing.Desktop;

namespace bufferswap;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Press ESC to close the window.");

        var otkWinSettings = GameWindowSettings.Default;
        var otkNativeSettings = NativeWindowSettings.Default;

        otkWinSettings.UpdateFrequency = 60;

        otkNativeSettings.Title = "Buffer-Swap Test";
        otkNativeSettings.Size = (960, 540);
        otkNativeSettings.APIVersion = new Version(4, 6);

        // Debug-message callbacks using the modern 4.3+ KHR style
        // https://opentk.net/learn/appendix_opengl/debug_callback.html?tabs=debug-context-4%2Cdelegate-gl%2Cenable-gl
        //otkNativeSettings.Flags = ContextFlags.Debug;

        var win = new Win(otkWinSettings, otkNativeSettings);
        win.Focus();
        win.Run();
        win.Dispose();
    }

    public static void Fail(string message)
    {
        Console.WriteLine($"\n\nBad Things have happened:\n{message}");
        Thread.Sleep(250); // let the slow-ass console catch up
        Environment.Exit(-1);
    }
}
