// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading.Tasks;
using Dapplo.Log;
using Dapplo.Log.XUnit;
using Dapplo.MPD.Client;
using Xunit;
using Xunit.Abstractions;

namespace Dapplo.MPD.Tests
{
	public class MpdTest
	{
		private static readonly LogSource Log = new LogSource();
		private static int _port;
		private static string _host;

		public MpdTest(ITestOutputHelper testOutputHelper)
		{
			LogSettings.RegisterDefaultLogger<XUnitLogger>(LogLevels.Verbose, testOutputHelper);
			Task.Run(async () =>
			{
				var mpdInstances = (await MpdSocketClient.FindByZeroConfAsync()).ToList();
				foreach (var mpdInstance in mpdInstances)
				{
					Log.Debug().WriteLine("Found {0} at {1}", mpdInstance.Key, mpdInstance.Value.AbsoluteUri);
				}

				_port = mpdInstances.First().Value.Port;
				_host = mpdInstances.First().Value.Host;
			}).Wait();
		}

		[Fact]
		public async Task TestConnectAsync()
		{
			using (var client = await MpdSocketClient.CreateAsync(_host, _port))
			{
				var status = await client.SendCommandAsync("status");
				Assert.True(status.IsOk);

				// Send unknown command
				status = await client.SendCommandAsync("dapplo");
				Assert.False(status.IsOk);
			}
		}

		[Fact]
		public async Task TestIdleAsync()
		{
			var taskCompletionSource = new TaskCompletionSource<bool>();

			using (var statusClient = await MpdStateMonitor.CreateAsync(_host, _port))
			{
				statusClient.StateChanged += (sender, args) =>
				{
					Log.Info().WriteLine("Subsystem changed {0}", args.ChangedSubsystem);
					taskCompletionSource.SetResult(true);
				};
				using (var controlClient = await MpdClient.CreateAsync(_host, _port))
				{
					var status = await controlClient.StatusAsync();
					await controlClient.PauseAsync(status.PlayState == PlayStates.Playing);
					status = await controlClient.StatusAsync();
					await controlClient.PauseAsync(status.PlayState == PlayStates.Playing);
				}
				// Using the delay with the token causes a TaskCanceledException
				await taskCompletionSource.Task;
			}
		}

		[Fact]
		public async Task TestStatusAsync()
		{
			using (var client = await MpdClient.CreateAsync(_host, _port))
			{
				var status = await client.StatusAsync();
				Assert.NotNull(status.Audioformat);
				Assert.Equal("44100:16:2", status.Audioformat);
			}
		}
	}
}