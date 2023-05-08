using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevBetterWeb.Infrastructure.Interfaces;
public interface INonCurrentMembersService
{
	/// <summary>
	/// Retrieves a list of user IDs without member, alumni, and administrator roles.
	/// </summary>
	/// <returns>A list of user IDs without the specified roles.</returns>
	Task<List<string>> GetUsersIdsWithoutRolesAsync();

	/// <summary>
	/// Retrieves a list of non-current member IDs based on the provided list of user IDs without a member role.
	/// </summary>
	/// <param name="usersIdWithoutMemberRole">A list of user IDs without a member role.</param>
	/// <param name="cancellationToken">An optional token to cancel the operation.</param>
	/// <returns>A list of non-current member IDs.</returns>
	Task<List<int>> GetNonCurrentMembersAsync(List<string> usersIdWithoutMemberRole,
		CancellationToken cancellationToken = default);
}
