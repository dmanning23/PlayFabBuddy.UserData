using PlayFab.ClientModels;
using PlayFabBuddyLib.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlayFabBuddyLib.UserData
{
	public class PlayFabUserDataService : IPlayFabUserDataService
	{
		#region Properties

		IPlayFabClient _playfab;
		IPlayFabAuthService _auth;

		#endregion //Properties

		#region Methods

		public PlayFabUserDataService(IPlayFabClient playfab, IPlayFabAuthService auth)
		{
			_playfab = playfab;
			_auth = auth;
		}

		public async Task<Dictionary<string, string>> GetUserData(List<string> keys)
		{
			var result = await _playfab.GetUserDataAsync(new GetUserDataRequest()
			{
				PlayFabId = _auth.PlayFabId,
				Keys = keys,
			});

			var data = new Dictionary<string, string>();
			if (null == result.Error)
			{
				foreach (var dataValue in result.Result.Data)
				{
					data.Add(dataValue.Key, dataValue.Value.Value);
				}
			}
			return data;
		}

		/// <summary>
		/// Get the current logged in user's publisher data
		/// </summary>
		/// <returns></returns>
		public async Task<Dictionary<string, string>> GetUserPublisherData(List<string> keys)
		{
			var result = await _playfab.GetUserPublisherDataAsync(new GetUserDataRequest()
			{
				PlayFabId = _auth.PlayFabId,
				Keys = keys,
			});

			var data = new Dictionary<string, string>();
			if (null == result.Error)
			{
				foreach (var dataValue in result.Result.Data)
				{
					data.Add(dataValue.Key, dataValue.Value.Value);
				}
			}
			return data;
		}

		/// <summary>
		/// Set the current logged in user's data
		/// </summary>
		public async Task<string> SetUserData(Dictionary<string, string> data)
		{
			var result = await _playfab.UpdateUserDataAsync(new UpdateUserDataRequest()
			{
				Data = data
			});

			return result.Error?.ErrorMessage ?? string.Empty;
		}

		/// <summary>
		/// Set the current logged in user's publisher data
		/// </summary>
		/// <param name="displayName"></param>
		/// <returns>Empty if successul, the error message if something went wrong</returns>
		public async Task<string> SetUserPublisherData(Dictionary<string, string> data)
		{
			var result = await _playfab.UpdateUserPublisherDataAsync(new UpdateUserDataRequest()
			{
				Data = data
			});

			return result.Error?.ErrorMessage ?? string.Empty;
		}

		#endregion //Methods
	}
}
