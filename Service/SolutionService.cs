using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Output;
using Microsoft.PowerPlatform.Dataverse.Client;
using SyncSolutionPlugins.Model;
using SyncSolutionPlugins.Repository;

namespace SyncSolutionPlugins.Service
{
    internal class SolutionExistCheck
    {
        public bool Exists { get; set; } = true;
        public string ErrorMessage { get; set; } = string.Empty;
        public Solution Solution { get; set; }
    }

    internal class SolutionService
    {
        private readonly SyncSolutionPluginsCommand command;
        private readonly IOrganizationServiceRepository organizationServiceRepository;
        private readonly SolutionRepository solutionRepository;
        private readonly IOutput output;

        public SolutionService(SyncSolutionPluginsCommand command, IOrganizationServiceAsync2 organizationService, IOutput output, IOrganizationServiceRepository organizationServiceRepository)
        {
            this.command = command;
            this.solutionRepository = new SolutionRepository(organizationService);
            this.output = output;
            this.organizationServiceRepository = organizationServiceRepository;
        }

        public async Task<SolutionExistCheck> CheckIfSolutionExists()
        {
            SolutionExistCheck solutionExistsRespone = new SolutionExistCheck();
            solutionExistsRespone.Solution = null;

            var errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(command.Solution))
            {
                output.WriteLine($"Solution name parameter not given, use the current default solution.", ConsoleColor.Yellow);
                command.Solution = await organizationServiceRepository.GetCurrentDefaultSolutionAsync() ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(command.Solution))
            {
                solutionExistsRespone.ErrorMessage = "Cannot get current default solution, exiting...";
                solutionExistsRespone.Exists = false;
                return solutionExistsRespone;
            }

            var solutionEntity = solutionRepository.GetSolutionByUniqueName(command.Solution);

            if (solutionEntity is null)
            {
                solutionExistsRespone.ErrorMessage = $"Cannot find a solution with name <{command.Solution}>";
                solutionExistsRespone.Exists = false;
                return solutionExistsRespone;
            }

            solutionExistsRespone.Solution = solutionEntity;
            output.WriteLine($"Solution <{solutionExistsRespone.Solution.UniqueName}> found with Id {solutionExistsRespone.Solution.SolutionId}", ConsoleColor.Green);

            return solutionExistsRespone;
        }
    }
}