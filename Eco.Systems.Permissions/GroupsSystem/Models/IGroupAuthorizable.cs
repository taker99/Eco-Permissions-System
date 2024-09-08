using Eco.Shared.Serialization;
using System;

namespace Eco.Systems.Permissions.Groups
{
    public interface IGroupAuthorizable
    {
        string Identifier { get; }
        bool BlackListed { get; set; }
        bool Permit(SimpleGroupUser user) => BlackListed;
    }
}
