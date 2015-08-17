using UnityEngine;
using System.Collections;


public class Fill : MonoBehaviour {
	
	float lerpTime = 2f;
	float currentLerpTime;
	public menuTeleport menuTeleport;
	
	//float moveDistance = 5f;



	Color startColor = Color.clear;
	Color endColor = Color.clear;


	
	public void Start() {
		startColor = Color.red;
		//endColor = new Color(0.0f,0.0f,1.0f,0.5f);
	}
	
	public void Update() {
	

	
		//reset when we press spacebar
		if (Input.GetKeyDown(KeyCode.Space)) {
		//if (menuTeleport.SetGazedAt()) {
			currentLerpTime = 0f;
			startColor = Color.red;
			endColor = new Color(0.0f,0.0f,1.0f,0.5f);
		}
		
		//increment timer once per frame
		currentLerpTime += Time.deltaTime;
		if (currentLerpTime > lerpTime) {
			currentLerpTime = lerpTime;
		}
		
		//lerp!
		float perc = currentLerpTime / lerpTime;
		GetComponent<Renderer>().material.color = Color.Lerp(startColor, endColor, perc);
	}
}