using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Output;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using SyncSolutionPlugins.Interfaces;
using SyncSolutionPlugins.Model;
using static SyncSolutionPlugins.Model.EntityModel;

namespace SyncSolutionPlugins.Strategy
{
    internal class UploadPluginPackage : IPluginUpload
    {
        public async Task<CommandResult> UploadPlugin(Plugin pluginToLoad, SyncSolutionPluginsCommand command, IOrganizationServiceAsync2 organizationService, IOutput output)
        {
            try
            {
                var pluginPackage = new Entity(nameof(pluginpackage), pluginToLoad.PluginPackageId.Value);
                var pluginBytes = await File.ReadAllBytesAsync(pluginToLoad.Path);

                pluginPackage[pluginassembly.content] = Convert.ToBase64String(pluginBytes);

                output.Write($"{Environment.NewLine}Uploading {pluginToLoad.Name} to system...");

                if (!command.Preview)
                {
                    await organizationService.UpdateAsync(pluginPackage);
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
