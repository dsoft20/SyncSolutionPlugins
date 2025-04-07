using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Output;
using SyncSolutionPlugins.Repository;
using SyncSolutionPlugins.Service;

namespace SyncSolutionPlugins
{
    public class SyncSolutionPluginsCommandExecutor : ICommandExecutor<SyncSolutionPluginsCommand>
    {
        private readonly IOutput output;
        private readonly IOrganizationServiceRepository organizationServiceRepository;

        public SyncSolutionPluginsCommandExecutor(
            IOutput output,
            IOrganizationServiceRepository organizationServiceRepository)
        {
            this.output = output;
            this.organizationServiceRepository = organizationServiceRepository;
        }

        public async Task<CommandResult> ExecuteAsync(SyncSolutionPluginsCommand command, CancellationToken cancellationToken)
        {
            if (command.Preview)
            {
                output.WriteLine("Preview mode enabled, no changes will be made", ConsoleColor.Yellow);
            }

            var rebuildService = new RebuildService(command, output);

            if (!rebuildService.IsDotNetCliInstalled() && (command.Rebuild || command.RebuildAll))
            {
                return CommandResult.Fail("Dot Net CLI not installed, cannot build!");
            }

            this.output.Write($"Connecting to the current dataverse environment...");
            var crm = await this.organizationServiceRepository.GetCurrentConnectionAsync();
            this.output.WriteLine("Done", ConsoleColor.Green);

            var solutionService = new SolutionService(command, crm, this.output, this.organizationServiceRepository);
            var solutionExistCheck = await solutionService.CheckIfSolutionExists();

            if (!solutionExistCheck.Exists)
            {
                return CommandResult.Fail(solutionExistCheck.ErrorMessage);
            }

            if (!this.ManageSourceFolder(command, out string sourceFolderErrorMessage))
            {
                return CommandResult.Fail(sourceFolderErrorMessage);
            }

            if (command.RebuildAll)
            {
                if (!rebuildService.RebuildAll(out string errorMessageRebuild))
                {
                    return CommandResult.Fail(errorMessageRebuild);
                }
            }

            var pluginAssemblyRepository = new PluginRepository(crm, output, command);
            var pluginAssemblies = pluginAssemblyRepository.GetPluginsFromSolution();

            var pluginAssemblyService = new PluginUploadService(output, crm, command);
            await pluginAssemblyService.UploadPlugins(pluginAssemblies);

            if (command.Preview)
            {
                output.WriteLine("Preview mode end, no changes were made", ConsoleColor.Yellow);
            }

            return CommandResult.Success();
        }

        private bool ManageSourceFolder(SyncSolutionPluginsCommand command, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(command.Path))
            {
                this.output.WriteLine($"Source folder is empty, trying to use current directory {Directory.GetCurrentDirectory()}", ConsoleColor.Yellow);
                command.Path = Directory.GetCurrentDirectory();
            }

            if (!Directory.Exists(command.Path))
            {
                errorMessage = $"The source folder <{command.Path}> does not exist";
                return false;
            }

            return true;
        }
    }
}