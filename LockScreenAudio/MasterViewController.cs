// Loosely based on this guide: http://www.sagorin.org/ios-playing-audio-in-background-audio/
// and sample in Obj-C: https://github.com/jsagorin/iOSBackgroundAudio

// branch: streamaudio

using System;
using System.Drawing;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.MediaPlayer;
using System.Linq;
using MonoTouch.ObjCRuntime;

namespace LockScreenAudio
{
	public enum PlayerOption
	{
		Stream = 0,
		StreamAndSave
	}

	public partial class MasterViewController : UITableViewController
	{
		UIBarButtonItem leftBBI;

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
			Songs.querySongs();
			this.Title = String.Format("Songs ({0}) by Artist ({1})", Songs.songCount, Songs.artistCount);

			leftBBI = new UIBarButtonItem("Stream a song", UIBarButtonItemStyle.Bordered, this, new Selector("StreamSong:"));
			this.NavigationItem.LeftBarButtonItem = leftBBI;
			this.TableView.SectionIndexTrackingBackgroundColor = UIColor.FromRGB(0.9f, 0.9f, 0.9f);
			this.TableView.ReloadData();
		}

		// Get ready to segue to the detail view controller
		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			// The segue to use
			if (segue.Identifier == "showDetail") {
				DetailViewController detailVC = segue.DestinationViewController as DetailViewController;
				if (sender == leftBBI) {
					Song song = new Song();
					song.song = "People Let's Stop the War";
					song.album = "brad stanfield";
					song.artist = "brad stanfield";
					song.duration = 300.0;
					song.streamingURL = "http://ccmixter.org/content/bradstanfield/bradstanfield_-_People_Let_s_Stop_The_War.mp3";
					// Pass song info to the detail view controller
					detailVC.song = song;

				}
				else {
					// Selected song
					NSIndexPath indexPath = this.TableView.IndexPathForSelectedRow;
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

		#region - Table View data source overrides
		public override int NumberOfSections(UITableView tableView)
		{
			return Songs.artistCount;
		}

		public override int RowsInSection(UITableView tableview, int section)
		{
			return Songs.GetSongsByArtistIndex(section).Count;
		}

		string[] alphabet = new string[]{"A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"};
		string[] numbers = new string[]{"0","1","2","3","4","5","6","7","8","9"};

		//Provide an index in the table for quick scrolling
		public override string[] SectionIndexTitles(UITableView tableView)
		{
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
		public override int SectionFor(UITableView tableView, string title, int atIndex)
		{
			List<string> artists = Songs.GetListOfArtists();;
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
		public override UIView GetViewForHeader(UITableView tableView, int section)
		{
			UILabel headerLabel = new UILabel();
			headerLabel.Frame = new RectangleF(40.0f, 5.0f, tableView.Frame.Width -60.0f, 20.0f);
			headerLabel.Font = UIFont.PreferredHeadline;
			headerLabel.Text = TitleForHeader(tableView, section);
			headerLabel.TextColor = UIColor.White;

			UIImageView artworkView = new UIImageView(new RectangleF(0.0f, 0.0f, 30.0f, 30.0f));
			NSIndexPath indexPath = NSIndexPath.FromRowSection(0, section);
			Song song = Songs.GetSongBySectionRow(indexPath.Section, indexPath.Row);
			artworkView.Image = song.artwork.ImageWithSize(new SizeF(30.0f, 30.0f));

			UIView headerView = new UIView();
			headerView.BackgroundColor = UIColor.DarkGray;
			headerView.Add(headerLabel);
			headerView.Add(artworkView);

			return headerView;
		}

		// Get the text for the section header view 
		public override string TitleForHeader(UITableView tableView, int section)
		{
			return Songs.GetArtistByIndex(section);
		}

		// Set up the table view cells
		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell("cell");
			if (cell == null)
				cell = new UITableViewCell(UITableViewCellStyle.Subtitle, "cell");

			Song song = Songs.GetSongBySectionRow(indexPath.Section, indexPath.Row);

			cell.TextLabel.Text = song.song;
			cell.DetailTextLabel.Text = String.Format("Album: {0}", song.album);

			return cell;
		}
		#endregion

		#region - class helper methods
		[Export("StreamSong:")]
		public void StreamSong(UIBarButtonItem sender)
		{
			PerformSegue("showDetail", sender);
		}

		#endregion
	}
}

