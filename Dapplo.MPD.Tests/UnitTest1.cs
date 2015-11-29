using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dapplo.MPD.Tests
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public async Task TestConnectAsync()
		{
			using (var client = await MpdSocketClient.CreateAsync("n40l", 6600))
			{
				var status = await client.SendCommandAsync("status");
				status.Response.ToList().ForEach(x => Debug.WriteLine(x));

				Assert.IsTrue(status.IsOk);

				// Send unknown command
				status = await client.SendCommandAsync("dapplo");
				Assert.IsFalse(status.IsOk);
			}
		}
	}
}
