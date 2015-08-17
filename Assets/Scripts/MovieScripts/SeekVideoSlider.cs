using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SeekVideoSlider : MonoBehaviour {
	
	public VideoPlayer videoPlayer;
	public Slider seekSlider;
	public Slider bufferSlider;
	private float sliderValue=0.0f;
	
	void Start () {
		if(videoPlayer== null) videoPlayer = gameObject.GetComponent<VideoPlayer>();
		if(videoPlayer == null) Debug.Log("Need VideoPlayer assigned to or on gameObject " + gameObject.name); 

	}

	void Update () {
		bufferSlider.value=videoPlayer.loadedBufferFraction;
		sliderValue=videoPlayer.currentTime/videoPlayer.duration;
		seekSlider.value=sliderValue;
	}

	// value 0 to 1.0. 
	public void SliderValue (float value)
	{
		if (Application.platform != RuntimePlatform.OSXEditor) {
			if(value != sliderValue) { // was changed manually
				videoPlayer.seekTo(value*videoPlayer.duration);
			}
		}
	}

}
