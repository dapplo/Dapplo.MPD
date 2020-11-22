// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.MPD.Client
{
	/// <summary>
	///     This object represents the MPD current song
	/// </summary>
	public class CurrentSong
	{
		/// <summary>
		///     Uri of the file
		/// </summary>
		public string File { get; set; }

		/// <summary>
		///     Id of song
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		///     Name of current entry
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///     Position
		/// </summary>
		public int Pos { get; set; }

		/// <summary>
		///     Title of song
		/// </summary>
		public string Title { get; set; }
	}
}