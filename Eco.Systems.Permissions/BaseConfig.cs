using Eco.Shared.Localization;

namespace Eco.Systems.Permissions
{
   public class BaseConfig
    {
        [LocDisplayName("Wipe Groups Data file on New World")]
        [LocDescription("Determine if the Groups File should be wiped on each world reset or not (You will lose all groups and assigned commands in those groups)")]
        public bool WipeGroupsFileOnFreshWorld { get; set; } = false;
    }
}