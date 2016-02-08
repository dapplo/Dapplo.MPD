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


namespace Dapplo.MPD.Client
{
	public enum AckCodes
	{
		NotList = 1,
		Arg = 2,
		Password = 3,
		Permission = 4,
		Unknown = 5,
		NoExist = 50,
		PlaylistMax = 51,
		System = 52,
		PlaylistLoad = 53,
		UpdateAlready = 54,
		PlayerSync = 55,
		Exist = 56,
	}
}
