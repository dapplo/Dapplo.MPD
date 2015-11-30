namespace Dapplo.MPD
{
	/// <summary>
	/// This is the list of the sub systems that the MPD has and can notify about when using the idle command
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