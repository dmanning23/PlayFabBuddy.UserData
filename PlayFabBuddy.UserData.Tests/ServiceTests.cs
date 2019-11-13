using FakeItEasy;
using NUnit.Framework;
using PlayFab;
using PlayFab.ClientModels;
using PlayFabBuddyLib;
using PlayFabBuddyLib.Auth;
using PlayFabBuddyLib.UserData;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayFabBuddy.UserData.Tests
{
	public class TestPlayFabUserDataService : PlayFabUserDataService
	{
		public TestPlayFabUserDataService(IPlayFabClient playfab, IPlayFabAuthService auth) : base(playfab, auth)
		{
		}

		public List<string> TestGetOutgoingKeys(List<string> inputKeys, Dictionary<string, string> cache, Dictionary<string, string> output, bool force)
		{
			return GetOutgoingKeys(inputKeys, cache, output, force);
		}

		public Dictionary<string, string> TestPopulateData(PlayFabResult<GetTitleDataResult> result, Dictionary<string, string> cache, Dictionary<string, string> data)
		{
			return PopulateData(result, cache, data);
		}
	}

	[TestFixture]
	public class ServiceTests
	{
		TestPlayFabUserDataService _service;
		IPlayFabClient _client;
		IPlayFabAuthService _auth;

		[SetUp]
		public void Setup()
		{
			_client = A.Fake<IPlayFabClient>();
			_auth = A.Fake<IPlayFabAuthService>();

			_service = new TestPlayFabUserDataService(_client, _auth);
		}

		[Test]
		public void TestGetOutgoingKeys_CacheEmpty()
		{
			var inputKeys = new List<string>
			{
				"one",
				"two",
				"three",
			};
			var cache = new Dictionary<string, string>();
			var output = new Dictionary<string, string>();
			var force = false;

			var result = _service.TestGetOutgoingKeys(inputKeys, cache, output, force);

			result.Count.ShouldBe(3);
			result.Contains("one").ShouldBe(true);
			result.Contains("two").ShouldBe(true);
			result.Contains("three").ShouldBe(true);

			output.Count.ShouldBe(0);
			output.ContainsKey("one").ShouldBe(false);
			output.ContainsKey("two").ShouldBe(false);
			output.ContainsKey("three").ShouldBe(false);
		}

		[Test]
		public void TestGetOutgoingKeys_CacheHaValue()
		{
			var inputKeys = new List<string>
			{
				"one",
				"two",
				"three",
			};
			var cache = new Dictionary<string, string>()
			{
				{ "one", "1" }
			};

			var output = new Dictionary<string, string>();
			var force = false;

			var result = _service.TestGetOutgoingKeys(inputKeys, cache, output, force);

			result.Count.ShouldBe(2);
			result.Contains("one").ShouldBe(false);
			result.Contains("two").ShouldBe(true);
			result.Contains("three").ShouldBe(true);

			output.Count.ShouldBe(1);
			output.ContainsKey("one").ShouldBe(true);
			output["one"].ShouldBe("1");
			output.ContainsKey("two").ShouldBe(false);
			output.ContainsKey("three").ShouldBe(false);
		}

		[Test]
		public void TestGetOutgoingKeys_Force()
		{
			var inputKeys = new List<string>
			{
				"one",
				"two",
				"three",
			};
			var cache = new Dictionary<string, string>()
			{
				{ "one", "1" }
			};
			var output = new Dictionary<string, string>();
			var force = true;

			var result = _service.TestGetOutgoingKeys(inputKeys, cache, output, force);

			result.Count.ShouldBe(3);
			result.Contains("one").ShouldBe(true);
			result.Contains("two").ShouldBe(true);
			result.Contains("three").ShouldBe(true);

			output.Count.ShouldBe(0);
			output.ContainsKey("one").ShouldBe(false);
			output.ContainsKey("two").ShouldBe(false);
			output.ContainsKey("three").ShouldBe(false);
		}

		[Test]
		public void TestPopulateData()
		{
			var playFabResult = new PlayFabResult<GetTitleDataResult>
			{
				Result = new GetTitleDataResult
				{
					Data = new Dictionary<string, string>
					{
						{  "one", "1" }
					}
				}
			};

			var cache = new Dictionary<string, string>()
			{
				{ "two", "2" }
			};

			var data = new Dictionary<string, string>();

			var result = _service.TestPopulateData(playFabResult, cache, data);

			cache.Count.ShouldBe(2);
			cache["one"].ShouldBe("1");
			cache["two"].ShouldBe("2");

			data.Count.ShouldBe(1);
			data["one"].ShouldBe("1");
		}

		[Test]
		public void TestPopulateData_Overwrite()
		{
			var playFabResult = new PlayFabResult<GetTitleDataResult>
			{
				Result = new GetTitleDataResult
				{
					Data = new Dictionary<string, string>
					{
						{  "one", "1" }
					}
				}
			};

			var cache = new Dictionary<string, string>()
			{
				{ "one", "2" }
			};

			var data = new Dictionary<string, string>();

			var result = _service.TestPopulateData(playFabResult, cache, data);

			cache.Count.ShouldBe(1);
			cache["one"].ShouldBe("1");
		}
	}
}
