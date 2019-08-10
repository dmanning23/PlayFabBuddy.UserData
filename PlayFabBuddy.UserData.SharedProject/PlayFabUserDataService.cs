using PlayFab;
using PlayFab.ClientModels;
using PlayFabBuddyLib.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlayFabBuddyLib.UserData
{
	public class PlayFabUserDataService : IPlayFabUserDataService
	{
		#region Properties

		protected Dictionary<string, string> TitleData { get; set; }

		protected Dictionary<string, string> UserData { get; set; }

		protected Dictionary<string, string> UserPublisherData { get; set; }

		IPlayFabClient _playfab;
		IPlayFabAuthService _auth;

		#endregion //Properties

		#region Methods

		public PlayFabUserDataService(IPlayFabClient playfab, IPlayFabAuthService auth)
		{
			UserData = new Dictionary<string, string>();
			UserPublisherData = new Dictionary<string, string>();
			TitleData = new Dictionary<string, string>();

			_playfab = playfab;
			_auth = auth;
		}

		public async Task<Dictionary<string, string>> GetTitleData(List<string> keys)
		{
			//Check which keys need to be updated
			var data = new Dictionary<string, string>();
			var keysToCheck = GetOutgoingKeys(keys, TitleData, data, false);

			//Call out to playfab for any items we need to retrieve
			var result = await _playfab.GetTitleDataAsync(new GetTitleDataRequest()
			{
				Keys = keysToCheck,
			});

			//Store the results in the cache and return the completed list
			return PopulateData(result, TitleData, data);
		}

		public async Task<Dictionary<string, string>> GetUserData(List<string> keys, bool force = false)
		{
			var data = new Dictionary<string, string>();
			var keysToCheck = GetOutgoingKeys(keys, UserData, data, false);

			var result = await _playfab.GetUserDataAsync(new GetUserDataRequest()
			{
				PlayFabId = _auth.PlayFabId,
				Keys = keysToCheck,
			});

			return PopulateData(result, UserData, data);
		}

		/// <summary>
		/// Get the current logged in user's publisher data
		/// </summary>
		/// <returns></returns>
		public async Task<Dictionary<string, string>> GetUserPublisherData(List<string> keys, bool force = false)
		{
			var data = new Dictionary<string, string>();
			var keysToCheck = GetOutgoingKeys(keys, UserPublisherData, data, false);

			var result = await _playfab.GetUserPublisherDataAsync(new GetUserDataRequest()
			{
				PlayFabId = _auth.PlayFabId,
				Keys = keysToCheck,
			});

			return PopulateData(result, UserPublisherData, data);
		}

		/// <summary>
		/// Set the current logged in user's data
		/// </summary>
		public async Task<string> SetUserData(Dictionary<string, string> data)
		{
			foreach (var dataValue in data)
			{
				UserData[dataValue.Key] = dataValue.Value;
			}

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
			foreach (var dataValue in data)
			{
				UserPublisherData[dataValue.Key] = dataValue.Value;
			}

			var result = await _playfab.UpdateUserPublisherDataAsync(new UpdateUserDataRequest()
			{
				Data = data
			});

			return result.Error?.ErrorMessage ?? string.Empty;
		}

		protected List<string> GetOutgoingKeys(List<string> inputKeys, Dictionary<string, string> cache, Dictionary<string, string> output, bool force)
		{
			//if forcing it, we want to pull all the values from the backend.
			if (force)
			{
				return inputKeys;
			}

			var outgoingKeys = new List<string>();

			foreach (var key in inputKeys)
			{
				if (cache.ContainsKey(key))
				{
					output.Add(key, cache[key]);
				}
				else
				{
					outgoingKeys.Add(key);
				}
			}

			return outgoingKeys;
		}

		protected Dictionary<string, string> PopulateData(PlayFabResult<GetUserDataResult> result, Dictionary<string, string> cache, Dictionary<string, string> data)
		{
			if (null == result.Error)
			{
				foreach (var dataValue in result.Result.Data)
				{
					cache[dataValue.Key] = dataValue.Value.Value;
					data[dataValue.Key] = dataValue.Value.Value;
				}
			}
			return data;
		}

		protected Dictionary<string, string> PopulateData(PlayFabResult<GetTitleDataResult> result, Dictionary<string, string> cache, Dictionary<string, string> data)
		{
			if (null == result.Error)
			{
				foreach (var dataValue in result.Result.Data)
				{
					cache[dataValue.Key] = dataValue.Value;
					data[dataValue.Key] = dataValue.Value;
				}
			}
			return data;
		}

		#endregion //Methods
	}
}
