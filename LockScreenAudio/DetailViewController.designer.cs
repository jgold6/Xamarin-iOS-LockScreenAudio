// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace LockScreenAudio
{
	[Register ("DetailViewController")]
	partial class DetailViewController
	{
		[Outlet]
		MonoTouch.UIKit.UILabel albumNameLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel artistNameLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView artworkView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton nextBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton playPause { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton prevBtn { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel songIdLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel songTitleLabel { get; set; }

		[Action ("nextBtnTapped:")]
		partial void nextBtnTapped (MonoTouch.UIKit.UIButton sender);

		[Action ("playPauseButtonTapped:")]
		partial void playPauseButtonTapped (MonoTouch.UIKit.UIButton sender);

		[Action ("prevButtonTapped:")]
		partial void prevButtonTapped (MonoTouch.UIKit.UIButton sender);
		
		void ReleaseDesignerOutlets ()
		{
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

			if (playPause != null) {
				playPause.Dispose ();
				playPause = null;
			}

			if (prevBtn != null) {
				prevBtn.Dispose ();
				prevBtn = null;
			}

			if (nextBtn != null) {
				nextBtn.Dispose ();
				nextBtn = null;
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
