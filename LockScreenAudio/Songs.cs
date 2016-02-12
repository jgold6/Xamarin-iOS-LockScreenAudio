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


		public static Dictionary<string, List<Song>> searchResults = new Dictionary<string, List<Song>>();
		public static int searchSongCount { get; private set;}
		public static int searchArtistCount { get; private set;}

		static List<Song> streamingSongs;

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

		public static List<Song> GetSearchedSongsByArtist(string artistName)
		{
			List<Song> sba = new List<Song>();
			searchResults.TryGetValue(artistName, out sba);
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

			if (searching)
				return searchResults.Values.ToList()[section][row];
			else
				return artistSongs.Values.ToList()[section][row];
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

		public static int GetSearchedIndexOfSongByArtist(Song song)
		{
			int index = 0;
			foreach (Song s in Songs.GetSearchedSongsByArtist(song.artist)) {
				if (song.song == s.song)
					return index;
				else
					index++;
			}
			return 0;
		}


		public static void FilterContentsForSearch(string str)
		{
			searchResults.Clear();

			// Song and Artist search
			foreach (string artist in artistSongs.Keys) {
				List<Song> songs  = GetSongsByArtist(artist);
				if (artist.ToLower().Contains(str.ToLower())) {
					searchResults.Add(artist, songs);
				}
				else {
					List<Song> matchingSongs = songs.Where(a => a.song.ToLower().Contains(str.ToLower())).ToList();
					if (matchingSongs.Count > 0)
						searchResults.Add(artist, matchingSongs);
				}

			}

			// Song only search
//			foreach (List<Song> songs in artistSongs.Values) {
//				List<Song> matchingSongs = songs.Where(a => a.song.ToLower().Contains(str.ToLower())).ToList();
//				if (matchingSongs.Count > 0)
//					searchResults.Add(matchingSongs[0].artist, matchingSongs);
//			}

			searchArtistCount = searchResults.Keys.Count;
			var artists = searchResults.Values;
			searchSongCount = 0;
			foreach (List<Song> songs in artists) {
				searchSongCount += songs.Count;
			}
		}

		public static List<Song> GetStreamingSongs()
		{
			if (streamingSongs == null)
				setUpStreamingSongs ();
			return streamingSongs;
		}

		public static int GetIndexOfStreamingSong (Song song)
		{
			if (streamingSongs == null)
				setUpStreamingSongs ();
			return streamingSongs.IndexOf (song);
		}

		public static Song GetStreamingSongByIndex(int index)
		{
			if (streamingSongs == null)
				setUpStreamingSongs ();
			return streamingSongs [index];
		}

		static void setUpStreamingSongs()
		{
			streamingSongs = new List<Song>();

			Song song = new Song();
			song.song = "Crocodile Tears";
			song.album = "Johnny Gold";
			song.artist = "Johnny Gold";
			song.duration = 232.0;
			song.streamingURL = "http://johnnygold.com/music/croctears.mp3";
			streamingSongs.Add (song);

			song = new Song();
			song.song = "912";
			song.album = "Johnny Gold";
			song.artist = "Johnny Gold";
			song.duration = 257.0;
			song.streamingURL = "http://johnnygold.com/music/ninetwelve.mp3";
			streamingSongs.Add (song);

			song = new Song();
			song.song = "Summertime";
			song.album = "Johnny Gold";
			song.artist = "Johnny Gold";
			song.duration = 196.0;
			song.streamingURL = "http://johnnygold.com/music/summertime.mp3";
			streamingSongs.Add (song);

			song = new Song();
			song.song = "Mystery";
			song.album = "Johnny Gold";
			song.artist = "Johnny Gold";
			song.duration = 219.0;
			song.streamingURL = "http://johnnygold.com/music/mystery.mp3";
			streamingSongs.Add (song);

			song = new Song();
			song.song = "Mary Had a Little Lamb";
			song.album = "Johnny Gold";
			song.artist = "Johnny Gold";
			song.duration = 302.0;
			song.streamingURL = "http://johnnygold.com/music/maryhadalamb.mp3";
			streamingSongs.Add (song);

			song = new Song();
			song.song = "Little Beach Blues";
			song.album = "Johnny Gold";
			song.artist = "Johnny Gold";
			song.duration = 232.0;
			song.streamingURL = "http://johnnygold.com/music/littlebeach.mp3";
			streamingSongs.Add (song);

			song = new Song();
			song.song = "Stalking After Midnight";
			song.album = "Johnny Gold";
			song.artist = "Johnny Gold";
			song.duration = 110.0;
			song.streamingURL = "http://johnnygold.com/music/stalking.mp3";
			streamingSongs.Add (song);

			song = new Song();
			song.song = "Misty Eyes";
			song.album = "Johnny Gold";
			song.artist = "Johnny Gold";
			song.duration = 256.0;
			song.streamingURL = "http://johnnygold.com/music/mistyeyes.mp3";
			streamingSongs.Add (song);

			song = new Song();
			song.song = "Sunny Delight";
			song.album = "Johnny Gold";
			song.artist = "Johnny Gold";
			song.duration = 302.0;
			song.streamingURL = "http://johnnygold.com/music/sunnydelight.mp3";
			streamingSongs.Add (song);

			song = new Song();
			song.song = "Silently Lying";
			song.album = "Johnny Gold";
			song.artist = "Johnny Gold";
			song.duration = 232.0;
			song.streamingURL = "http://johnnygold.com/music/silentlylying.mp3";
			streamingSongs.Add (song);
		}

	}
}

