using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Net;
using MonoTouch.AudioToolbox;
using System.IO;
using System.Threading;
using System.Diagnostics;
using MonoTouch.AVFoundation;
using MonoTouch.MediaPlayer;
using MonoTouch.ObjCRuntime;

namespace LockScreenAudio
{
	[Preserve (AllMembers = true)] 
	public partial class PlayerViewController : UIViewController
	{
		private AVPlayerItem streamingItem;
		private AVPlayer player;

		public string SourceUrl { get; private set; }

		public PlayerOption PlayerOption { get; private set; }

		public bool IsPlaying { get; private set; }

		public PlayerViewController (PlayerOption playerOption, string sourceUrl) : base ("PlayerViewController", null)
		{
			PlayerOption = playerOption;
			SourceUrl = sourceUrl;
			AVAudioSession avSession = AVAudioSession.SharedInstance();

			avSession.SetCategory(AVAudioSessionCategory.Playback);

			NSError activationError = null;
			avSession.SetActive(true, out activationError);
			if (activationError != null)
				Console.WriteLine("Could not activate audio session {0}", activationError.LocalizedDescription);

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.View = View;
			volumeSlider.TouchUpInside += SetVolume;
			playPauseButton.TouchUpInside += PlayPauseButtonClickHandler;
		}

		private void SetVolume (object sender, EventArgs e)
		{
			if (player == null)
				return;

			player.Volume = volumeSlider.Value;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			playPauseButton.UserInteractionEnabled = false;
			playPauseButton.TintColor = UIColor.LightGray;
			// try different approach
			NSUrl url = NSUrl.FromString("http://ccmixter.org/content/bradstanfield/bradstanfield_-_People_Let_s_Stop_The_War.mp3");
			streamingItem = AVPlayerItem.FromUrl(url);
			player = AVPlayer.FromPlayerItem(streamingItem);
			streamingItem.AddObserver(this, new NSString("status"), NSKeyValueObservingOptions.Initial, player.Handle);
			//NSNotificationCenter.DefaultCenter.AddObserver(this, new Selector("playerItemDidReachEnd:"), AVPlayerItem.DidPlayToEndTimeNotification, streamingItem);

			Title = "Streaming";
			playPauseButton.TitleLabel.Text = "Pause";
			timeLabel.Text = string.Empty; 

			UIApplication.SharedApplication.BeginReceivingRemoteControlEvents();
			this.BecomeFirstResponder();
		}
			
		public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			Console.WriteLine("Status Observed Method {0}", player.Status);
			if (player.Status == AVPlayerStatus.ReadyToPlay) {
				playPauseButton.UserInteractionEnabled = true;
				playPauseButton.TintColor = UIColor.Blue;
				MPNowPlayingInfo np = new MPNowPlayingInfo();
				np.AlbumTitle = "brad stanfield";
				np.Artist = "brad stanfield";
				np.Title = "People Let's Stop the War";
				np.PlaybackDuration = streamingItem.Duration.Seconds;
				MPNowPlayingInfoCenter.DefaultCenter.NowPlaying = np;
				player.Play();
			}
			else if (player.Status == AVPlayerStatus.Failed) {
				Title = "Stream Failed";
			}
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
			player.Pause();
			streamingItem.RemoveObserver(this, new NSString("status"));
			player.Dispose();
			UIApplication.SharedApplication.EndReceivingRemoteControlEvents();
			this.ResignFirstResponder();
		}

		private void PlayPauseButtonClickHandler (object sender, EventArgs e)
		{
			if (player.Rate > 0.0f) {
				player.Pause();
				playPauseButton.SetTitle("Play", UIControlState.Normal);
			}
			else 
			{
				player.Play();
				playPauseButton.SetTitle("Pause", UIControlState.Normal);
			}
		}

		public override void RemoteControlReceived(UIEvent theEvent)
		{
			if (player == null)
				return;
			base.RemoteControlReceived(theEvent);
			if (theEvent.Subtype == UIEventSubtype.RemoteControlPause) {
				playPauseButton.SetTitle ("Play", UIControlState.Normal);
				player.Pause();
			}
			else if (theEvent.Subtype == UIEventSubtype.RemoteControlPlay) {
				playPauseButton.SetTitle ("Pause", UIControlState.Normal);
				player.Play();
			}
		}
	}
}

