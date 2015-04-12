// Loosely based on this guide: http://www.sagorin.org/ios-playing-audio-in-background-audio/
// and sample in Obj-C: https://github.com/jsagorin/iOSBackgroundAudio

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
			/*
			 	TigerMending album (12 missing on 5s) Picked up in app on 4 but not on 5s… not filtered out, just not picked up by app????
				Casey James (“Let’s do…"Missing on 4) <<<<<<<<<<<< filtered out as they should be as they ARE icloud items (not on computer or device)
				Israel K (2 extra versions on 5s) <<<<<<<<<<<<<<<<<
				Muse (2 extra “Hysteria” and “Time is running out” on 5s) <<<<<<<<<<<<
				Owsley (“Undone" missing on 4) <<<<<<<<<<<<<<<<<<<
				Radiohead (6 “Nude” single and stems missing on 4) <<<<<<<<<<<<<<<
				U2 (1 “Vertigo” extra on 5s) <<<<<<<<<<<<<<<<<<<
			*/
			MPMediaPropertyPredicate filter = MPMediaPropertyPredicate.PredicateWithValue(NSNumber.FromBoolean(false), MPMediaItem.IsCloudItemProperty);
			query.AddFilterPredicate(filter);

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
					song.duration = songMediumItem.PlaybackDuration;

					// Add the song to the list
					songs.Add(song);
				}
				if (!artistSongs.ContainsKey(artistName))
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

