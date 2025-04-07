using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Output;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Organization;
using Microsoft.Xrm.Sdk.Query;
using SyncSolutionPlugins.Model;
using System.Xml.Linq;
using static SyncSolutionPlugins.Model.EntityModel;

namespace SyncSolutionPlugins.Repository
{
    internal class PluginRepository
    {
        private readonly IOrganizationServiceAsync2 organizationService;
        private readonly SyncSolutionPluginsCommand command;
        private readonly IOutput output;

        public PluginRepository(IOrganizationServiceAsync2 organizationService, IOutput output, SyncSolutionPluginsCommand command)
        {
            this.organizationService = organizationService;
            this.output = output;
            this.command = command;
        }

        public List<Plugin> GetPluginsFromSolution()
        {
            //            select pa.name from solution as s with(nolock)
            //            inner join solutioncomponent as sc ON s.solutionid = sc.solutionid
            //            inner join pluginassembly as pa ON sc.objectid = pa.pluginassemblyid
            //            left outer join pluginpackage as pp ON pa.packageid = pp.pluginpackageid
            //            where s.uniquename = 'test_sync' and sc.componenttype = 91
            var query = new QueryExpression(nameof(solution));
            query.NoLock = true;
            query.ColumnSet.AddColumns(solution.solutionid, solution.uniquename, solution.ismanaged);

            query.Criteria.AddCondition(solution.uniquename, ConditionOperator.Equal, this.command.Solution);

            var solutionComponentLink = query.AddLink(nameof(solutioncomponent), solution.solutionid, solutioncomponent.solutionid);
            solutionComponentLink.EntityAlias = nameof(solutioncomponent);
            solutionComponentLink.LinkCriteria.AddCondition(solutioncomponent.componenttype, ConditionOperator.Equal, 91);

            var pluginAssemblyLink = solutionComponentLink.AddLink(nameof(pluginassembly), solutioncomponent.objectid, pluginassembly.pluginassemblyid);
            pluginAssemblyLink.EntityAlias = nameof(pluginassembly);
            pluginAssemblyLink.Columns.AddColumns(pluginassembly.name, pluginassembly.pluginassemblyid, pluginassembly.packageid);

            var results = organizationService.RetrieveMultiple(query).Entities.ToList();
            var plugins = new List<Plugin>();

            foreach (var result in results)
            {
                var pluginAssembly = new Plugin();

                pluginAssembly.Name = result.GetAliasedValue<string>(pluginassembly.name, nameof(pluginassembly));
                pluginAssembly.PluginId = result.GetAliasedValue<Guid>(pluginassembly.pluginassemblyid, nameof(pluginassembly));

                var packageEntityReference = result.GetAliasedValue<EntityReference>(pluginassembly.packageid, nameof(pluginassembly));
                if (packageEntityReference is not null)
                {
                    pluginAssembly.PluginPackageId = packageEntityReference.Id;
                    pluginAssembly.Type = Plugin.PluginType.PluginPackage;
                }

                plugins.Add(pluginAssembly);
            }

            output.WriteLine($"Found {plugins.Count} plugin assemblies in solution <{this.command.Solution}>", ConsoleColor.Green);

            var allPluginsExistsOnFileSystem = CheckIfPluginAssembliesExists(plugins);
            output.WriteTable(plugins, () => ["Name", "Id", "Type", "File exists", "File modified on"], x => [x.Name, x.PluginId.ToString(), x.Type.ToString(), x.ExistsOnFileSystem.ToString(), x.ExistsOnFileSystem ? x.ModifiedOnFileSystem.ToString() : "N/A"]);

            if (!allPluginsExistsOnFileSystem && !command.AllowPartialUpdaters)
            {
                throw new CommandException(103, "Some plugin assemblies not found on file system, exiting");
            }

            return plugins;
        }

        private bool CheckIfPluginAssembliesExists(List<Plugin> pluginAssemblies)
        {
            bool allExists = true;

            for (int i = 0; i < pluginAssemblies.Count; i++)
            {
                allExists = false;
                switch (pluginAssemblies[i].Type)
                {
                    case Plugin.PluginType.Plugin:
                        allExists = CheckIfPluginAssemblyExists(pluginAssemblies[i]);
                        break;
                    case Plugin.PluginType.PluginPackage:
                        allExists = CheckIfPluginPackageExists(pluginAssemblies[i]);
                        break;
                }

                if (!allExists)
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckIfPluginPackageExists(Plugin pluginAssembly)
        {
            var pluginPackagePath = Path.Combine(this.command.Path, pluginAssembly.Name, "bin", "Debug");
            var enumerateFiles = Directory.EnumerateFiles(pluginPackagePath, "*.nupkg").ToList();

            if (enumerateFiles.Count == 0)
            {
                return false;
            }

            pluginPackagePath = enumerateFiles[0];
            pluginAssembly.ExistsOnFileSystem = true;
            pluginAssembly.ModifiedOnFileSystem = File.GetLastWriteTime(pluginPackagePath);
            pluginAssembly.Path = pluginPackagePath;
            return true;
        }

        private bool CheckIfPluginAssemblyExists(Plugin plugin)
        {
            var pluginPath = Path.Combine(this.command.Path, plugin.Name, "bin", "Debug", $"{plugin.Name}.dll");

            if (!File.Exists(pluginPath))
            {
                return false;
            }

            plugin.ExistsOnFileSystem = true;
            plugin.ModifiedOnFileSystem = File.GetLastWriteTime(pluginPath);
            plugin.Path = pluginPath;

            return true;
        }
    }
}