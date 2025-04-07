using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Output;
using System.Diagnostics;

namespace SyncSolutionPlugins.Service
{
    internal class RebuildService
    {
        private readonly SyncSolutionPluginsCommand command;
        private readonly IOutput output;

        public RebuildService(SyncSolutionPluginsCommand command, IOutput output)
        {
            this.command = command;
            this.output = output;
        }

        public bool IsDotNetCliInstalled()
        {
            output.Write($"{Environment.NewLine}Checking if dotnet cli is installed...");
            var exists = LaunchProcess("dotnet", new List<string>(), true);

            output.Write($"Done{Environment.NewLine}", exists ? ConsoleColor.Green : ConsoleColor.Red);

            return exists;
        }

        public bool RebuildAll(out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                output.Write($"{Environment.NewLine}Trying to rebuild all projects...");

                if (!LaunchProcess("dotnet", new List<string> { "build", "--no-incremental" }))
                {
                    throw new CommandException(104, $"Build failed!");
                }

                output.Write($"Done{Environment.NewLine}", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }

            return true;
        }

        public bool RebuildPlugin(string pluginName, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!command.Rebuild)
            {
                return true;
            }

            try
            {
                string path = Path.Combine(command.Path, pluginName);
                output.Write($"{Environment.NewLine}Rebuild <{pluginName}> from folder {path}...");

                if (!LaunchProcess("dotnet", new List<string> { "build", path, "--no-incremental" }))
                {
                    throw new CommandException(104, $"Build failed!");
                }

                output.Write($"Done{Environment.NewLine}", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                errorMessage = $"{Environment.NewLine} {ex.Message}";
                return false;
            }

            return true;
        }

        private bool LaunchProcess(string executable, List<string> args, bool doNotWriteStandardOutput = false)
        {
            try
            {
                if (command.Preview)
                {
                    return true;
                }

                var processStartInfo = new ProcessStartInfo(executable, args);
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardOutput = true;

                var process = Process.Start(processStartInfo);
                var logOutput = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (!doNotWriteStandardOutput && (command.RebuildLogOnSuccess || process.ExitCode != 0))
                {
                    output.WriteLine($"{Environment.NewLine}{logOutput}");
                }

                if (process.ExitCode == 1)
                {
                    return false;
                }

                return process.ExitCode != -1;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}