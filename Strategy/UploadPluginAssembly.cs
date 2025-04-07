using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Output;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using SyncSolutionPlugins.Interfaces;
using SyncSolutionPlugins.Model;
using static SyncSolutionPlugins.Model.EntityModel;

namespace SyncSolutionPlugins.Strategy
{
    internal class UploadPluginAssembly : IPluginUpload
    {
        public async Task<CommandResult> UploadPlugin(Plugin pluginToLoad, SyncSolutionPluginsCommand command, IOrganizationServiceAsync2 organizationService, IOutput output)
        {
            try
            {
                var plugin = new Entity(nameof(pluginassembly), pluginToLoad.PluginId);
                var pluginBytes = await File.ReadAllBytesAsync(pluginToLoad.Path);

                plugin[pluginassembly.content] = Convert.ToBase64String(pluginBytes);

                output.Write($"{Environment.NewLine}Uploading {pluginToLoad.Name} to system...");

                if (!command.Preview)
                {
                    await organizationService.UpdateAsync(plugin);
                }

                output.Write($"Done{Environment.NewLine}", ConsoleColor.Green);
                pluginToLoad.Uploaded = true;
            }
            catch (Exception ex)
            {
                output.Write($"Failed{Environment.NewLine}", ConsoleColor.Green);
                return CommandResult.Fail($"Cannot upload {pluginToLoad.Name} to system.{Environment.NewLine}{ex.Message}");
            }

            return CommandResult.Success();
        }


    }
}
