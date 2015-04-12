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
	public class ArtistSongTableViewSource : UITableViewSource
	{
		MasterViewController mvc;

		public ArtistSongTableViewSource(MasterViewController m)
		{
			mvc = m;
		}

		#region - Table View data source overrides
		public override nint NumberOfSections(UITableView tableView)
		{
			if (Songs.searching)
				return Songs.searchArtistCount;
			else
				return Songs.artistCount;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return Songs.GetSongsByArtistIndex((int)section).Count;
		}

		string[] alphabet = new string[]{"A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"};
		string[] numbers = new string[]{"0","1","2","3","4","5","6","7","8","9"};

		//Provide an index in the table for quick scrolling
		public override String[] SectionIndexTitles(UITableView tableView)
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
			UILabel headerLabel = new UILabel();
			headerLabel.Frame = new CGRect(40.0f, 5.0f, tableView.Frame.Width -60.0f, 20.0f);
			headerLabel.Font = UIFont.PreferredHeadline;
			headerLabel.Text = TitleForHeader(tableView, section);
			headerLabel.TextColor = UIColor.White;

			UIImageView artworkView = new UIImageView(new CGRect(0.0f, 0.0f, 30.0f, 30.0f));
			NSIndexPath indexPath = NSIndexPath.FromRowSection(0, section);
			Song song = Songs.GetSongBySectionRow(indexPath.Section, indexPath.Row);
			if (song.artwork != null)
				artworkView.Image = song.artwork.ImageWithSize(new CGSize(30.0f, 30.0f));

			UIView headerView = new UIView(new CGRect(0, 0, tableView.Frame.Width, 30));
			headerView.BackgroundColor = UIColor.DarkGray;
			headerView.Add(headerLabel);
			headerView.Add(artworkView);

			return headerView;
		}

		public override nfloat GetHeightForHeader(UITableView tableView, nint section)
		{
			return 30.0f;
		}

		// Get the text for the section header view 
		public override string TitleForHeader(UITableView tableView, nint section)
		{
			return Songs.GetArtistByIndex((int)section);
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

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			if (Songs.searching)
				mvc.PerformSegue("showDetail", tableView);
		}
	}
}

