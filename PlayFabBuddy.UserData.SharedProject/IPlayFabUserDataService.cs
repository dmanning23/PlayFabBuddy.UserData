using PlayFab.ClientModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlayFabBuddyLib.UserData
{
	public interface IPlayFabUserDataService
	{
		Dictionary<string, string> UserData { get; }

		Dictionary<string, string> UserPublisherData { get; }

		/// <summary>
		/// Get the current logged in user's data
		/// </summary>
		Task<Dictionary<string, string>> GetUserData(List<string> keys);

		/// <summary>
		/// Get the current logged in user's publisher data
		/// </summary>
		/// <returns></returns>
		Task<Dictionary<string, string>> GetUserPublisherData(List<string> keys);

		/// <summary>
		/// Set the current logged in user's data
		/// </summary>
		/// <param name="displayName"></param>
		/// <returns>Empty if successul, the error message if something went wrong</returns>
		Task<string> SetUserData(Dictionary<string, string> data);

		/// <summary>
		/// Set the current logged in user's publisher data
		/// </summary>
		/// <param name="displayName"></param>
		/// <returns>Empty if successul, the error message if something went wrong</returns>
		Task<string> SetUserPublisherData(Dictionary<string, string> data);
	}
}
