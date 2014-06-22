// Loosely based on this guide: http://www.sagorin.org/ios-playing-audio-in-background-audio/
// and sample in Obj-C: https://github.com/jsagorin/iOSBackgroundAudio

using System;
using System.Drawing;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.MediaPlayer;
using MonoTouch.ObjCRuntime;

namespace LockScreenAudio
{
	public partial class DetailViewController : UIViewController
	{
		#region - instance variables
		// Currently selected song
		public Song song { get; set;} 
		// The Music Player
		public MyMusicPlayer musicPlayer {get; set;}
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
			DisplaySongInfo();
			// set up handler for when app resumes from background
			NSNotificationCenter.DefaultCenter.AddObserver(this, new Selector("resumeFromBackground"), UIApplication.DidBecomeActiveNotification, null);
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			this.NavigationController.NavigationBar.BackgroundColor = UIColor.DarkGray;
			this.NavigationController.NavigationBar.BarTintColor = UIColor.DarkGray;
			this.NavigationController.NavigationBar.TintColor = UIColor.LightGray;
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			// Create and initialize music player
			musicPlayer = new MyMusicPlayer(this);
			// Play song (and load all songs by artist to player queue)
			musicPlayer.playSongWithId(song.songID);
			// Register for receiving controls from lock screen and controlscreen
			UIApplication.SharedApplication.BeginReceivingRemoteControlEvents();
			this.BecomeFirstResponder();
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			// Clear the music player
			musicPlayer.clear();
			// Unregister for control events
			UIApplication.SharedApplication.EndReceivingRemoteControlEvents();
			this.ResignFirstResponder();
			// Clear the music players reference back to this class - avoid retain cycle
			musicPlayer.dvc = null;
			this.NavigationController.NavigationBar.BackgroundColor = UIColor.White;
			this.NavigationController.NavigationBar.BarTintColor = UIColor.White;
			this.NavigationController.NavigationBar.TintColor = UIColor.Blue;
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
			DisplaySongInfo();
		}

		// Forward remote control received events, from the lock or control screen, to the music player.
		public override void RemoteControlReceived(UIEvent theEvent)
		{
			base.RemoteControlReceived(theEvent);
			musicPlayer.RemoteControlReceived(theEvent);
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
			musicPlayer.PreviousTrack();
		}

		partial void nextBtnTapped(UIButton sender)
		{
			musicPlayer.NextTrack();
		}
		#endregion

		#region - Class helper methods
		public void DisplaySongInfo()
		{
			// Display info for current song as might've changed
			artistNameLabel.Text = song.artist;
			albumNameLabel.Text = song.album;
			songTitleLabel.Text = song.song;
			songIdLabel.Text = song.songID.ToString();
			artworkView.Image = song.artwork.ImageWithSize(new SizeF(115.0f, 115.0f));
		}
		#endregion
	}
}

