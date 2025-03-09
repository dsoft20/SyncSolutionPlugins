namespace SyncSolutionPlugins.Model
{
    internal class PluginAssembly
    {
        public Guid PluginAssemblyId { get; set; } = Guid.Empty;
        public string Name { get; set; } = string.Empty;
        public bool ExistsOnFileSystem { get; set; } = false;
        public DateTime ModifiedOnFileSystem { get; set; }
        public string Path { get; set; } = string.Empty;
        public bool Uploaded { get; set; } = false;
    }
}
