﻿// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
		Exist = 56
	}
}