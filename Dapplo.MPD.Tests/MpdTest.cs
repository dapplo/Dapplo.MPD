using System;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.MPD.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dapplo.MPD.Tests
{
	[TestClass]
	public class MpdTest
	{
		[TestMethod]
		public async Task TestConnectAsync()
		{
			using (var client = await MpdSocketClient.CreateAsync("n40l", 6600))
			{
				var status = await client.SendCommandAsync("status");
				status.ResponseLines.ToList().ForEach(x => Debug.WriteLine(x));

				Assert.IsTrue(status.IsOk);

				// Send unknown command
				status = await client.SendCommandAsync("dapplo");
				Assert.IsFalse(status.IsOk);
			}
		}

		[TestMethod]
		public async Task TestStatusAsync()
		{
			using (var client = await MpdClient.CreateAsync("n40l", 6600))
			{
				var status = await client.StatusAsync();
				Assert.IsNotNull(status.Audioformat);
				Assert.AreEqual("44100:24:2", status.Audioformat);
			}
		}

		[TestMethod]
		public async Task TestIdleAsync()
		{
			var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(1));

            using (var client = await MpdStateMonitor.CreateAsync("n40l", 6600))
			{
				client.StateChanged += (sender, args) =>
				{
					Debug.WriteLine(args.ChangedSubsystem);
					cancellationTokenSource.Cancel();
				};
				do
				{
					// Using the delay with the token causes a TaskCanceledException
					await Task.Delay(100);
				} while (!cancellationTokenSource.IsCancellationRequested);
			}
		}
	}
}
