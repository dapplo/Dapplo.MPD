using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dapplo.MPD
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
			// ReSharper disable once UnusedVariable
			var ignoringTask = BackgroundChecker();
		}

		/// <summary>
		/// This is the main loop
		/// </summary>
		/// <returns>Task which can be ignored</returns>
		private async Task BackgroundChecker()
		{
			do
			{
				var response = await _mpdSocketClient.SendCommandAsync("idle");
				if (!response.IsOk)
				{
					// TODO: Output response error message
					continue;
				}
				var subSystemString = response.Response[0].Replace("changed: ", "");
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
