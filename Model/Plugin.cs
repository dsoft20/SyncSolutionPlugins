namespace SyncSolutionPlugins.Model
{
    public class Plugin
    {
        public enum PluginType
        {
            Plugin,
            PluginPackage
        }

        public Guid PluginId { get; set; } = Guid.Empty;
        public Guid? PluginPackageId { get; set; } = Guid.Empty;
        public PluginType Type { get; set; } = PluginType.Plugin;
        public string Name { get; set; } = string.Empty;
        public bool ExistsOnFileSystem { get; set; } = false;
        public DateTime ModifiedOnFileSystem { get; set; }
        public string Path { get; set; } = string.Empty;
        public bool Uploaded { get; set; } = false;
    }
}
