using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Dapplo.MPD
{
	/// <summary>
	/// This is the MpdClient which has high-level commands
	/// </summary>
	public class MpdClient : MpdSocketClient
	{
		/// <summary>
		/// Static factory to create a MpdClient
		/// </summary>
		/// <param name="hostname"></param>
		/// <param name="port"></param>
		/// <returns>Task which return MpdClient</returns>
		public new static async Task<MpdClient> CreateAsync(string hostname, int port)
		{
			var mpdClient = new MpdClient();
			await mpdClient.InitAsync(hostname, port);
			return mpdClient;
		}

		/// <summary>
		/// Get the status of the MPD
		/// </summary>
		/// <returns>MpdStatus</returns>
		public async Task<MpdStatus> Status()
		{
			var response = await SendCommandAsync("status");
			var status = new MpdStatus();
			foreach (var statusEntry in response.Response)
			{
				var nameValue = statusEntry.Split(new[] { ':' }, 2);
				var name = nameValue[0];
				var value = nameValue[1].Trim();
                switch (name)
				{
					case "volume":
						int volume;
						int.TryParse(value, out volume);
						status.Volume = volume;
						break;
					case "repeat":
						status.Repeat = "1" == value;
						break;
					case "random":
						status.Random = "1" == value;
						break;
					case "single":
						status.Single = "1" == value;
						break;
					case "consume":
						status.Consume = "1" == value;
						break;
					case "playlist":
						uint playlistVersion;
						uint.TryParse(value, out playlistVersion);
						status.PlaylistVersion = playlistVersion;
						break;
					case "playlistlength":
						uint playlistLength;
						uint.TryParse(value, out playlistLength);
						status.PlaylistLength = playlistLength;
						break;
					case "song":
						uint playlistSong;
						uint.TryParse(value, out playlistSong);
						status.PlaylistSong = playlistSong;
						break;
					case "songid":
						uint playlistSongId;
						uint.TryParse(value, out playlistSongId);
						status.PlaylistSongId = playlistSongId;
						break;
					case "nextsong":
						uint playlistNextSong;
						uint.TryParse(value, out playlistNextSong);
						status.PlaylistNextSong = playlistNextSong;
						break;
					case "nextsongid":
						uint playlistNextSongId;
						uint.TryParse(value, out playlistNextSongId);
						status.PlaylistNextSongId = playlistNextSongId;
						break;
					case "bitrate":
						int bitrate;
						int.TryParse(value, out bitrate);
						status.Bitrate = bitrate;
						break;
					case "audio":
						status.Audioformat = value;
						break;
					case "xfade":
						int crossfade;
						int.TryParse(value, out crossfade);
						status.Crossfade = crossfade;
						break;
					case "state":
						PlayStates playState;
						Enum.TryParse(value, true, out playState);
						status.PlayState = playState;
						break;
					case "elapsed":
						int seconds, milliseconds;
						var timeParts = value.Split(new[] {'.' }, 2);
						int.TryParse(timeParts[0], out seconds);
						int.TryParse(timeParts[1], out milliseconds);

						status.Elapsed = TimeSpan.FromSeconds(seconds) + TimeSpan.FromMilliseconds(milliseconds);
						break;
					case "duration":
						status.Duration = TimeSpan.Parse(value);
						break;
					default:
						Debug.WriteLine($"Unprocessed status: {nameValue[0]}");
						break;
				}
			}
			return status;
		}
	}
}
