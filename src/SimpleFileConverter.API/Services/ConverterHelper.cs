using System.Diagnostics;

namespace SimpleFileConverter.API.Services;

internal static class ConverterHelper
{
    public static Task<bool> RunCli(string exe, string args)
    {
        var tcs = new TaskCompletionSource<bool>();
        var psi = new ProcessStartInfo(exe, args)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        var process = Process.Start(psi)!;
        process.EnableRaisingEvents = true;
        process.Exited += (s, e) => tcs.SetResult(process.ExitCode == 0);
        return tcs.Task;
    }
}
