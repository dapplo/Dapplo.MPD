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

#endregion

namespace Dapplo.MPD.Client
{
	/// <summary>
	///     This object represents the MPD current status
	/// </summary>
	public class Status
	{
		/// <summary>
		///     Audioformat sampleRate:bits:channels
		/// </summary>
		public string Audioformat { get; set; }

		/// <summary>
		///     Bitrate in kbps
		/// </summary>
		public int Bitrate { get; set; }

		/// <summary>
		///     true if consume is enabled
		/// </summary>
		public bool Consume { get; set; }

		/// <summary>
		///     Crossfade in seconds
		/// </summary>
		public int Crossfade { get; set; }

		/// <summary>
		///     Song duration
		/// </summary>
		public TimeSpan Duration { get; set; }

		/// <summary>
		///     Time elapsed in song
		/// </summary>
		public TimeSpan Elapsed { get; set; }

		/// <summary>
		///     Length of the playlist
		/// </summary>
		public uint PlaylistLength { get; set; }

		/// <summary>
		///     Playlist next song
		/// </summary>
		public uint PlaylistNextSong { get; set; }

		/// <summary>
		///     Playlist next song id
		/// </summary>
		public uint PlaylistNextSongId { get; set; }

		/// <summary>
		///     Playlist song  (stopped or playing)
		/// </summary>
		public uint PlaylistSong { get; set; }

		/// <summary>
		///     Playlist song id (stopped or playing)
		/// </summary>
		public uint PlaylistSongId { get; set; }

		/// <summary>
		///     31-bit unsigned integer, the playlist version number
		/// </summary>
		public uint PlaylistVersion { get; set; }

		/// <summary>
		///     Current play state
		/// </summary>
		public PlayStates PlayState { get; set; }

		/// <summary>
		///     true if random is enabled
		/// </summary>
		public bool Random { get; set; }

		/// <summary>
		///     true if repeat is enabled
		/// </summary>
		public bool Repeat { get; set; }

		/// <summary>
		///     true if single is enabled
		/// </summary>
		public bool Single { get; set; }

		/// <summary>
		///     Volume in MPD, -1 means there is no volume control
		/// </summary>
		public int Volume { get; set; }
	}
}