//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016 Dapplo
// 
//  For more information see: http://dapplo.net/
//  Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
//  This file is part of Dapplo.MPD
// 
//  Dapplo.MPD is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  Dapplo.MPD is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have a copy of the GNU Lesser General Public License
//  along with Dapplo.MPD. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

#region using

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Dapplo.Log.Facade;
using Zeroconf;

#endregion

namespace Dapplo.MPD.Client
{
	/// <summary>
	///     A simple low level client to connect to a MPD server
	/// </summary>
	public class MpdSocketClient : IDisposable
	{
		private static readonly LogSource Log = new LogSource();
		private NetworkStream _networkStream;

		private TcpClient _tcpClient;

		/// <summary>
		///     The MPD connection version, not the version of MPD itself
		/// </summary>
		public string Version { get; private set; }

		/// <summary>
		///     Static factory to create a MpdSocketClient
		/// </summary>
		/// <param name="hostname"></param>
		/// <param name="port"></param>
		/// <returns>Task which return MpdSocketClient</returns>
		public static async Task<MpdSocketClient> CreateAsync(string hostname, int port)
		{
			var mpdSocketClient = new MpdSocketClient();
			await mpdSocketClient.InitAsync(hostname, port).ConfigureAwait(false);
			return mpdSocketClient;
		}

		/// <summary>
		///     Detect running MPD instances via ZeroConf
		/// </summary>
		/// <returns></returns>
		public static async Task<IEnumerable<KeyValuePair<string, Uri>>> FindByZeroConfAsync()
		{
			var responses = await ZeroconfResolver.ResolveAsync("_mpd._tcp.local.").ConfigureAwait(false);
			return
				from response in responses
				from service in response.Services
				select new KeyValuePair<string, Uri>(response.DisplayName, new Uri($"http://{response.IPAddress}:{service.Value.Port}"));
		}

		/// <summary>
		///     Initialize the MpdSocketClient
		/// </summary>
		/// <param name="hostname"></param>
		/// <param name="port"></param>
		/// <returns>Task too await</returns>
		protected async Task InitAsync(string hostname, int port)
		{
			Log.Verbose().WriteLine("Connecting to {0}:{1}", hostname, port);
			_tcpClient = new TcpClient();
			await _tcpClient.ConnectAsync(hostname, port).ConfigureAwait(false);
			_networkStream = _tcpClient.GetStream();
			var mpdResponse = await ReadResponseAsync().ConfigureAwait(false);
			if (!mpdResponse.IsOk)
			{
				throw new Exception(mpdResponse.ErrorMessage);
			}
			Version = mpdResponse.ResponseLines[0];
			Log.Debug().WriteLine("Connected to MPD version {0}", Version);
		}

		/// <summary>
		///     Wait for a response
		/// </summary>
		/// <returns>MpdResponse</returns>
		private async Task<MpdResponse> ReadResponseAsync()
		{
			var result = new MpdResponse();
			using (var reader = new StreamReader(_networkStream, Encoding.UTF8, true, 512, true))
			{
				string line;
				do
				{
					line = await reader.ReadLineAsync().ConfigureAwait(false);
				} while (!result.AddLine(line));
				if (Log.IsVerboseEnabled())
				{
					Log.Verbose().WriteLine("Received answer: {0}", string.Join(Environment.NewLine, result.ResponseLines));
				}
			}
			return result;
		}

		/// <summary>
		///     Send a command to the server, and await the response
		/// </summary>
		/// <param name="command"></param>
		/// <param name="arguments"></param>
		/// <returns>MpdResponse</returns>
		public async Task<MpdResponse> SendCommandAsync(string command, params string[] arguments)
		{
			var commandline = new StringBuilder(command);
			if (arguments != null)
			{
				foreach (var argument in arguments)
				{
					commandline.Append($" {argument}");
				}
			}
			Log.Verbose().WriteLine("Sending command: {0}", commandline);
			var buffer = Encoding.UTF8.GetBytes($"{commandline}\n");
			await _networkStream.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

			return await ReadResponseAsync().ConfigureAwait(false);
		}

		#region IDisposable Support

		private bool _disposedValue; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					_networkStream.Dispose();
					_tcpClient.Close();
				}

				_disposedValue = true;
			}
		}

		/// <summary>
		///     This code added to correctly implement the disposable pattern.
		/// </summary>
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}

		#endregion
	}
}