using System;
using MonoTouch.Foundation;
using MonoTouch.MediaPlayer;
using System.Collections.Generic;

namespace LockScreenAudio
{
	public class MusicQuery
	{

		public MusicQuery()
		{
		}

		// Get the songs on the device
		public Dictionary<string, List<Song>> queryForSongs()
		{
			MPMediaQuery query = MPMediaQuery.artistsQuery;

			MPMediaItemCollection[] songsByArtist = query.Collections;

			Dictionary<string, List<Song>> artistSongs = new Dictionary<string, List<Song>>();
			List<Song> songs;

			foreach (MPMediaItemCollection album in songsByArtist) {
				MPMediaItem[] albumSongs = album.Items;
				string artistName = "";
				songs = new List<Song>();
				foreach (MPMediaItem songMediumItem in albumSongs) {
					// Create a new song type and add the info from this song to it
					Song song = new Song();
					song.album = songMediumItem.AlbumTitle.ToString();
					song.artist = songMediumItem.Artist.ToString();
					if (artistName == "")
						artistName = song.artist;
					song.song = songMediumItem.Title.ToString();
					song.songID = songMediumItem.PersistentID;
					song.artwork = songMediumItem.Artwork;
					// Add the song to the list
					songs.Add(song);
				}
				artistSongs.Add(artistName, songs);
			}
			return artistSongs;
		}

		// Get a song with a particular id
		public MPMediaItem queryForSongWithId(ulong songPersistenceId)
		{
			MPMediaPropertyPredicate mediaItemPersistenceIdPredicate = MPMediaPropertyPredicate.PredicateWithValue(new NSNumber(songPersistenceId), MPMediaItem.PersistentIDProperty);

			MPMediaQuery songQuery = new MPMediaQuery();
			songQuery.AddFilterPredicate(mediaItemPersistenceIdPredicate);

			var items = songQuery.Items;

			return items[items.Length - 1];
		}
	}
}

