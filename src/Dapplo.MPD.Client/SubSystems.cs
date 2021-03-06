// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.MPD.Client
{
	/// <summary>
	///     This is the list of the sub systems that the MPD has and can notify about when using the idle command
	/// </summary>
	public enum SubSystems
	{
		Database,
		Update,
		StoredPlaylist,
		Playlist,
		Player,
		Mixer,
		Output,
		Options,
		Sticker,
		Subscription,
		Message
	}
}