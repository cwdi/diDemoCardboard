using UnityEngine;

// Update rotates gameObject
public class TestVideoPlayerGreen : MonoBehaviour
{
		private VideoPlayer video;

		void Start ()
		{
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
						if (video.videoIndex == 0)
							video.PlayTexture ("LaurenGreenScreenOuttakes4.mp4");
						else
							video.PlayTexture ("alpha_vidAV.mov");
				                  // "trailer_480p.mov");
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
