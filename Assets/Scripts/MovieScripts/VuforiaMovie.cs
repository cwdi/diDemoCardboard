using UnityEngine;
using System.Collections;


// This needs a tracker class as well to function ... but is not included here as it will not compile without vuforia.
// For a vuforia project see http://www.predictions-software.com/
// Copyright Predictions Software Ltd 2014

public class VuforiaMovie : VideoPlayer {
	private string currentInformation;
	
	public void OnTrackingFound(string information) {
		if(currentInformation == information) {
			base.Resume();
		} else {
			currentInformation = information;
			base.Stop ();
			string movie=FindMovieToPlay(information);
			base.PlayTexture(movie);
		}
	}

	public void OnTrackingLost() {
			base.Pause();
	}

	private string FindMovieToPlay(string information) {
		if(information == "Stones"  || information == "stones" ) {// cloud is Stones, inbuild target is stones.
			return "https://devimages.apple.com.edgekey.net/streaming/examples/bipbop_16x9/bipbop_16x9_variant.m3u8";
			//	return "http://mirrorblender.top-ix.org/peach/bigbuckbunny_movies/big_buck_bunny_480p_h264.mov";
		} else {
			return "https://devimages.apple.com.edgekey.net/streaming/examples/bipbop_16x9/bipbop_16x9_variant.m3u8";
			//return "trailer_480p.mov";
		}
	}
	


}
