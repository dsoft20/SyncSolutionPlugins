using Greg.Xrm.Command;

namespace SyncSolutionPlugins
{
    [Command("syncsolutionplugins", HelpText = "This command syncs the plugins in a specified solution to the built version on the local machine")]
    public class SyncSolutionPluginsCommand
    {
        [Option("solutionname", "sn", HelpText = "The name of the solution. If not provided the current solution is used")]
        public string SolutionName { get; set; }
        [Option("sourcefolder", "sf", HelpText = "The path of the source folder. If not provided the directory where pacx is executed will be used")]
        public string SourceFolder { get; set; }
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