using System;
using System.Drawing;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.MediaPlayer;
using System.Linq;

namespace LockScreenAudio
{
	public partial class MasterViewController : UITableViewController
	{
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
			this.TableView.ReloadData();
		}

		// Get ready to segue to the detail view controller
		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			// The segue to use
			if (segue.Identifier == "showDetail") {
				// Selected song
				NSIndexPath indexPath = this.TableView.IndexPathForSelectedRow;

				Song song = GetSong(indexPath);

				// Pass song info to the detail view controller
				DetailViewController detailVC = segue.DestinationViewController as DetailViewController;
				detailVC.song = song;
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
			return Songs.artistSongs.Keys.Count;
		}

		public override int RowsInSection(UITableView tableview, int section)
		{
			var artists = Songs.artistSongs.Values;
			int index = 0;
			foreach (List<Song> value in artists) {
				if (index == section)
					return value.Count;
				else
					index++;
			}
			return 0;
		}

		//Provide an index in the table for quick scrolling
		public override string[] SectionIndexTitles(UITableView tableView)
		{
			List<string> index = new List<string>();
			var artists = Songs.artistSongs.Keys.ToList().OrderBy(x => x.Substring(0,1));
			string lastChar = "";
			foreach (string artist in artists) {
				// Ignore artists that start with "The " and "A " and "'"
				if ((artist.Length > 4 && artist.Substring(0,4) == "The ") || 
					(artist.Length > 2 && artist.Substring(0,2) == "A ") || 
					(artist.Length > 1 && artist.Substring(0,1) == "'"))
					continue;
				// Add new initial letter to List
				if (artist.Substring(0,1).ToUpper() != lastChar) {
					lastChar = artist.Substring(0,1).ToUpper();
					index.Add(lastChar);
				}
			}
			return index.ToArray();
		}

		// Get the section to scroll to when the side index is used
		public override int SectionFor(UITableView tableView, string title, int atIndex)
		{
			var artists = Songs.artistSongs.Keys;
			int section = 0;
			foreach (string artist in artists) {
				// Ignore artists that start with "The " and "A " and "'"
				if ((artist.Length > 4 && artist.Substring(0,4) == "The ") || 
					(artist.Length > 2 && artist.Substring(0,2) == "A ") || 
					(artist.Length > 1 && artist.Substring(0,1) == "'")) {
					section++;
					continue;
				}
				// Is this the first artist with the initial letter equal to index letter (title parameter)? 
				if (artist.Substring(0,1).ToUpper() == title)
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
			Song song = GetSong(indexPath);
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
			var artists = Songs.artistSongs.Keys;
			int index = 0;
			foreach (string artist in artists) {
				if (index == section) {
					return artist;
				}
				else
					index++;
			}
			return "No Artist";
		}

		// Set up the table view cells
		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell("cell");
			if (cell == null)
				cell = new UITableViewCell(UITableViewCellStyle.Subtitle, "cell");

			Song song = GetSong(indexPath);

			cell.TextLabel.Text = song.song;
			cell.DetailTextLabel.Text = String.Format("Album: {0}", song.album);

			return cell;
		}
		#endregion

		#region - class helper methods
		// Get the song from an index path
		private Song GetSong(NSIndexPath indexPath)
		{
			var artists = Songs.artistSongs.Values;
			int index = 0;
			List<Song> songs = new List<Song>();
			foreach (List<Song> value in artists) {
				if (index == indexPath.Section) {
					songs = value;
					break;
				}
				else
					index++;
			}
			return songs[indexPath.Row];
		}
		#endregion
	}
}

