// Loosely based on this guide: http://www.sagorin.org/ios-playing-audio-in-background-audio/
// and sample in Obj-C: https://github.com/jsagorin/iOSBackgroundAudio

using System;
using CoreGraphics;
using System.Collections.Generic;

using Foundation;
using UIKit;
using MediaPlayer;
using ObjCRuntime;

namespace LockScreenAudio
{
	public partial class DetailViewController : UIViewController
	{
		#region - instance variables
		// Currently selected song
		public Song song { get; set;} 
		// The Music Player
		public MyMusicPlayer musicPlayer {get; set;}
		// Current list of songs
		public List<Song> currentSongList {get; set;}
		public int currentSongIndex;
		public int currentSongCount;
		UIColor systemNavBarTintColor;

		#endregion

		#region - Constructors
		public DetailViewController(IntPtr handle) : base(handle)
		{
		}
		#endregion

		#region - View Controller overrides
		public override void DidReceiveMemoryWarning()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning();
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// set up handler for when app resumes from background
			NSNotificationCenter.DefaultCenter.AddObserver(this, new Selector("resumeFromBackground"), UIApplication.DidBecomeActiveNotification, null);
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			if (song.streamingURL != null) {
				currentSongList = new List<Song>(){song};
				currentSongCount = 1;
				currentSongIndex = 0;
			}
			else {
				currentSongList = Songs.GetSongsByArtist(song.artist);
				currentSongCount = currentSongList.Count;
				currentSongIndex = Songs.GetIndexOfSongByArtist(song);
			}
			DisplaySongInfo();
			this.NavigationController.NavigationBar.BackgroundColor = UIColor.DarkGray;
			this.NavigationController.NavigationBar.BarTintColor = UIColor.DarkGray;
			systemNavBarTintColor = this.NavigationController.NavigationBar.TintColor;
			this.NavigationController.NavigationBar.TintColor = UIColor.LightGray;
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			// Create and initialize music player
			musicPlayer = new MyMusicPlayer(this);
			// Play song (and load all songs by artist to player queue)
			if (song.streamingURL == null) {
				actIndView.StopAnimating();
				actIndView.Hidden = true;
				playPause.UserInteractionEnabled = true;
				playPause.TintColor = UIColor.Blue;
			}
			else {
				actIndView.StartAnimating();
				actIndView.Hidden = false;
				playPause.UserInteractionEnabled = false;
				playPause.TintColor = UIColor.DarkGray;
			}
			musicPlayer.playSong(song);
			SetPrevNextButtonStatus();

			// Register for receiving controls from lock screen and controlscreen
			UIApplication.SharedApplication.BeginReceivingRemoteControlEvents();
			this.BecomeFirstResponder();
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			actIndView.StopAnimating();
			actIndView.Hidden = true;
			musicPlayer.pause();
			// Unregister for control events
			UIApplication.SharedApplication.EndReceivingRemoteControlEvents();
			this.ResignFirstResponder();
			this.NavigationController.NavigationBar.BackgroundColor = UIColor.White;
			this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
			this.NavigationController.NavigationBar.TintColor = systemNavBarTintColor;
			// Clear the music players reference back to this class - avoid retain cycle
			musicPlayer.dvc = null;
		}

		public override bool CanBecomeFirstResponder
		{
			get
			{
				return true;
			}
		}

		public override bool ShouldAutorotate()
		{
			return false;
		}
		#endregion

		#region - Handle events from outside the app
		[Export("resumeFromBackground")]
		public void resumeFromBackGround()
		{
			if (musicPlayer.Rate > 0.0f) {
				playPause.SetTitle("Pause", UIControlState.Normal);
			}
			else 
			{
				playPause.SetTitle("Play", UIControlState.Normal);
			}
			if (song.streamingURL == null) {
				currentSongIndex = Songs.GetIndexOfSongByArtist(song);
				DisplaySongInfo();
			}
		}

		// Forward remote control received events, from the lock or control screen, to the music player.
		public override void RemoteControlReceived(UIEvent theEvent)
		{
			base.RemoteControlReceived(theEvent);
			if (theEvent.Subtype == UIEventSubtype.RemoteControlPreviousTrack) {
				PlayPrevSong();
			}
			else if (theEvent.Subtype == UIEventSubtype.RemoteControlNextTrack) {
				PlayNextSong();
			}
			else {
				musicPlayer.RemoteControlReceived(theEvent);
			}
		}
		#endregion

		#region - handle in-app playback controls
		// In app play/pause button clicked
		partial void playPauseButtonTapped (UIButton sender)
		{
			if (musicPlayer.Rate > 0.0f) {
				musicPlayer.pause();
				sender.SetTitle("Play", UIControlState.Normal);
			}
			else 
			{
				musicPlayer.play();
				sender.SetTitle("Pause", UIControlState.Normal);
			}
		}

		partial void prevButtonTapped(UIButton sender)
		{
			PlayPrevSong();
		}

		partial void nextBtnTapped(UIButton sender)
		{
			PlayNextSong();
		}
		#endregion

		#region - Class helper methods

		public void PlayPrevSong()
		{
			if (currentSongIndex >0) {
				currentSongIndex--;
				song = currentSongList[currentSongIndex];
				musicPlayer.playSong(song);
				DisplaySongInfo();
			}
		}

		public void PlayNextSong()
		{
			if (currentSongIndex + 1 < currentSongCount) {
				currentSongIndex++;
				song = currentSongList[currentSongIndex];
				musicPlayer.playSong(song);
				DisplaySongInfo();
			}
		}

		public void DisplaySongInfo()
		{
			// Display info for current song as might've changed
			artistNameLabel.Text = song.artist;
			albumNameLabel.Text = song.album;
			songTitleLabel.Text = song.song;
			songIdLabel.Text = song.streamingURL == null ? song.songID.ToString() : song.streamingURL;
			if (song.artwork != null)
				artworkView.Image = song.artwork.ImageWithSize(new CGSize(115.0f, 115.0f));
			this.NavigationItem.Title = String.Format("Playing song {0} of {1}", currentSongIndex + 1, currentSongCount);
			SetPrevNextButtonStatus();
			if (musicPlayer != null) {
				playPause.SetTitle(musicPlayer.Rate > 0.0f ? "Pause" : "Play", UIControlState.Normal);
			}
		}

		public void SetPrevNextButtonStatus() {
			if (currentSongIndex == 0) {
				prevBtn.UserInteractionEnabled = false;
				prevBtn.TintColor = UIColor.DarkGray;
			}
			else {
				prevBtn.UserInteractionEnabled = true;
				prevBtn.TintColor = UIColor.Blue;
			}
			if (currentSongIndex + 1 == currentSongCount) {
				nextBtn.UserInteractionEnabled = false;
				nextBtn.TintColor = UIColor.DarkGray;
			}
			else {
				nextBtn.UserInteractionEnabled = true;
				nextBtn.TintColor = UIColor.Blue;
			}

		}

		public void enablePlayPauseButton()
		{
			playPause.UserInteractionEnabled = true;
			playPause.TintColor = UIColor.Blue;
			actIndView.StopAnimating();
			actIndView.Hidden = true;
		}

		#endregion
	}
}

