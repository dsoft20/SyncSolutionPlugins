using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Output;
using Microsoft.PowerPlatform.Dataverse.Client;
using SyncSolutionPlugins.Model;

namespace SyncSolutionPlugins.Interfaces
{
    public interface IPluginUpload
    {
        Task<CommandResult> UploadPlugin(Plugin pluginToLoad, SyncSolutionPluginsCommand command, IOrganizationServiceAsync2 organizationService, IOutput output);
    }
}
