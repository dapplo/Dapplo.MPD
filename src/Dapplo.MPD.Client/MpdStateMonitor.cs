﻿// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.Log;

namespace Dapplo.MPD.Client
{
	/// <summary>
	///     Handler of the StateChanged event
	/// </summary>
	/// <param name="sender">object which sent the event</param>
	/// <param name="e">MpdStateChangedEventArgs</param>
	public delegate void MpdStateChangedHandler(object sender, MpdStateChangedEventArgs e);

	/// <summary>
	///     EventArgs for the MpdStateChanged event
	/// </summary>
	public class MpdStateChangedEventArgs : EventArgs
	{
		public SubSystems ChangedSubsystem { get; set; }
	}

	/// <summary>
	///     Use this to monitor subsystem state chances of the MPD server
	/// </summary>
	public class MpdStateMonitor : IDisposable
	{
		private static readonly LogSource Log = new LogSource();
		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private MpdSocketClient _mpdSocketClient;

		/// <summary>
		///     This is the main loop
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

		/// <summary>
		///     Static factory to create a MpdStateMonitor
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
		///     Initialize the MpdStateMonitor
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

		public event MpdStateChangedHandler StateChanged;

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
	}
}