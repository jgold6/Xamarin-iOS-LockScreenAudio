using System;
using MediaPlayer;

namespace LockScreenAudio
{
	public class Song
	{
		public string artist {get; set;}
		public string album {get; set;}
		public string song {get; set;}
		public string streamingURL {get; set;}
		public ulong songID {get; set;}
		public double duration {get; set;}
		public MPMediaItemArtwork artwork { get; set;}

		public Song()
		{
		}
	}
}

