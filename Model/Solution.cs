using Greg.Xrm.Command;
using Microsoft.Xrm.Sdk;
using static SyncSolutionPlugins.Model.EntityModel;

namespace SyncSolutionPlugins.Model
{
    public class Solution
    {
        public Guid SolutionId { get; private set; }
        public string UniqueName { get; private set; }
        public bool IsManaged { get; private set; }

        public Solution(Entity entity, bool allowManaged = false)
        {
            if (entity is null)
            {
                throw new CommandException(102, "Entity is null");
            }

            if (!entity.ToEntityReference().LogicalName.Equals(nameof(solution)))
            {
                throw new CommandException(100, "The entity is not a solution entity");
            }

            this.SolutionId = entity.GetAttributeValue<Guid>(solution.solutionid);
            this.UniqueName = entity.GetAttributeValue<string>(solution.uniquename);
            this.IsManaged = entity.GetAttributeValue<bool>(solution.ismanaged);

            if (this.IsManaged && !allowManaged)
            {
                throw new CommandException(101, $"Managed solutions are not allowed");
            }
        }
    }
}
