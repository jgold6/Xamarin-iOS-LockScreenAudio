// Loosely based on this guide: http://www.sagorin.org/ios-playing-audio-in-background-audio/
// and sample in Obj-C: https://github.com/jsagorin/iOSBackgroundAudio

using System;
using MonoTouch.AVFoundation;
using MonoTouch.Foundation;
using MonoTouch.AudioToolbox;
using MonoTouch.MediaPlayer;
using MonoTouch.UIKit;
using System.Collections.Generic;
using MonoTouch.CoreMedia;

namespace LockScreenAudio
{
	public class MyMusicPlayer
	{
		#region - Private instance variables
		List<Song> songs = new List<Song>();
		int currentSongIndex = 0;
		AVQueuePlayer avQueuePlayer = new AVQueuePlayer();
		#endregion

		#region - Public properties
		public DetailViewController dvc { get; set;}
		public float Rate { 
			get {
				return avQueuePlayer.Rate;
			}
			set {
				avQueuePlayer.Rate = value;
			}
		}
		#endregion

		#region - Constructors
		public MyMusicPlayer(DetailViewController viewController)
		{
			dvc = viewController;
			initSession();
		}

		// Initialize audio session
		void initSession()
		{
			AVAudioSession avSession = AVAudioSession.SharedInstance();

			avSession.SetCategory(AVAudioSessionCategory.Playback);

			NSError activationError = null;
			avSession.SetActive(true, out activationError);
			if (activationError != null)
				Console.WriteLine("Could not activate audio session {0}", activationError.LocalizedDescription);
		}
		#endregion

		#region - Public methods
		// Play song from persistentSongID
		public void playSongWithId(ulong songId)
		{
			MusicQuery musicQuery = new MusicQuery();
			MPMediaItem mediaItem = musicQuery.queryForSongWithId(songId);
			if (mediaItem != null) {
				NSUrl assetUrl = mediaItem.AssetURL;
				var aSongs = Songs.artistSongs.Keys;
				int index = 0;

				foreach (string artist in aSongs) {
					if (artist == mediaItem.Artist) {
						songs.Clear();
						Songs.artistSongs.TryGetValue(artist, out songs);
						if (songs.Count > 0) {

							foreach (Song song in songs) {
								MPMediaItem mi = musicQuery.queryForSongWithId(song.songID);
								if (mi != null) {
									NSUrl Url = mi.AssetURL;
									AVPlayerItem item = AVPlayerItem.FromUrl(Url);
									if (item != null) {
										this.avQueuePlayer.InsertItem(item, null);
									}
								}
								if (mi == mediaItem) {
									currentSongIndex = index;
									SetNowPlayingInfo(song);
								}
								index++;
							}
							for (int i = 0; i < currentSongIndex; i++) {
								avQueuePlayer.AdvanceToNextItem();
							}
							this.play();
						}
						return;
					}
				} // end artist loop
			}
		}

		public void pause()
		{
			this.avQueuePlayer.Pause();
		}

		public void play()
		{
			this.avQueuePlayer.Play();
		}

		public void PreviousTrack()
		{
			if (currentSongIndex > 0) {
				currentSongIndex--;
				clear();
				int index = 0;
				foreach (Song song in songs) {
					if (index>=currentSongIndex) {
						MusicQuery musicQuery = new MusicQuery();
						MPMediaItem mi = musicQuery.queryForSongWithId(song.songID);
						if (mi != null) {
							NSUrl Url = mi.AssetURL;
							AVPlayerItem item = AVPlayerItem.FromUrl(Url);
							if (item != null) {
								this.avQueuePlayer.InsertItem(item, null);
								if (index == currentSongIndex) {
									dvc.song = song;
									SetNowPlayingInfo(dvc.song);
									dvc.DisplaySongInfo();
								}
							}
						}
					}
					index++;
				}
			}
		}

		public void NextTrack()
		{
			if (currentSongIndex < songs.Count - 1) {
				this.avQueuePlayer.AdvanceToNextItem();
				currentSongIndex++;
				dvc.song = songs[currentSongIndex];
				SetNowPlayingInfo(dvc.song);
				dvc.DisplaySongInfo();
			}
		}

		// Handle control events from lock or control screen
		public void RemoteControlReceived(UIEvent theEvent)
		{
			if (theEvent.Subtype == UIEventSubtype.RemoteControlPause) {
				Console.WriteLine("Received pause event");
				this.avQueuePlayer.Pause();
			}
			else if (theEvent.Subtype == UIEventSubtype.RemoteControlPlay) {
				Console.WriteLine("Received play event");
				this.avQueuePlayer.Play();
			}
			else if (theEvent.Subtype == UIEventSubtype.RemoteControlPreviousTrack) {
				Console.WriteLine("Received back event");
				PreviousTrack();
			}
			else if (theEvent.Subtype == UIEventSubtype.RemoteControlNextTrack) {
				Console.WriteLine("Received forward event");
				NextTrack();
			}
		}

		public void clear()
		{
			this.avQueuePlayer.RemoveAllItems();
		}
		#endregion

		#region - Helper methods
		void SetNowPlayingInfo(Song song)
		{
			// Pass song info to the lockscreen/control screen
			MPNowPlayingInfo np = new MPNowPlayingInfo();
			np.AlbumTitle = song.album;
			np.Artist = song.artist;
			np.Title = song.song;
			np.PersistentID = song.songID;
			np.Artwork = song.artwork;
			MPNowPlayingInfoCenter.DefaultCenter.NowPlaying = np;
		}
		#endregion
	}
}

