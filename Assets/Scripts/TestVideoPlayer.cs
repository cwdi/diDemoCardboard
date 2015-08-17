using UnityEngine;

public class TestVideoPlayer : MonoBehaviour
{
	private VideoPlayer video;
	public string videoForIndex0 = "http://www.predictions-software.com/trailer_480pF.mov";
	//local file "trailer_480p.mov"; 
	//full movie -- sometimes slow connection  "http://download.blender.org/peach/bigbuckbunny_movies//big_buck_bunny_480p_h264.mov";

	public string videoForIndex1 = "https://devimages.apple.com.edgekey.net/streaming/examples/bipbop_16x9/bipbop_16x9_variant.m3u8";
	
	void Start ()
	{
			//if you want  you can have add the player onto a game object  gameObject.AddComponent<VideoPlayer>();
			video = GetComponent<VideoPlayer> ();// find a video player on this game object
	}
	
	private void OnGUI ()
	{
		if (video.videoIndex == 1) {
			GUILayout.Space (600); // second players controls -- move down
		}
		
		GUILayout.BeginHorizontal (GUILayout.Width (Screen.width));
		
		if (GUILayout.Button ("<size=21>TexturePlay</size>", GUILayout.Height (100))) {
			video.Stop ();
			if (video.videoIndex == 0) {
				video.PlayTexture(videoForIndex0);
			}
			else
				video.PlayTexture (videoForIndex1);
		}
		GUILayout.EndHorizontal ();
		
		GUILayout.BeginHorizontal (GUILayout.Width (Screen.width));
		
		if (GUILayout.Button ("<size=21>Pause</size>", GUILayout.Height (100))) {
			video.Pause ();
		}
		if (GUILayout.Button ("<size=21>Resume</size>", GUILayout.Height (100))) {
			video.Resume ();
		}
		if (GUILayout.Button ("<size=21>Stop</size>", GUILayout.Height (100))) {
			video.Stop ();
		}
		GUILayout.EndHorizontal ();
		{
			
			GUILayout.Label ("isPlay" + video.isPlaying + " " + video.videoIndex);
			GUILayout.Label ("Ready " + video.ready);
			GUILayout.Label ("Duration " + video.duration + "");
			GUILayout.Label ("CurrentTime " + video.currentTime + "");
			GUILayout.Label ("VideoSize " + video.videoSize + "");
		}
	}
	
}
