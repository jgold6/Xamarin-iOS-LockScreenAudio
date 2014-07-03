using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace LockScreenAudio
{
	public static class Songs
	{
		public static Dictionary<string, List<Song>> artistSongs = new Dictionary<string, List<Song>>();
		public static int songCount { get; private set;}
		public static int artistCount { get; private set;}


		public static Dictionary<string, List<Song>> searchResults;
		public static int searchSongCount { get; private set;}
		public static int searchArtistCount { get; private set;}

		public static bool searching = false;

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

			searchSongCount = 0;
			searchArtistCount = 0;
		}

		public static List<string> GetListOfArtists()
		{
			if (searching)
				return searchResults.Keys.ToList();
			else
				return artistSongs.Keys.ToList();
		}

		public static List<Song> GetSongsByArtist(string artistName)
		{
			List<Song> sba = new List<Song>();
			artistSongs.TryGetValue(artistName, out sba);
			return sba;
		}

		public static List<Song> GetSongsByArtistIndex(int index)
		{
			if (searching) {
				var searchSongList = searchResults.Values.ToList();
				return searchSongList[index];
			}
			else {
				var artistSongList = artistSongs.Values.ToList();
				return artistSongList[index];
			}
		}

		public static Song GetSongBySectionRow(int section, int row)
		{
			List<List<Song>> SongList;
			if (searching)
				SongList = searchResults.Values.ToList();
			else
				SongList = artistSongs.Values.ToList();
			return SongList[section][row];
//			var artistSongList = artistSongs.Values.ToList();
//			return artistSongList[section][row];
		}

		public static int GetIndexOfArtist(string artistName)
		{
			var artists = artistSongs.Keys.ToList();
			int index = 0;
			foreach (string artist in artists) {
				if (artist == artistName)
					return index;
				else
					index++;
			}
			return -1;
		}

		public static string GetArtistByIndex(int index)
		{
			if (searching)
				return searchResults.Keys.ToList()[index];
			else
				return artistSongs.Keys.ToList()[index];
		}

		public static Song GetSongByTitle(string title)
		{
			var artists = artistSongs.Values;
			foreach (List<Song> songs in artists) {
				foreach (Song song in songs) {
					if (song.song == title)
						return song;
				}
			}
			return null;
		}

		public static int GetIndexOfSongByArtist(Song song)
		{
			int index = 0;
			foreach (Song s in Songs.GetSongsByArtist(song.artist)) {
				if (song.song == s.song)
					return index;
				else
					index++;
			}
			return 0;
		}

		public static void FilterContentsForSearch(string str)
		{
			searchResults = artistSongs.Where(a => a.Key.ToLower().Contains(str.ToLower())).ToDictionary(a => a.Key, a => a.Value);
			Dictionary<string, List<Song>> otherArtistSongs = artistSongs.Where(a => !a.Key.ToLower().Contains(str.ToLower())).ToDictionary(a => a.Key, a => a.Value);
			foreach (List<Song> songs in otherArtistSongs.Values) {
				List<Song> matchingSongs = songs.Where(a => a.song.ToLower().Contains(str.ToLower())).ToList();
				if (matchingSongs.Count > 0)
					searchResults.Add(matchingSongs[0].artist, matchingSongs);
			}
			searchArtistCount = searchResults.Keys.Count;
			var artists = searchResults.Values;
			searchSongCount = 0;
			foreach (List<Song> songs in artists) {
				searchSongCount += songs.Count;
			}
			searchResults = searchResults.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
		}
	}
}

