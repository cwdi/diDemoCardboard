using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

// Much modified from original unity code.
// Gerard A. Allan Predictions Software Ltd.
// Copyright 2014 
public class VideoPlayer : MonoBehaviour
{
	public int videoIndex=0;
	public bool loop;
	public bool flipVideoTexture=true;
	private Vector2 lastvideoSize;
	private bool isStop;// used when ---  does not start playing and need get out of loop (stop) anyway.
	
	private delegate void playeriOSCallback(string message);
	static VideoPlayer[] allPlayersStaticArray = {null,null,null,null};// 4 possible players
	private Texture2D _videoTexture;
	private TextureFormat videoTextureFormat = TextureFormat.BGRA32;


	// player rate 1 is normal speed. (when paused is 0.) 
	// To override it is best to use preferredRate as rate with be set to prefered rate at start or after a pause
	public float rate
	{
		get
		{
			return VideoPlayerPluginPlayerRate(videoIndex);
		}
		set 
		{
			VideoPlayerPluginSetPlayerRate(value, videoIndex);
		}
	}

	// Default rate is 1.0 but to have video start/restart at a different rate set the perferredRate
	// As movie can pause and restart under system control (streaming) if you want restart at a non default rate is best to use prefferedRate 
	public float preferredRate

	{
		get
		{
			return VideoPlayerPluginPlayerPreferredPlayRate(videoIndex);
		}
		set 
		{
			VideoPlayerPluginSetPlayerPreferredPlayRate(value, videoIndex);
		}
	}

	// fraction 0 to 1
	public float loadedBufferFraction
	{
		get
		{
			return VideoPlayerPluginPlayerLoadedBuffer(videoIndex);
		}
	}

    public bool ready
    {
        get
        {
            return VideoPlayerPluginPlayerReady(videoIndex);
        }
	}
    public float duration
    {
        get
        {
            return VideoPlayerPluginDurationSeconds(videoIndex);
        }
    }

	public float currentTime
	{
		get
		{
			return VideoPlayerPluginCurrentTimeSeconds(videoIndex);
		}
	}

    public bool isPlaying
    {
        get
        {
            return VideoPlayerPluginIsPlaying(videoIndex);
        }
    }

	public bool isStalled
	{
		get
		{
			return VideoPlayerPluginIsStalled(videoIndex);
		}
	}

	public bool isFileURL
	{
		get
		{
			return VideoPlayerPluginPlayerIsFileURL(videoIndex);
		}
	}

	public bool isStreamURL
	{
		get
		{
			return VideoPlayerPluginPlayerIsStreamURL(videoIndex);
		}
	}

	public bool streamIsHLS
	{
		get
		{
			return VideoPlayerPluginPlayerStreamIsHLS(videoIndex);
		}
	}

    public Vector2 videoSize
    {
        get
        {
            int width = 0, height = 0;
            VideoPlayerPluginExtents(ref width, ref height,videoIndex);
            return new Vector2(width, height);
        }
	}

	public void seekTo(float time) {
		VideoPlayerPluginSeekToVideo(time,videoIndex);
	}
	
    public Texture2D videoTexture
    {
        get
        {
            int nativeTex = ready ? VideoPlayerPluginCurFrameTexture(videoIndex) : 0;

            if (nativeTex != 0)
            {
				if (_videoTexture == null) 
                {
					lastvideoSize= videoSize;
					//Debug.Log("Createtexure " + videoSize + " " + nativeTex);
                    _videoTexture = Texture2D.CreateExternalTexture((int)lastvideoSize.x, (int)lastvideoSize.y, videoTextureFormat,
                        false, false, (IntPtr)nativeTex);
                    _videoTexture.filterMode = FilterMode.Bilinear;
                    _videoTexture.wrapMode = TextureWrapMode.Repeat;
                } 
				_videoTexture.UpdateExternalTexture((IntPtr)nativeTex);
               
            }
            else
            {
				//Debug.Log("NULL Texture");
                _videoTexture = null;
				return null;
            }
            return _videoTexture;
        }
    }
	
    public void PlayTexture(string videoURL)
    {
		int dummyText=1;
		_videoTexture = Texture2D.CreateExternalTexture((int) 16, (int)16, videoTextureFormat,
		                                                false, false, (IntPtr) dummyText);
        PlayTexture(videoURL, null);
    }

	public void PlayTexture(string videoURL, Material material)
	{
		VideoPlayerPluginPlayerCallbacks(playerFinished,playerBufferEmpty,playerLikelyToKeepUp,playerStalled,playerFirstFrameReady,videoIndex);
		VideoPlayerPluginPlayVideo(videoURL, videoIndex);
		StartCoroutine(UpdateTexture(material, false));
	}
	
	public void PlayTextureAtTime(string videoURL,float time)
	{
		int dummyText=1;
		_videoTexture = Texture2D.CreateExternalTexture((int) 16, (int)16, videoTextureFormat,
		                                                false, false, (IntPtr) dummyText);
		VideoPlayerPluginPlayerCallbacks(playerFinished,playerBufferEmpty,playerLikelyToKeepUp,playerStalled,playerFirstFrameReady,videoIndex);
		
		VideoPlayerPluginPlayVideoAtTime(videoURL, time, videoIndex);
		StartCoroutine(UpdateTexture(null, false));
	}

    private Material lastMaterial;
    private IEnumerator UpdateTexture(Material material, bool isResume)
    {
		isStop=false;
        if (isResume)
        {
			//Debug.Log("Resume Coroutine " + videoIndex);
            material = lastMaterial;
        }
        while (!isPlaying)
        {
			//Debug.Log("!play Coroutine " + videoIndex);
            yield return null;
			if(isStop) {
				isStop=false;
				//Debug.Log("Coroutine exit on stop " + videoIndex);
				yield break;//return false;
			}
        }
		//Debug.Log("Coroutine " + videoIndex);
        while (isPlaying)
        {
			Texture2D texture=videoTexture;
			if(texture!=null) {
            if (!material)
            {
                GetComponent<Renderer>().sharedMaterial.mainTexture = videoTexture;
            }
            else
            {
                material.mainTexture = videoTexture;
            }
			}
            yield return new WaitForEndOfFrame();
        }
		isStop=false;
        lastMaterial = material;
		//Debug.Log("Coroutine exit " + videoIndex);
    }

    public void Pause()
    {
		if(isPlaying) {
			//Debug.Log("Pause " + videoIndex);
        	VideoPlayerPluginPauseVideo(videoIndex);
		}
    }

	public void Resume()
	{
		if(ready && !isPlaying) {
			VideoPlayerPluginResumeVideo(videoIndex);
			StartCoroutine(UpdateTexture(null, true));
		}
		
	}

    public void Stop()
    {
       	VideoPlayerPluginStopVideo(videoIndex);
		GetComponent<Renderer>().sharedMaterial.mainTexture = null;
		isStop=true;
    }
	
    public void Rewind()
    {
		VideoPlayerPluginRewindVideo(videoIndex);
    }

	public void playerFinishedCallbackFromDevice()
	{
		if(loop) {
			Rewind ();
			StartCoroutine(UpdateTexture(null, false));
		} else {
			Stop ();
		}
	}

	public void playerFirstFrameReadyCallbackFromDevice()
	{
		print("First Frame Ready " + videoIndex);
	}

	public void playerStalledCallbackFromDevice(bool stalled)
	{
		if(stalled) {
		  print("STALLED " + videoIndex );
			// display hud?
		} else {
			print("UNSTALLED " + videoIndex);
		}
	}

	public void playerLikelyToKeepUpCallbackFromDevice(bool canKeepUp)
	{
		if(canKeepUp) {
			print("Can keep up " + videoIndex);
		} else {
			print("Can NOT keep up " + videoIndex);
		}
	}

	public void playerBufferEmptyCallbackFromDevice(bool empty)
	{
		if(empty) {
			print("Buffer Empty " + videoIndex);
		} else {
			print("Buffer NOT empty " + videoIndex);
		}
	}


	
	void Start() {
		allPlayersStaticArray[videoIndex]=this;
		if(flipVideoTexture) {
			Vector3 scale=transform.localScale;
			scale.x = -scale.x;
			transform.localScale=scale;
		}
	}

	~VideoPlayer() {
		if(videoIndex >= 0 && videoIndex < 4) {
			allPlayersStaticArray[videoIndex]= null;
		}
	}
	
	private bool afterPauseReadTexture;
	void OnApplicationPause(bool pauseStatus) {
		bool willNeedReplaceTexture=VideoPlayerPluginOnApplicationPause(pauseStatus, videoIndex);
		if(pauseStatus) {
			if(willNeedReplaceTexture) {
				isStop=true;
			}
		} else {
			if(willNeedReplaceTexture) {
				print ("video texture accessed");
				int nativeTex = VideoPlayerPluginCurFrameTexture(videoIndex);
				_videoTexture.UpdateExternalTexture((IntPtr)nativeTex);
				GetComponent<Renderer>().sharedMaterial.mainTexture = _videoTexture;
			}
		}
	}

	#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern float VideoPlayerPluginPlayerPreferredPlayRate(int index);
	[DllImport("__Internal")]
	private static extern void VideoPlayerPluginSetPlayerPreferredPlayRate(float rate, int index);
	[DllImport("__Internal")]
	private static extern float VideoPlayerPluginPlayerRate(int index);
	[DllImport("__Internal")]
	private static extern void VideoPlayerPluginSetPlayerRate(float rate, int index);
	[DllImport("__Internal")]
	private static extern float VideoPlayerPluginPlayerLoadedBuffer(int index);
	[DllImport("__Internal")]
	private static extern bool VideoPlayerPluginPlayerReady(int index);
	[DllImport("__Internal")]
	private static extern float VideoPlayerPluginDurationSeconds(int index);
	[DllImport("__Internal")]
	private static extern float VideoPlayerPluginCurrentTimeSeconds(int index);
	[DllImport("__Internal")]
	private static extern void VideoPlayerPluginExtents(ref int width, ref int height, int index);
	[DllImport("__Internal")]
	private static extern int VideoPlayerPluginCurFrameTexture(int index);
	[DllImport("__Internal")]
	private static extern void VideoPlayerPluginPlayVideo(string videoURL,int index);
	[DllImport("__Internal")]
	private static extern void VideoPlayerPluginPlayVideoView(int left, int top, int right, int bottom, string videoURL);
	[DllImport("__Internal")]
	private static extern void VideoPlayerPluginPlayVideoAtTimeView(int left, int top, int right, int bottom, string videoURL, float time);
	[DllImport("__Internal")]
	private static extern void VideoPlayerPluginSeekVideoAtTime(string videoURL,float time, int index);
	[DllImport("__Internal")]
	private static extern  void VideoPlayerPluginPlayVideoAtTime(string videoURL,float time, int index);
	[DllImport("__Internal")]
	private static extern void VideoPlayerPluginPauseVideo(int index);
	[DllImport("__Internal")]
	private static extern void VideoPlayerPluginResumeVideo(int index);
	[DllImport("__Internal")]
	private static extern void VideoPlayerPluginRewindVideo(int index);
	[DllImport("__Internal")]
	private static extern void VideoPlayerPluginSeekToVideo(float time,int index);
	[DllImport("__Internal")]
	private static extern bool VideoPlayerPluginIsPlaying(int index);
	[DllImport("__Internal")]
	private static extern bool VideoPlayerPluginIsStalled(int index);
	[DllImport("__Internal")]
	private static extern void VideoPlayerPluginStopVideo(int index);
	[DllImport("__Internal")]
	private static extern void VideoPlayerPluginVolume(float volume, int index);
	[DllImport("__Internal")]
	private static extern bool VideoPlayerPluginPlayerIsFileURL(int index);
	[DllImport("__Internal")]
	private static extern bool VideoPlayerPluginPlayerStreamIsHLS(int index);
	[DllImport("__Internal")]
	private static extern bool VideoPlayerPluginPlayerIsStreamURL(int index);
	[DllImport("__Internal")]
	private static extern void VideoPlayerPluginPlayerCallbacks(playeriOSCallback playerFinish, 
	                                                            playeriOSCallback playerBufferEmpty,
	                                                            playeriOSCallback playerLikelyToKeepUp,
	                                                            playeriOSCallback playerIsStalled,
	                                                            playeriOSCallback playerFirstFrameReady,
	                                                            int index);
	[DllImport("__Internal")]
	private static extern bool VideoPlayerPluginOnApplicationPause(bool status,int index);
#else
	private static float VideoPlayerPluginPlayerPreferredPlayRate(int index){return 1.0f;}
	private static void VideoPlayerPluginSetPlayerPreferredPlayRate(float rate, int index) {}
	private static float VideoPlayerPluginPlayerRate(int index){return 1.0f;}
	private static void VideoPlayerPluginSetPlayerRate(float rate, int index) {}
	private static float VideoPlayerPluginPlayerLoadedBuffer(int index){return 0.5f;}
	private static bool VideoPlayerPluginPlayerReady(int index) { return false; }
	private static float VideoPlayerPluginDurationSeconds(int index) { return 1.0f; }
	private static float VideoPlayerPluginCurrentTimeSeconds(int index) { return 0.5f; }
	private static void VideoPlayerPluginExtents(ref int width, ref int height,int index) { }
	private static int VideoPlayerPluginCurFrameTexture(int index) { return 0; }
	private static void VideoPlayerPluginPlayVideo(string videoURL,int index) { }
	private static void VideoPlayerPluginPlayVideoView(int left, int top, int right, int bottom, string videoURL) { }
	private static void VideoPlayerPluginPlayVideoAtTimeView(int left, int top, int right, int bottom, string videoURL, float time) {}
	private static void VideoPlayerPluginPlayVideoAtTime(string videoURL,float time, int index) {}
	private static void VideoPlayerPluginSeekVideoAtTime(string videoURL,float time, int index) {}
	private static void VideoPlayerPluginPauseVideo(int index) { }
	private static void VideoPlayerPluginResumeVideo(int index) { }
	private static void VideoPlayerPluginRewindVideo(int index) { }
	private static void VideoPlayerPluginSeekToVideo(float time,int index) { }
	private static bool VideoPlayerPluginIsPlaying(int index) { return false; }
	private static bool VideoPlayerPluginIsStalled(int index) {return false;}
	private static void VideoPlayerPluginStopVideo(int index) { }
	private static void VideoPlayerPluginVolume(float volume, int index) {}
	private static bool VideoPlayerPluginPlayerIsFileURL(int index) {return false;}
	private static bool VideoPlayerPluginPlayerStreamIsHLS(int index) {return false;}
	private static bool VideoPlayerPluginPlayerIsStreamURL(int index) {return false;}
	private static void VideoPlayerPluginPlayerCallbacks(playeriOSCallback playerFinish, 
	                                                     playeriOSCallback playerBufferEmpty,
	                                                     playeriOSCallback playerLikelyToKeepUp,
	                                                     playeriOSCallback playerIsStalled,
	                                                     playeriOSCallback playerFirstFrameReady,
	                                                     int index){}
	private static bool VideoPlayerPluginOnApplicationPause(bool status,int index) {return false;}
	#endif


	[MonoPInvokeCallback(typeof(playeriOSCallback))]
	protected static void playerFirstFrameReady(string message) {
		int index = Convert.ToInt32(message);
		if(index >= 0 && index < 4) {
			if(allPlayersStaticArray[index] != null) {
				allPlayersStaticArray[index].playerFirstFrameReadyCallbackFromDevice();
			}
		}
	}

	[MonoPInvokeCallback(typeof(playeriOSCallback))]
	protected static void playerFinished(string message) {
		int index = Convert.ToInt32(message);
		if(index >= 0 && index < 4) {
			if(allPlayersStaticArray[index] != null) {
				allPlayersStaticArray[index].playerFinishedCallbackFromDevice();
			}
		}
	}
	
	[MonoPInvokeCallback(typeof(playeriOSCallback))]
	protected static void playerBufferEmpty(string message) {
		string[] words = message.Split(' ');
		int index = Convert.ToInt32(words[0]);
		int boolVal=Convert.ToInt32(words[1]);
		if(index >= 0 && index < 4) {
			if(allPlayersStaticArray[index] != null) {
				allPlayersStaticArray[index].playerBufferEmptyCallbackFromDevice((boolVal > 0));
			}
		}
	}
	
	[MonoPInvokeCallback(typeof(playeriOSCallback))]
	protected static void playerLikelyToKeepUp(string message) {
		string[] words = message.Split(' ');
		int index = Convert.ToInt32(words[0]);
		int boolVal=Convert.ToInt32(words[1]);
		if(index >= 0 && index < 4) {
			if(allPlayersStaticArray[index] != null) {
				allPlayersStaticArray[index].playerLikelyToKeepUpCallbackFromDevice((boolVal > 0));
			}
		}
	}
	
	[MonoPInvokeCallback(typeof(playeriOSCallback))]
	protected static void playerStalled(string message) {
		string[] words = message.Split(' ');
		int index = Convert.ToInt32(words[0]);
		int boolVal=Convert.ToInt32(words[1]);
		if(index >= 0 && index < 4) {
			if(allPlayersStaticArray[index] != null) {
				allPlayersStaticArray[index].playerStalledCallbackFromDevice((boolVal > 0));
			}
		}
	}
}
