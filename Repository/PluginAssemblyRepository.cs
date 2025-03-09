using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Output;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using SyncSolutionPlugins.Model;
using static SyncSolutionPlugins.Model.EntityModel;

namespace SyncSolutionPlugins.Repository
{
    internal class PluginAssemblyRepository
    {
        private readonly IOrganizationServiceAsync2 organizationService;
        private readonly SyncSolutionPluginsCommand command;
        private readonly IOutput output;

        public PluginAssemblyRepository(IOrganizationServiceAsync2 organizationService, IOutput output, SyncSolutionPluginsCommand command)
        {
            this.organizationService = organizationService;
            this.output = output;
            this.command = command;
        }

        public List<PluginAssembly> GetPluginsFromSolution()
        {
            var query = new QueryExpression(nameof(solution));
            query.NoLock = true;
            query.ColumnSet.AddColumns(solution.solutionid, solution.uniquename, solution.ismanaged);

            query.Criteria.AddCondition(solution.uniquename, ConditionOperator.Equal, this.command.SolutionName);

            var solutionComponentLink = query.AddLink(nameof(solutioncomponent), solution.solutionid, solutioncomponent.solutionid);
            solutionComponentLink.EntityAlias = nameof(solutioncomponent);
            solutionComponentLink.LinkCriteria.AddCondition(solutioncomponent.componenttype, ConditionOperator.Equal, 91);

            var pluginAssemblyLink = solutionComponentLink.AddLink(nameof(pluginassembly), solutioncomponent.objectid, pluginassembly.pluginassemblyid);
            pluginAssemblyLink.EntityAlias = nameof(pluginassembly);
            pluginAssemblyLink.Columns.AddColumns(pluginassembly.name, pluginassembly.pluginassemblyid);

            var results = organizationService.RetrieveMultiple(query).Entities.ToList();

            var pluginAssemblies = new List<PluginAssembly>();

            foreach (var result in results)
            {
                var pluginAssembly = new PluginAssembly();
                pluginAssembly.Name = result.GetAliasedValue<string>(pluginassembly.name, nameof(pluginassembly));
                pluginAssembly.PluginAssemblyId = result.GetAliasedValue<Guid>(pluginassembly.pluginassemblyid, nameof(pluginassembly));

                pluginAssemblies.Add(pluginAssembly);
            }

            output.WriteLine($"Found {pluginAssemblies.Count} plugin assemblies in solution <{this.command.SolutionName}>", ConsoleColor.Green);

            var allPluginsExistsOnFileSystem = CheckIfPluginAssembliesExists(pluginAssemblies);
            output.WriteTable(pluginAssemblies, () => ["Name", "Id", "Exists [Filesystem]", "Modified date [Filesystem]"], x => [x.Name, x.PluginAssemblyId.ToString(), x.ExistsOnFileSystem.ToString(), x.ExistsOnFileSystem ? x.ModifiedOnFileSystem.ToString() : "N/A"]);

            if (!allPluginsExistsOnFileSystem && !command.AllowPartialUpdaters)
            {
                throw new CommandException(103, "Some plugin assemblies not found on file system, exiting");
            }

            return pluginAssemblies;
        }

        private bool CheckIfPluginAssembliesExists(List<PluginAssembly> pluginAssemblies)
        {
            for (int i = 0; i < pluginAssemblies.Count; i++)
            {
                var pluginPath = Path.Combine(this.command.SourceFolder, pluginAssemblies[i].Name, "bin", "Debug", $"{pluginAssemblies[i].Name}.dll");
                if (!File.Exists(pluginPath))
                {
                    return false;
                }

                pluginAssemblies[i].ExistsOnFileSystem = true;
                pluginAssemblies[i].ModifiedOnFileSystem = File.GetLastWriteTime(pluginPath);
                pluginAssemblies[i].Path = pluginPath;
            }

            return true;
        }
    }
}