using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DevBetterWeb.Infrastructure.Interfaces;
public interface IGetUsersHaveRolesService
{
	Task<List<IdentityUser>> ExecuteAsync();
}
