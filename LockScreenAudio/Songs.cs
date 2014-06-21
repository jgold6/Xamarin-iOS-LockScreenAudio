using System;
using System.Collections.Generic;

namespace LockScreenAudio
{
	public static class Songs
	{
		public static Dictionary<string, List<Song>> artistSongs = new Dictionary<string, List<Song>>();
		public static int songCount { get; set;}
		public static int artistCount { get; set;}

		// Get the songs from the music library
		public static void querySongs()
		{
			MusicQuery musicQuery = new MusicQuery();
			artistSongs = musicQuery.queryForSongs();
			artistCount = artistSongs.Count;

			var artists = artistSongs.Values;
			songCount = 0;
			foreach (List<Song> songs in artists) {
				songCount += songs.Count;
			}
		}
	}
}

