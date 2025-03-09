using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using SyncSolutionPlugins.Model;
using static SyncSolutionPlugins.Model.EntityModel;

namespace SyncSolutionPlugins.Repository
{
    internal class SolutionRepository
    {
        private readonly IOrganizationServiceAsync2 organizationService;

        public SolutionRepository(IOrganizationServiceAsync2 organizationService)
        {
            this.organizationService = organizationService;
        }

        public Solution GetSolutionByUniqueName(string uniqueName)
        {
            var query = new QueryExpression(nameof(solution));
            query.NoLock = true;
            query.TopCount = 1;
            query.ColumnSet.AddColumns(solution.solutionid, solution.uniquename, solution.ismanaged);
            query.Criteria.AddCondition(solution.uniquename, ConditionOperator.Equal, uniqueName);

            var retrievedSolution = organizationService.RetrieveMultiple(query).Entities.FirstOrDefault();

            return new Solution(retrievedSolution);
        }
    }
}