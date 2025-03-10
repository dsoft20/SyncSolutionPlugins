using Greg.Xrm.Command;

namespace SyncSolutionPlugins
{
    [Command("dstPlugin","syncsolutionplugins", HelpText = "This command syncs the plugins in a specified solution to the built version on the local machine")]
    [Alias("dst", "sync")]
    public class SyncSolutionPluginsCommand
    {
        [Option("solution", "s", HelpText = "The name of the solution. If not provided the current solution is used")]
        public string Solution { get; set; }
        [Option("path", "p", HelpText = "The path of the source folder. If not provided the directory where pacx is executed will be used")]
        public string Path { get; set; }
        [Option("preview", "pr", HelpText = "Shows a preview of the operations")]
        public bool Preview { get; set; }
        [Option("allowpartialsync", "aps", HelpText = "Continue execution if an error occours while updating a plugin assembly", DefaultValue = false)]
        public bool AllowPartialUpdaters { get; set; }
        [Option("rebuild", "r", HelpText = "[Experimental] Rebuild the plugin project", DefaultValue = false)]
        public bool Rebuild { get; set; }
        [Option("rebuildall", "ra", HelpText = "[Experimental] Rebuild all project", DefaultValue = false)]
        public bool RebuildAll { get; set; }
        [Option("rebuildlogonsuccess", "rl", HelpText = "[Experimental] Show log of rebuild proecesses even when the process succeds", DefaultValue = false)]
        public bool RebuildLogOnSuccess { get; set; }
    }
}