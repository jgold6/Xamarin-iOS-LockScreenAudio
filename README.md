Xamarin-iOS-LockScreenAudio - master
===========================

A sample Xamarin.iOS app that loads the music from the user's media library into a list view. Selecting a song from the list view loads the song into the detail view, starts playing it, and also loads all of that artists songs into the media player queue, in the same order as in the list view. Once the song is playing, song information is displayed on the lock screen and on the control panel (swipe up from bottom), and you can use the controls on the lock and control screens to control the audio. It also demonstrates search functionality using a UISearchBar and a UISearchDisplayController 

This sample is loosely based on the Objective-C guide and sample I found at:
http://www.sagorin.org/ios-playing-audio-in-background-audio/
The objective-C sample app can be found at:
https://github.com/jsagorin/iOSBackgroundAudio

This sample demonstrates using the MPMediaQuery (with filters), AVPlayer, AVAudioSession, and the MPNowPlayingInfo API's. In addition it demonstrates setting up a quick scroll index when there are multiple sections for each index, in other words there are A-Z indexes but of course there may be multiple artists with the same first letter, so this shows one way to implement the indexing in this scenario. And, of course, the search functionality.

I hope you find this useful!
