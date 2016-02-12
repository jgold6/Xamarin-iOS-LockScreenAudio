// Loosely based on this guide: http://www.sagorin.org/ios-playing-audio-in-background-audio/
// and sample in Obj-C: https://github.com/jsagorin/iOSBackgroundAudio

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
		private UIBarButtonItem rightBBI;
		private bool loading;
		private DetailViewController detailViewController;

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
			NavigationItem.LeftBarButtonItem = leftBBI;
			rightBBI = new UIBarButtonItem("Now Playing", UIBarButtonItemStyle.Bordered, this, new Selector("NowPlaying:"));
			NavigationItem.RightBarButtonItem = rightBBI;
			TableView.SectionIndexTrackingBackgroundColor = UIColor.FromRGB(0.9f, 0.9f, 0.9f);

			loading = true;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (!Songs.searching)
				TableView.ReloadRows (TableView.IndexPathsForVisibleRows, UITableViewRowAnimation.None);
			else
				searchController.SearchResultsTableView.ReloadRows (searchController.SearchResultsTableView.IndexPathsForVisibleRows, UITableViewRowAnimation.None);
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			if (searchBar == null) {
				Songs.querySongs();
				this.Title = String.Format("Songs");

				searchBar = new UISearchBar(new CGRect (0, 0, 320, 44)) {
					Placeholder = "Search",
					AutocorrectionType = UITextAutocorrectionType.No,
					AutocapitalizationType = UITextAutocapitalizationType.None
				};
				searchBar.SizeToFit();

				TableView.TableHeaderView = searchBar;

				searchController = new UISearchDisplayController(searchBar, this);

				searchController.WeakDelegate = this;
				searchController.SearchResultsDelegate = this;
				searchController.SearchResultsDataSource = this;
				loading = false;
				TableView.ReloadData();
				searchBar.UserInteractionEnabled = true;
			}
			// Register for receiving controls from lock screen and controlscreen
			UIApplication.SharedApplication.BeginReceivingRemoteControlEvents();
			this.BecomeFirstResponder ();
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			// Unregister for control events
			UIApplication.SharedApplication.EndReceivingRemoteControlEvents();
			this.ResignFirstResponder();
		}

		// Get ready to segue to the detail view controller
		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			// The segue to use
			if (segue.Identifier == "showDetail") {
				detailViewController = segue.DestinationViewController as DetailViewController;
				if (sender == leftBBI) {
					Song song = Songs.GetStreamingSongByIndex (0);
					// Pass song info to the detail view controller
					detailViewController.song = song;
				}
				else if (sender == rightBBI) {
					Song playingSong = MyMusicPlayer.GetInstance ().currentSong;
					detailViewController.song = playingSong;
				}
				else {
					// Selected song
					NSIndexPath indexPath = Songs.searching ? searchController.SearchResultsTableView.IndexPathForSelectedRow : TableView.IndexPathForSelectedRow;
					Song song = Songs.GetSongBySectionRow(indexPath.Section, indexPath.Row);
					song.streamingURL = null;
					// Pass song info to the detail view controller
					detailViewController.song = song;
				}
			}
		}

		public override bool ShouldAutorotate()
		{
			return false;
		}

		#endregion

		#region - Handle events from outside the app
		// Forward remote control received events, from the lock or control screen, to the music player.
		public override void RemoteControlReceived(UIEvent theEvent)
		{
			base.RemoteControlReceived(theEvent);
			if (theEvent.Subtype == UIEventSubtype.RemoteControlPreviousTrack) {
				detailViewController.PlayPrevSong();
			}
			else if (theEvent.Subtype == UIEventSubtype.RemoteControlNextTrack) {
				detailViewController.PlayNextSong();
			}
			else {
				detailViewController.musicPlayer.RemoteControlReceived(theEvent);
			}
			if (!Songs.searching)
				TableView.ReloadRows (TableView.IndexPathsForVisibleRows, UITableViewRowAnimation.None);
			else
				searchController.SearchResultsTableView.ReloadRows (searchController.SearchResultsTableView.IndexPathsForVisibleRows, UITableViewRowAnimation.None);
		}
		#endregion


		#region - class helper methods
		[Export("StreamSong:")]
		public void StreamSong(UIBarButtonItem sender)
		{
			PerformSegue("showDetail", sender);
		}

		[Export("NowPlaying:")]
		public void NowPlaying(UIBarButtonItem sender)
		{
			if (MyMusicPlayer.GetInstance ().currentSong != null)
				PerformSegue("showDetail", sender);
			else {
				UIAlertController ac = UIAlertController.Create ("No songs playing", "Please select a song to play", UIAlertControllerStyle.Alert);
				UIAlertAction defaultAction = UIAlertAction.Create ("OK", UIAlertActionStyle.Cancel, null);
				ac.AddAction (defaultAction);

				PresentViewController	(ac, true, null);
			}
		}
		#endregion

		#region - Table View data source overrides
		public override nint NumberOfSections(UITableView tableView)
		{
			if (loading)
				return 1;
			else if (Songs.searching)
				return Songs.searchArtistCount;
			else
				return Songs.artistCount;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			if (loading)
				return 1;
			else
				return Songs.GetSongsByArtistIndex((int)section).Count;
		}

		string[] alphabet = new string[]{"A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"};
		string[] numbers = new string[]{"0","1","2","3","4","5","6","7","8","9"};

		//Provide an index in the table for quick scrolling
		public override String[] SectionIndexTitles(UITableView tableView)
		{
			if (loading)
				return null;
			
			List<string> index = new List<string>();
			List<string> artists = Songs.GetListOfArtists();
			string lastChar = "";

			foreach (string artist in artists) {
				string initialLetter = "";
				if (artist.Length > 4 && artist.Substring(0,4) == "The ") {
					initialLetter = artist.Substring(4,1).ToUpper();
				}
				else if (artist.Length > 2 && artist.Substring(0,2) == "A ") {
					initialLetter = artist.Substring(2,1).ToUpper();
				}
				else if (artist.Length >1)
					initialLetter = artist.Substring(0,1).ToUpper();

				// Handle numbers first
				if (artist.Length > 1 && numbers.Contains<string>(initialLetter) && !index.Contains("#")) {
					index.Add("#");
					continue;
				}
				// Ignore artists that start with non-alphabetic characters
				if (artist.Length > 1 && !alphabet.Contains<string>(initialLetter)) {
					continue;
				}
				// Add new initial letter to List
				if (initialLetter != lastChar) {
					lastChar = initialLetter;
					index.Add(initialLetter);
				}
			}
			return index.ToArray();
		}

		// Get the section to scroll to when the side index is used
		public override nint SectionFor(UITableView tableView, string title, nint atIndex)
		{
			List<string> artists = Songs.GetListOfArtists();
			int section = 0;

			foreach (string artist in artists) {
				string initialLetter = "";
				if (artist.Length > 4 && artist.Substring(0,4) == "The ") {
					initialLetter = artist.Substring(4,1).ToUpper();
				}
				else if (artist.Length > 2 && artist.Substring(0,2) == "A ") {
					initialLetter = artist.Substring(2,1).ToUpper();
				}
				else if (artist.Length >1)
					initialLetter = artist.Substring(0,1).ToUpper();

				// Ignore non-alphanumeric characters
				if (!alphabet.Contains<string>(initialLetter) && !numbers.Contains<string>(initialLetter)) {
					section++;
					continue;
				}
				// Is this the first artist with the initial letter equal to index letter (title parameter) or a number? 
				if (initialLetter == title || numbers.Contains<string>(initialLetter))
					return section;
				else
					section++;
			}
			return 0;
		}

		// Get the view for the section header - create and style container view, label, and image view
		public override UIView GetViewForHeader(UITableView tableView, nint section)
		{
			if (loading)
				return null;
			
			UILabel headerLabel = new UILabel();
			headerLabel.Frame = new CGRect(40.0f, 5.0f, TableView.Frame.Width -60.0f, 20.0f);
			headerLabel.Font = UIFont.PreferredHeadline;
			headerLabel.Text = TitleForHeader(TableView, section);
			headerLabel.TextColor = UIColor.White;

			UIImageView artworkView = new UIImageView(new CGRect(0.0f, 0.0f, 30.0f, 30.0f));
			NSIndexPath indexPath = NSIndexPath.FromRowSection(0, section);
			Song song = Songs.GetSongBySectionRow(indexPath.Section, indexPath.Row);
			if (song.artwork != null)
				artworkView.Image = song.artwork.ImageWithSize(new CGSize(30.0f, 30.0f));

			UIView headerView = new UIView(new CGRect(0, 0, TableView.Frame.Width, 30));
			headerView.BackgroundColor = UIColor.DarkGray;
			headerView.Add(headerLabel);
			headerView.Add(artworkView);

			return headerView;
		}

		public override nfloat GetHeightForHeader(UITableView tableView, nint section)
		{
			if (loading)
				return 0.0f;

			return 30.0f;
		}

		// Get the text for the section header view 
		public override string TitleForHeader(UITableView tableView, nint section)
		{
			if (loading)
				return "";

			return Songs.GetArtistByIndex((int)section);
		}

		// Set up the table view cells
		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = TableView.DequeueReusableCell("cell");
			if (cell == null)
				cell = new UITableViewCell(UITableViewCellStyle.Subtitle, "cell");

			if (loading) {
				cell.TextLabel.Text = "Loading...";
				cell.DetailTextLabel.Text = "This may take a bit for a large library";
			} else {
				Song song = Songs.GetSongBySectionRow (indexPath.Section, indexPath.Row);
				cell.TextLabel.Text = song.song;
				cell.DetailTextLabel.Text = String.Format ("Album: {0}", song.album);
				if (song == MyMusicPlayer.GetInstance ().currentSong) {
					cell.BackgroundColor = UIColor.FromRGB (0.9f, 0.9f, 0.9f);
				} else {
					cell.BackgroundColor = UIColor.White;
				}
			}

			return cell;
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

