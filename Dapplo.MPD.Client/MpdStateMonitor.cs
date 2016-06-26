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

using System;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.Log.Facade;

namespace Dapplo.MPD.Client
{
	/// <summary>
	/// Handler of the StateChanged event
	/// </summary>
	/// <param name="sender">object which sent the event</param>
	/// <param name="e">MpdStateChangedEventArgs</param>
	public delegate void MpdStateChangedHandler(object sender, MpdStateChangedEventArgs e);

	/// <summary>
	/// EventArgs for the MpdStateChanged event
	/// </summary>
	public class MpdStateChangedEventArgs : EventArgs
	{
		public SubSystems ChangedSubsystem
		{
			get;
			set;
		} 
	}

	/// <summary>
	/// Use this to monitor subsystem state chances of the MPD server
	/// </summary>
	public class MpdStateMonitor : IDisposable
	{
		private static readonly LogSource Log = new LogSource();
		private MpdSocketClient _mpdSocketClient;
		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public event MpdStateChangedHandler StateChanged;

		/// <summary>
		/// Static factory to create a MpdStateMonitor
		/// </summary>
		/// <param name="hostname"></param>
		/// <param name="port"></param>
		/// <returns>Task which return MpdStateMonitor</returns>
		public static async Task<MpdStateMonitor> CreateAsync(string hostname, int port)
		{
			var mpdStateMonitor = new MpdStateMonitor();
			await mpdStateMonitor.InitAsync(hostname, port);
			return mpdStateMonitor;
		}

		/// <summary>
		/// Initialize the MpdStateMonitor
		/// </summary>
		/// <param name="hostname"></param>
		/// <param name="port"></param>
		/// <returns>Task to await</returns>
		private async Task InitAsync(string hostname, int port)
		{
			_mpdSocketClient = await MpdSocketClient.CreateAsync(hostname, port);
			// TODO: store in member variable to prevent GC?
			// ReSharper disable once UnusedVariable
			var ignoringTask = BackgroundCheckerAsync();
		}

		/// <summary>
		/// This is the main loop
		/// </summary>
		/// <returns>Task which can be ignored</returns>
		private async Task BackgroundCheckerAsync()
		{
			do
			{
				var response = await _mpdSocketClient.SendCommandAsync("idle");
				if (!response.IsOk)
				{
					Log.Error().WriteLine("Error response: {0}", string.Join(Environment.NewLine, response.ResponseLines));
					continue;
				}
				var subSystemString = response.ResponseLines[0].Replace("changed: ", "");
				SubSystems subSystem;
				if (Enum.TryParse(subSystemString.Replace("_", ""), true, out subSystem))
				{
					StateChanged?.Invoke(this, new MpdStateChangedEventArgs {ChangedSubsystem = subSystem});
				}
			} while (!_cancellationTokenSource.IsCancellationRequested);
		}

		#region IDisposable Support
		private bool _disposedValue; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					_cancellationTokenSource.Cancel();
					_cancellationTokenSource.Dispose();
					_mpdSocketClient.Dispose();
				}

				_disposedValue = true;
			}
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}
		#endregion
	}
}
