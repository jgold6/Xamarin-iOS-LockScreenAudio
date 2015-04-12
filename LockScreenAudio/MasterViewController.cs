// Loosely based on this guide: http://www.sagorin.org/ios-playing-audio-in-background-audio/
// and sample in Obj-C: https://github.com/jsagorin/iOSBackgroundAudio

// branch: streamaudio

using System;
using CoreGraphics;
using System.Collections.Generic;

using Foundation;
using UIKit;
using MediaPlayer;
using System.Linq;
using ObjCRuntime;

namespace LockScreenAudio
{
	public partial class MasterViewController : UITableViewController
	{
		private UISearchBar searchBar;
		private UISearchDisplayController searchController;
		private UIBarButtonItem leftBBI;

		#region - Constructors
		public MasterViewController(IntPtr handle) : base(handle)
		{
		}
		#endregion

		#region - View Controller overrides
		public override void AwakeFromNib()
		{
			base.AwakeFromNib();
		}

		public override void DidReceiveMemoryWarning()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning();
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			leftBBI = new UIBarButtonItem("Stream a song", UIBarButtonItemStyle.Bordered, this, new Selector("StreamSong:"));
			this.NavigationItem.LeftBarButtonItem = leftBBI;
			this.TableView.SectionIndexTrackingBackgroundColor = UIColor.FromRGB(0.9f, 0.9f, 0.9f);

			tableView.Source = new EmptyTableViewSource();
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			if (searchBar == null) {
				Songs.querySongs();
				this.Title = String.Format("Songs ({0}) by Artist ({1})", Songs.songCount, Songs.artistCount);

				searchBar = new UISearchBar(new CGRect (0, 0, 320, 44)) {
					Placeholder = "Search",
					AutocorrectionType = UITextAutocorrectionType.No,
					AutocapitalizationType = UITextAutocapitalizationType.None
				};
				searchBar.SizeToFit();

				TableView.TableHeaderView = searchBar;
				// Alternate - display artist/song count in table header view and the search bar in the Navigation bar
	//			TableView.TableHeaderView = new UILabel(new RectangleF(0.0f, 0.0f, TableView.Frame.Width, 30.0f)){
	//				Text = String.Format("Songs ({0}) by Artist ({1})", Songs.songCount, Songs.artistCount),
	//				AutoresizingMask = UIViewAutoresizing.All
	//			};

				searchController = new UISearchDisplayController(searchBar, this);
	//			searchController.DisplaysSearchBarInNavigationBar = true;

				WeakReference mvc = new WeakReference(this);
				tableView.Source = new ArtistSongTableViewSource(mvc);
				searchController.WeakDelegate = this;
				searchController.SearchResultsSource = tableView.Source;
				tableView.ReloadData();
				searchBar.UserInteractionEnabled = true;
			}
		}

		// Get ready to segue to the detail view controller
		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			// The segue to use
			if (segue.Identifier == "showDetail") {
				DetailViewController detailVC = segue.DestinationViewController as DetailViewController;
				if (sender == leftBBI) {
					Song song = new Song();
					song.song = "Crocodile Tears";
					song.album = "Johnny Gold";
					song.artist = "Johnny Gold";
					song.duration = 232.0;
					song.streamingURL = "http://johnnygold.com/music/croctears.mp3";
					// Pass song info to the detail view controller
					detailVC.song = song;
				}
				else if (!Songs.searching) {
					// Selected song
					NSIndexPath indexPath = this.TableView.IndexPathForSelectedRow;
					Song song = Songs.GetSongBySectionRow(indexPath.Section, indexPath.Row);
					song.streamingURL = null;
					// Pass song info to the detail view controller
					detailVC.song = song;
				}
				else {
					// Selected song
					UITableView tv = sender as UITableView;
					NSIndexPath indexPath = tv.IndexPathForSelectedRow;
					Song song = Songs.GetSongBySectionRow(indexPath.Section, indexPath.Row);
					song.streamingURL = null;
					// Pass song info to the detail view controller
					detailVC.song = song;
				}
			}
		}

		public override bool ShouldAutorotate()
		{
			return false;
		}

		#endregion

		#region - class helper methods
		[Export("StreamSong:")]
		public void StreamSong(UIBarButtonItem sender)
		{
			PerformSegue("showDetail", sender);
		}

		#endregion

		#region - Search WeakDelegate
		[Export ("searchDisplayControllerWillBeginSearch:")]
		public void WillBeginSearch(UISearchDisplayController controller)
		{
			Songs.searching = true;
			Songs.FilterContentsForSearch("");
		}

		[Export ("searchDisplayController:shouldReloadTableForSearchString:")]
		public bool ShouldReloadForSearchString(UISearchDisplayController controller, string forSearchString)
		{
			Songs.FilterContentsForSearch(forSearchString);
			return true;
		}

		[Export ("searchDisplayControllerDidEndSearch:")]
		public void DidEndSearch(UISearchDisplayController controller)
		{
			Songs.searching = false;
		}
		#endregion
	}
}

