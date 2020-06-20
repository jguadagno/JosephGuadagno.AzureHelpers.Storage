using System;

namespace JosephGuadagno.AzureHelpers.Storage.Tests.Models
{
	/// <summary>
	/// Represents a test object for the integration tests
	/// </summary>
	[Serializable]
	public class TestObject
	{
		public string StringProperty { get; set; }
		public int RandomNumber { get; set; }

		public static TestObject GetSampleObject()
		{
			return new TestObject
			{
				StringProperty = "string property",
				RandomNumber = new Random().Next(1, 1000)
			};
		}
	}
}