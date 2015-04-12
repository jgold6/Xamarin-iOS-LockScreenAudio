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
	public class EmptyTableViewSource : UITableViewSource
	{
		#region - Table View data source overrides
		public override nint NumberOfSections(UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return 1;
		}

		// Set up the table view cells
		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			
			UITableViewCell cell = tableView.DequeueReusableCell("cell");
			if (cell == null)
				cell = new UITableViewCell(UITableViewCellStyle.Subtitle, "cell");
			cell.TextLabel.Text = "Loading...";
			cell.DetailTextLabel.Text = "This may take a bit for a large library";
			return cell;
		}
		#endregion
	}
}

