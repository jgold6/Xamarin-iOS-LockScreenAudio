// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace LockScreenAudio
{
	[Register ("DetailViewController")]
	partial class DetailViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIActivityIndicatorView actIndView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel albumNameLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel artistNameLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView artworkView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton nextBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton playPause { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton prevBtn { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel songIdLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel songTitleLabel { get; set; }

		[Action ("nextBtnTapped:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void nextBtnTapped (UIButton sender);

		[Action ("playPauseButtonTapped:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void playPauseButtonTapped (UIButton sender);

		[Action ("prevButtonTapped:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void prevButtonTapped (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (actIndView != null) {
				actIndView.Dispose ();
				actIndView = null;
			}
			if (albumNameLabel != null) {
				albumNameLabel.Dispose ();
				albumNameLabel = null;
			}
			if (artistNameLabel != null) {
				artistNameLabel.Dispose ();
				artistNameLabel = null;
			}
			if (artworkView != null) {
				artworkView.Dispose ();
				artworkView = null;
			}
			if (nextBtn != null) {
				nextBtn.Dispose ();
				nextBtn = null;
			}
			if (playPause != null) {
				playPause.Dispose ();
				playPause = null;
			}
			if (prevBtn != null) {
				prevBtn.Dispose ();
				prevBtn = null;
			}
			if (songIdLabel != null) {
				songIdLabel.Dispose ();
				songIdLabel = null;
			}
			if (songTitleLabel != null) {
				songTitleLabel.Dispose ();
				songTitleLabel = null;
			}
		}
	}
}
