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