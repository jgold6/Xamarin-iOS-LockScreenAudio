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
			avQueuePlayer.ActionAtItemEnd = AVPlayerActionAtItemEnd.Pause;

		}
		#endregion

		#region - Public methods
		// Play song from persistentSongID
		public void playSongWithId(ulong songId)
		{
			MusicQuery musicQuery = new MusicQuery();
			MPMediaItem mediaItem = musicQuery.queryForSongWithId(songId);
			if (mediaItem != null) {
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
									AVPlayerItem item = null;
									NSUrl Url = mi.AssetURL;
									item = AVPlayerItem.FromUrl(Url);
									if (item != null) {
										this.avQueuePlayer.InsertItem(item, null);
									}
									if (mi == mediaItem) {
										currentSongIndex = index;
										MPNowPlayingInfo np = new MPNowPlayingInfo();
										SetNowPlayingInfo(song, np);
									}
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
							AVPlayerItem item =  null;
							item = AVPlayerItem.FromUrl(Url);
							if (item != null) {
								this.avQueuePlayer.InsertItem(item, null);
								if (index == currentSongIndex) {
									dvc.song = song;
									MPNowPlayingInfo np = new MPNowPlayingInfo();
									SetNowPlayingInfo(dvc.song, np);
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
				MPNowPlayingInfo np = new MPNowPlayingInfo();
				SetNowPlayingInfo(dvc.song, np);
				dvc.DisplaySongInfo();
			}
		}

		// Handle control events from lock or control screen
		public void RemoteControlReceived(UIEvent theEvent)
		{
			MPNowPlayingInfo np = new MPNowPlayingInfo();
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
				if (avQueuePlayer.CurrentTime.Seconds < dvc.song.duration - 1.0)
					PreviousTrack();
				else 
					avQueuePlayer.Seek(CMTime.FromSeconds(0.0, 1));
			}
			else if (theEvent.Subtype == UIEventSubtype.RemoteControlNextTrack) {
				Console.WriteLine("Received forward event");
				NextTrack();
			}
			else if (theEvent.Subtype == UIEventSubtype.RemoteControlBeginSeekingForward) {
				Console.WriteLine("Received seek forward event");
				avQueuePlayer.Rate = 5.0f;
				np.PlaybackRate = 5.0f;
			}
			else if (theEvent.Subtype == UIEventSubtype.RemoteControlEndSeekingForward) {
				Console.WriteLine("Received end seek forward event");
				avQueuePlayer.Rate = 1.0f;
				np.PlaybackRate = 1.0f;
			}
			else if (theEvent.Subtype == UIEventSubtype.RemoteControlBeginSeekingBackward) {
				Console.WriteLine("Received seek backward event");
				avQueuePlayer.Rate = -5.0f;
				np.PlaybackRate = -5.0f;
			}
			else if (theEvent.Subtype == UIEventSubtype.RemoteControlEndSeekingBackward) {
				Console.WriteLine("Received end seek forward event");
				avQueuePlayer.Rate = 1.0f;
				np.PlaybackRate = 1.0f;
			}
			np.ElapsedPlaybackTime = avQueuePlayer.CurrentTime.Seconds;
			SetNowPlayingInfo(dvc.song, np);
		}

		public void clear()
		{
			this.avQueuePlayer.RemoveAllItems();
		}
		#endregion

		#region - Helper methods
		void SetNowPlayingInfo(Song song, MPNowPlayingInfo np)
		{
			// Pass song info to the lockscreen/control screen
			np.AlbumTitle = song.album;
			np.Artist = song.artist;
			np.Title = song.song;
			np.PersistentID = song.songID;
			np.Artwork = song.artwork;
			np.PlaybackDuration = song.duration;
			MPNowPlayingInfoCenter.DefaultCenter.NowPlaying = np;
		}
		#endregion
	}
}

