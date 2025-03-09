using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Output;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using SyncSolutionPlugins.Model;
using static SyncSolutionPlugins.Model.EntityModel;

namespace SyncSolutionPlugins.Service
{
    internal class PluginAssemblyUploadService
    {
        private readonly SyncSolutionPluginsCommand command;
        private readonly IOrganizationServiceAsync2 organizationService;
        private readonly IOutput output;
        private readonly RebuildService rebuildService;

        public PluginAssemblyUploadService(IOutput output, IOrganizationServiceAsync2 organizationService, SyncSolutionPluginsCommand command)
        {
            this.command = command;
            this.organizationService = organizationService;
            this.output = output;
            rebuildService = new RebuildService(command, output);
        }

        public async Task<CommandResult> UploadPlugins(List<PluginAssembly> pluginAssemblies)
        {
            RebuildPlugins(pluginAssemblies);

            foreach (var plugin in pluginAssemblies)
            {
                var uploaded = await this.UploadPluginAssembly(plugin);

                if (!uploaded.IsSuccess && !command.AllowPartialUpdaters)
                {
                    CommandResult.Fail($"Failed to update {plugin.Name}");
                }
            }

            output.WriteTable(pluginAssemblies, () => ["Name", "Id", "Uploaded"], x => [x.Name, x.PluginAssemblyId.ToString(), x.Uploaded.ToString()]);

            return CommandResult.Success();
        }

        private void RebuildPlugins(List<PluginAssembly> pluginAssemblies)
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

        private async Task<CommandResult> UploadPluginAssembly(PluginAssembly pluginAssembly)
        {
            try
            {
                var plugin = new Entity(nameof(pluginassembly), pluginAssembly.PluginAssemblyId);
                var pluginBytes = await File.ReadAllBytesAsync(pluginAssembly.Path);

                plugin[pluginassembly.content] = Convert.ToBase64String(pluginBytes);
                

                output.WriteLine($"Uploading {pluginAssembly.Path} to system{Environment.NewLine}");
                await organizationService.UpdateAsync(plugin);

                pluginAssembly.Uploaded = true;
            }
            catch (Exception ex)
            {
                return CommandResult.Fail($"Cannot upload {""} to system.{Environment.NewLine}{ex.Message}");
            }

            return CommandResult.Success();
        }
    }
}