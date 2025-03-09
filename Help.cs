using Greg.Xrm.Command.Parsing;
using Greg.Xrm.Command.Services;

namespace SyncSolutionPlugins
{
    public class Help : INamespaceHelper
    {
        private readonly string help = "Sync local plugin assemblies to solution";

        public string[] Verbs { get; } = new string[1] { "syncsolutionplugins" };

        public Help() { }

        public string GetHelp()
        {
            return this.help;
        }

        public void WriteHelp(MarkdownWriter writer)
        {
            writer.WriteParagraph(this.GetHelp());
        }
    }
}