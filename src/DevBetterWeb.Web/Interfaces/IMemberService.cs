using System.Collections.Generic;
using System.Threading.Tasks;
using DevBetterWeb.Core.Entities;

namespace DevBetterWeb.Web.Interfaces;

/// <summary>
/// Interface for the MemberService, providing methods for managing members.
/// </summary>
public interface IMemberService
{
	/// <summary>
	/// Gets a list of all alumni members asynchronously.
	/// </summary>
	/// <returns>A list of all alumni members.</returns>
	Task<List<Member>> GetAlumniMembersAsync();

	/// <summary>
	/// Gets a list of all active alumni members asynchronously.
	/// </summary>
	/// <returns>A list of all active alumni members.</returns>
	Task<List<Member>> GetActiveAlumniMembersAsync();

	/// <summary>
	/// Gets a list of all members asynchronously.
	/// </summary>
	/// <returns>A list of all members.</returns>
	Task<List<Member>> GetMembersAsync();

	/// <summary>
	/// Gets a list of all active members asynchronously.
	/// </summary>
	/// <returns>A list of all active members.</returns>
	Task<List<Member>> GetActiveMembersAsync();
}
