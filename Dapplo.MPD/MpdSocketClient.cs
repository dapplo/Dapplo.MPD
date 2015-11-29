using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Dapplo.MPD
{
	/// <summary>
	/// A simple low level client to connect to a MPD server
	/// </summary>
	public class MpdSocketClient : IDisposable
	{
		private TcpClient _tcpClient;
		private NetworkStream _networkStream;

		/// <summary>
		/// The MPD connection version, not the version of MPD itself
		/// </summary>
		public string Version
		{
			get;
			private set;
		}

		/// <summary>
		/// Static factory to create a MpdSocketClient
		/// </summary>
		/// <param name="hostname"></param>
		/// <param name="port"></param>
		/// <returns>Task which return MpdSocketClient</returns>
		public static async Task<MpdSocketClient> CreateAsync(string hostname, int port)
		{
			var mpdSocketClient = new MpdSocketClient();
			await mpdSocketClient.InitAsync(hostname, port);
			return mpdSocketClient;
		}

		/// <summary>
		/// Initialize the MpdSocketClient
		/// </summary>
		/// <param name="hostname"></param>
		/// <param name="port"></param>
		/// <returns>Task too await</returns>
		private async Task InitAsync(string hostname, int port)
		{
			_tcpClient = new TcpClient();
			await _tcpClient.ConnectAsync(hostname, port).ConfigureAwait(false);
			_networkStream = _tcpClient.GetStream();
			var mpdResponse = await ReadResponseAsync().ConfigureAwait(false);
			if (!mpdResponse.IsOk)
			{
				throw new Exception(mpdResponse.ErrorMessage);
			}
			Version = mpdResponse.Response[0];
		}

		/// <summary>
		/// Send a command, and read a response
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

			}
			return result;
		}

		/// <summary>
		/// Write the line, as a command, and read a response
		/// </summary>
		/// <param name="command"></param>
		/// <returns>MpdResponse</returns>
		public async Task<MpdResponse> SendCommandAsync(string command)
		{
			var buffer = Encoding.UTF8.GetBytes($"{command}\n");
			await _networkStream.WriteAsync(buffer, 0, buffer.Length);

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
		/// This code added to correctly implement the disposable pattern.
		/// </summary>
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}
		#endregion

	}
}
