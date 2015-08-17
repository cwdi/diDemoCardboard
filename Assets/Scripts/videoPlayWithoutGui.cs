using UnityEngine;

public class videoPlayWithoutGui : MonoBehaviour
{
	private VideoPlayer video;
	public string videoForIndex0 = "BlochShort.mov";
	//local file "trailer_480p.mov"; 
	//full movie -- sometimes slow connection  "http://download.blender.org/peach/bigbuckbunny_movies//big_buck_bunny_480p_h264.mov";
	
	public string videoForIndex1 = "https://devimages.apple.com.edgekey.net/streaming/examples/bipbop_16x9/bipbop_16x9_variant.m3u8";
	
	void Start ()
	{
		//if you want  you can have add the player onto a game object  gameObject.AddComponent<VideoPlayer>();
		video = GetComponent<VideoPlayer> ();// find a video player on this game object
		if (video.videoIndex == 0) {
			video.PlayTexture(videoForIndex0);
		}
		else
			video.PlayTexture (videoForIndex1);
	}
	}
	
	
	

