using Cognite.Arb.Server.Contract;

namespace Cognite.Arb.Web.Core.Authorization.Interfaces
{
    public interface IUserProvider
    {
        User User { get; set; }
    }
}