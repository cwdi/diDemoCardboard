using UnityEngine;
using System.Collections;

public class playController : MonoBehaviour {
	public MovieTexture movTexture;
	public bool loop;
	void Start() {
		GetComponent<Renderer>().material.mainTexture = movTexture;
		movTexture.loop = true;
		movTexture.Play();
	}
}
