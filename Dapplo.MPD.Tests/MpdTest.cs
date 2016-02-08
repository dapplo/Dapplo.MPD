/*
	Dapplo - building blocks for desktop applications
	Copyright (C) 2015-2016 Dapplo

	For more information see: http://dapplo.net/
	Dapplo repositories are hosted on GitHub: https://github.com/dapplo

	This file is part of Dapplo.MPD

	Dapplo.MPD is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	Dapplo.MPD is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with Dapplo.MPD. If not, see <http://www.gnu.org/licenses/>.
 */

using System.Linq;
using System.Threading.Tasks;
using Dapplo.LogFacade;
using Dapplo.MPD.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dapplo.MPD.Tests
{
	[TestClass]
	public class MpdTest
	{
		private static readonly LogSource Log = new LogSource();
		private static int _port;
		private static string _host;

		[ClassInitialize]
		public static void Initialize(TestContext context)
		{
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

		[TestMethod]
		public async Task TestConnectAsync()
		{

			using (var client = await MpdSocketClient.CreateAsync(_host, _port))
			{
				var status = await client.SendCommandAsync("status");
				Assert.IsTrue(status.IsOk);

				// Send unknown command
				status = await client.SendCommandAsync("dapplo");
				Assert.IsFalse(status.IsOk);
			}
		}

		[TestMethod]
		public async Task TestStatusAsync()
		{
			using (var client = await MpdClient.CreateAsync(_host, _port))
			{
				var status = await client.StatusAsync();
				Assert.IsNotNull(status.Audioformat);
				Assert.AreEqual("44100:24:2", status.Audioformat);
			}
		}

		[TestMethod]
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
	}
}
