using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Output;
using Microsoft.PowerPlatform.Dataverse.Client;
using SyncSolutionPlugins.Interfaces;
using SyncSolutionPlugins.Model;
using SyncSolutionPlugins.Strategy;

namespace SyncSolutionPlugins.Service
{
    internal class PluginUploadService
    {
        private readonly SyncSolutionPluginsCommand command;
        private readonly IOrganizationServiceAsync2 organizationService;
        private readonly IOutput output;
        private readonly RebuildService rebuildService;
        private readonly Dictionary<Plugin.PluginType, IPluginUpload> pluginUploadStrategies = new Dictionary<Plugin.PluginType, IPluginUpload>();

        public PluginUploadService(IOutput output, IOrganizationServiceAsync2 organizationService, SyncSolutionPluginsCommand command)
        {
            this.command = command;
            this.organizationService = organizationService;
            this.output = output;
            rebuildService = new RebuildService(command, output);

            pluginUploadStrategies.Add(Plugin.PluginType.Plugin, new UploadPluginAssembly());
            pluginUploadStrategies.Add(Plugin.PluginType.PluginPackage, new UploadPluginPackage());
        }

        public async Task<CommandResult> UploadPlugins(List<Plugin> pluginAssemblies)
        {
            RebuildPlugins(pluginAssemblies);

            foreach (var plugin in pluginAssemblies)
            {
                var uploaded = await pluginUploadStrategies[plugin.Type].UploadPlugin(plugin, command, organizationService, output);

                if (!uploaded.IsSuccess && !command.AllowPartialUpdaters)
                {
                    CommandResult.Fail($"Failed to update {plugin.Name}");
                }
            }

            output.WriteLine($"Execution results:");

            output.WriteTable(pluginAssemblies, () => ["Name", "Id", "Uploaded"], x => [x.Name, x.PluginId.ToString(), x.Uploaded.ToString()]);

            return CommandResult.Success();
        }

        private void RebuildPlugins(List<Plugin> pluginAssemblies)
        {
            if (!command.Rebuild)
            {
                return;
            }

            foreach (var pluginAssembly in pluginAssemblies)
            {
                rebuildService.RebuildPlugin(pluginAssembly.Name, out string errorMessage);
            }
        }
    }
}