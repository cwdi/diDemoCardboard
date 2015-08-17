// Copyright 2014 Google Inc. All rights reserved.
	//
	// Licensed under the Apache License, Version 2.0 (the "License");
	// you may not use this file except in compliance with the License.
	// You may obtain a copy of the License at
	//
	//     http://www.apache.org/licenses/LICENSE-2.0
	//
	// Unless required by applicable law or agreed to in writing, software
	// distributed under the License is distributed on an "AS IS" BASIS,
	// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	// See the License for the specific language governing permissions and
	// limitations under the License.

using UnityEngine;
using System.Collections;





[RequireComponent(typeof(Collider))]
public class menuTeleport : MonoBehaviour {
	private Vector3 startingPosition;
	public Fill Fill;
	float lerpTime = 5f;
	public float currentLerpTime;
	Color startColor = Color.green;
	Color endColor = Color.blue;
	public static bool gazedAt1 = false;
	public static bool aniTrigger = false;
	public float elapsedTime = 0f;



	void Start() {
		startingPosition = transform.localPosition;
		Debug.Log("Starting");
		SetGazedAt(false);

	}
	


	

	public void SetGazedAt(bool gazedAt) { 
		if (gazedAt == true) {
			aniTrigger = true;
			Vector3 source = new Vector3 (1,-2,0);
			Vector3 ending = new Vector3 (0,0,0);
			startColor = Color.clear;
			endColor = new Color(0.0f,0.0f,1.0f,0.8f);
			StartCoroutine(FillObject (startColor,endColor, 3 ));
			//GetComponent<Renderer>().material.color = Color.blue;
		} else {
			GetComponent<Renderer>().material.color = Color.clear;
			ScaleDown();
		}
		
		
	}
	
	public void Reset() {
		transform.localPosition = startingPosition;
	}

	public void ResetGaze() {
		GetComponent<Renderer>().material.color = Color.clear;
	}
	
	public void ToggleVRMode() {
		Cardboard.SDK.VRModeEnabled = !Cardboard.SDK.VRModeEnabled;
	}

	IEnumerator MoveObject(Vector3 source, Vector3 target, float overTime)
	{
		float startTime = Time.time;
		while(Time.time < startTime + overTime)
		{
			transform.position = Vector3.Lerp(source, target, (Time.time - startTime)/overTime);
			//GetComponent<Renderer>().material.color = Color.Lerp(startColor, endColor, (Time.time - startTime)/overTime);
			yield return null;
		}
		transform.position = target;
	}

	IEnumerator ScaleObject() {
		GameObject obj = this.gameObject;  // Link to the gameobject you want to scale
		Vector3 amount = new Vector3 (6,6,6); // Place desired scale amount here
		float time = 3;   // Put your time here
		iTween.ScaleBy(obj, amount, time);
		ScaleDown ();
		yield return null;

	}
	IEnumerator ScaleDown(){
		GameObject obj = this.gameObject;  // Link to the gameobject you want to scale
		Vector3 amount = new Vector3 (-5,-5,-5); // Place desired scale amount here
		float time = 1;   // Put your time here
		iTween.ScaleBy(obj, amount, time);
		yield return null;
	}
	


	IEnumerator FillObject(Color startColor, Color endColor, float overTime)
	{
		//Vector3 source = new Vector3 (0,0,0);
		//Vector3 ending = new Vector3 (0,-10,-10);
		//StartCoroutine(ScaleObject ());

		float startTime = Time.time;
		while(Time.time < startTime + overTime)
		{
			//transform.position = Vector3.Lerp(source, target, (Time.time - startTime)/overTime);
			GetComponent<Renderer>().material.color = Color.Lerp(startColor, endColor, (Time.time - startTime)/overTime);
			//StartCoroutine(ScaleObject ());
			yield return null;
		}
		GetComponent<Renderer> ().material.color = endColor;
		StartCoroutine(ScaleObject ());
		yield return new WaitForSeconds(2);
		var x = this.gameObject.name;
		TeleportNow (x);

	}

	
	
	public void TeleportRandomly() {
		Vector3 direction = Random.onUnitSphere;
		direction.y = Mathf.Clamp(direction.y, 0.5f, 1f);
		float distance = 2 * Random.value + 1.5f;
		transform.localPosition = direction * distance;
	}
	public void TeleportDemo() {
		Application.LoadLevel ("DemoScene");
		
	}
	public void TeleportNow(string level) {
		Application.LoadLevel (level);
		
	}




	
	
	
	
	
}
