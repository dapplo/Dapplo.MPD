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
using System.Diagnostics;
using System.Threading.Tasks;

#endregion

namespace Dapplo.MPD.Client
{
	/// <summary>
	///     This is the MpdClient which has high-level commands
	/// </summary>
	public class MpdClient : MpdSocketClient
	{
		/// <summary>
		///     When consume is activated, each song played is removed from playlist.
		/// </summary>
		/// <param name="enable">true to enable, false to disable</param>
		public async Task ConsumeAsync(bool enable)
		{
			await SendCommandAsync("consume", Convert.ToInt32(enable).ToString());
		}

		/// <summary>
		///     Static factory to create a MpdClient
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
		///     Sets crossfading between songs
		/// </summary>
		/// <param name="seconds"></param>
		public async Task CrossfadeAsync(int seconds)
		{
			await SendCommandAsync("crossfade", seconds.ToString());
		}

		/// <summary>
		///     Get the current song information
		/// </summary>
		/// <returns>CurrentSong</returns>
		public async Task<CurrentSong> CurrentSongAsync()
		{
			var response = await SendCommandAsync("currentsong");
			var currentSong = new CurrentSong();
			foreach (var statusEntry in response.ResponseLines)
			{
				var nameValue = statusEntry.Split(new[] {':'}, 2);
				var name = nameValue[0];
				var value = nameValue[1].Trim();
				switch (name)
				{
					case "file":
						currentSong.File = value;
						break;
					case "Title":
						currentSong.Title = value;
						break;
					case "Name":
						currentSong.Name = value;
						break;
					case "Pos":
						int pos;
						int.TryParse(value, out pos);
						currentSong.Pos = pos;
						break;
					case "Id":
						int id;
						int.TryParse(value, out id);
						currentSong.Id = id;
						break;
					default:
						Debug.WriteLine($"Unprocessed current song info: {nameValue[0]}");
						break;
				}
			}
			return currentSong;
		}

		/// <summary>
		///     Sets the threshold at which songs will be overlapped.
		///     Like crossfading but doesn't fade the track volume, just overlaps.
		///     The songs need to have MixRamp tags added by an external tool.
		///     0dB is the normalized maximum volume so use negative values.
		///     In the absence of mixramp tags crossfading will be used.
		///     See http://sourceforge.net/projects/mixramp
		/// </summary>
		/// <param name="deciBels">bis 0</param>
		public async Task MixRampDbAsync(int deciBels)
		{
			await SendCommandAsync("mixrampdb", deciBels.ToString());
		}

		/// <summary>
		///     Additional time subtracted from the overlap calculated by mixrampdb.
		///     A value of lt 0 is converted to "nan" and disables MixRamp overlapping and falls back to crossfading.
		/// </summary>
		/// <param name="seconds"></param>
		public async Task MixRampDelayAsync(int seconds)
		{
			if (seconds >= 0)
			{
				await SendCommandAsync("mixrampdelay", seconds.ToString());
			}
			else
			{
				await SendCommandAsync("mixrampdelay", "nan");
			}
		}

		/// <summary>
		///     Next song
		/// </summary>
		public async Task NextAsync()
		{
			await SendCommandAsync("next");
		}

		/// <summary>
		///     Toggles pause/resumes playing,
		/// </summary>
		/// <param name="resume">true to resume, false to pause</param>
		public async Task PauseAsync(bool resume)
		{
			await SendCommandAsync("pause", Convert.ToInt32(resume).ToString());
		}

		/// <summary>
		///     Begins playing the playlist at song number songpos.
		/// </summary>
		/// <param name="songpos"></param>
		public async Task PlayAsync(int songpos)
		{
			await SendCommandAsync("play", songpos.ToString());
		}

		/// <summary>
		///     Begins playing the playlist at song songid.
		/// </summary>
		/// <param name="songId"></param>
		public async Task PlayIdAsync(int songId)
		{
			await SendCommandAsync("playid", songId.ToString());
		}

		/// <summary>
		///     Previous song
		/// </summary>
		public async Task PreviousAsync()
		{
			await SendCommandAsync("previous");
		}

		/// <summary>
		///     Enable or disable random
		/// </summary>
		/// <param name="enable">true to enable, false to disable</param>
		public async Task RandomAsync(bool enable)
		{
			await SendCommandAsync("random", Convert.ToInt32(enable).ToString());
		}

		/// <summary>
		///     Enable or disable repeat
		/// </summary>
		/// <param name="enable">true to enable, false to disable</param>
		public async Task RepeatAsync(bool enable)
		{
			await SendCommandAsync("repeat", Convert.ToInt32(enable).ToString());
		}

		/// <summary>
		///     Sets the replay gain mode. One of off, track, album, auto[5].
		///     Changing the mode during playback may take several seconds, because the new settings does not affect the buffered
		///     data.
		///     This command triggers the options idle event.
		/// </summary>
		/// <param name="replayGainMode">replay gain mode</param>
		public async Task ReplayGainModeAsync(ReplayGainModes replayGainMode)
		{
			await SendCommandAsync("replay_gain_mode ", replayGainMode.ToString().ToLowerInvariant());
		}

		/// <summary>
		///     Get the replay gain status
		/// </summary>
		/// <returns>ReplayGainModes</returns>
		public async Task<ReplayGainModes> ReplayGainStatusAsync()
		{
			var response = await SendCommandAsync("replay_gain_status");
			ReplayGainModes replayGainMode;
			Enum.TryParse(response.ResponseLines[0], true, out replayGainMode);
			return replayGainMode;
		}

		/// <summary>
		///     Seeks to the position TIME (in seconds; fractions allowed) of entry song-pos in the playlist.
		/// </summary>
		/// <param name="songPos"></param>
		/// <param name="timeSpan"></param>
		public async Task SeekAsync(int songPos, TimeSpan timeSpan)
		{
			await SendCommandAsync("seek", songPos.ToString(), timeSpan.TotalSeconds.ToString("F3"));
		}

		/// <summary>
		///     Seeks to the position TIME (in seconds; fractions allowed) within the current song.
		///     Timespan can be negative.
		/// </summary>
		/// <param name="timeSpan"></param>
		public async Task SeekCurAsync(TimeSpan timeSpan)
		{
			await SendCommandAsync("seekcur", timeSpan.TotalSeconds.ToString("F3"));
		}

		/// <summary>
		///     Seeks to the position TIME (in seconds; fractions allowed) of song-id.
		/// </summary>
		/// <param name="songId"></param>
		/// <param name="timeSpan"></param>
		public async Task SeekIdAsync(int songId, TimeSpan timeSpan)
		{
			await SendCommandAsync("seekid", songId.ToString(), timeSpan.TotalSeconds.ToString("F3"));
		}

		/// <summary>
		///     When single is activated, playback is stopped after current song, or song is repeated if the 'repeat' mode is
		///     enabled.
		/// </summary>
		/// <param name="enable">true to enable, false to disable</param>
		public async Task SingleAsync(bool enable)
		{
			await SendCommandAsync("single", Convert.ToInt32(enable).ToString());
		}

		/// <summary>
		///     Get the status
		/// </summary>
		/// <returns>MpdStatus</returns>
		public async Task<Status> StatusAsync()
		{
			var response = await SendCommandAsync("status");
			var status = new Status();
			foreach (var statusEntry in response.ResponseLines)
			{
				var nameValue = statusEntry.Split(new[] {':'}, 2);
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
						var timeParts = value.Split(new[] {'.'}, 2);
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

		/// <summary>
		///     Stop
		/// </summary>
		public async Task StopAsync()
		{
			await SendCommandAsync("stop");
		}

		/// <summary>
		///     Sets the volume
		/// </summary>
		/// <param name="volume">0-100</param>
		public async Task VolumeAsync(int volume)
		{
			await SendCommandAsync("setvol", volume.ToString());
		}
	}
}