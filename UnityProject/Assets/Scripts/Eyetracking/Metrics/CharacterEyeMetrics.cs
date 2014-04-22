﻿using UnityEngine;
using System.Collections;

public class CharacterEyeMetrics : MonoBehaviour
{

	[SerializeField]
	private GazeThesisTest
		gazeHandler;
	[SerializeField]
	private float
		hitRayMaxDistance = 100f;
	[SerializeField]
	private float
		minPupilDilation = 15f;
	[SerializeField]
	private Color
		rayColor;
	[SerializeField]
	private Transform target;

	private Camera characterCamera;
	private Vector3 gazePosWorld = Vector3.zero;
	private Vector3 gazePosScreen = Vector3.zero;
	private Transform currentTarget;
	private Ray gazeRay;
	private bool isHit;
	private Vector3 poi;
	private float pupilSize;

	void Start()
	{
		characterCamera = GetComponentInChildren<Camera>();
//		StartCoroutine(CollectEyetrackerMetrics());
//		StartCoroutine(CollectMetrics());
		StartCoroutine(GazeRay());
		StartCoroutine(EyeData());
//		StartCoroutine(DebugGazeRay());


	}

	/*
	 *	Debugging
	 */
	void OnDrawGizmos()
	{
		if(isHit)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(poi, 0.5f);
		}
		Gizmos.color = Color.cyan;
		Gizmos.DrawRay(gazeRay.origin, gazeRay.direction * hitRayMaxDistance);
	}

	private IEnumerator DebugGazeRay()
	{
		while(true)
		{
			gazePosWorld = transform.position;
//			Vector3 gazePosOrigin = characterCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
			Vector3 gazePosOrigin = characterCamera.transform.position;
			gazePosWorld = characterCamera.ScreenToWorldPoint(new Vector3(Mathf.Clamp((int)Input.mousePosition.x, 0, Screen.width), Mathf.Clamp((int)Input.mousePosition.y, 0, Screen.height), characterCamera.farClipPlane));

			//Ray showing gaze direction
			RaycastHit hit;
			gazeRay = new Ray(gazePosOrigin, gazePosWorld);
//			Debug.DrawRay(gazeRay.origin, gazeRay.direction * hitRayMaxDistance, rayColor);
			if(Physics.Raycast(gazeRay, out hit, hitRayMaxDistance))
			{
				currentTarget = hit.transform;
				Debug.Log("hit something: " + currentTarget.name, currentTarget.gameObject);
				poi = hit.point;
				isHit = true;

			}
			else
			{
				currentTarget = null;
				poi = gazeRay.direction * hitRayMaxDistance;
				isHit = false;
//				Debug.Log("staring into infinity and beyond");
			}
			
			yield return null;
		}
	}

	private IEnumerator GazeRay()
	{
		while(true)
		{
			gazePosWorld = transform.position;
			Vector3 eyescreenpos = gazeHandler.GetGazeScreenPosition();
//			gazePosScreen = new Vector3(Mathf.Clamp((int)eyescreenpos.x, 0, Screen.width), Mathf.Clamp((int)eyescreenpos.y, 0, Screen.height), characterCamera.farClipPlane);
			gazePosScreen = eyescreenpos;
			Vector3 gazePosOrigin = characterCamera.transform.position;
			gazePosWorld = characterCamera.ScreenToWorldPoint(new Vector3(gazePosScreen.x, gazePosScreen.y, characterCamera.farClipPlane));
			//Ray showing gaze direction
			RaycastHit hit;
			gazeRay = new Ray(gazePosOrigin, gazePosWorld);
			Debug.DrawRay(gazeRay.origin, gazeRay.direction * hitRayMaxDistance, rayColor);
			if(Physics.Raycast(gazeRay, out hit, hitRayMaxDistance))
			{
				currentTarget = hit.transform;
				target.transform.position = hit.point;
				Debug.Log("hit something: " + currentTarget.name, currentTarget.gameObject);
				poi = hit.point;
				isHit = true;
			}
			else
			{
				currentTarget = null;
				poi = gazeRay.direction * hitRayMaxDistance;
				isHit = false;
//				Debug.Log("staring into infinity and beyond");
			}

			yield return null;
		}
	}

	private IEnumerator EyeData()
	{
		while(true)
		{
			pupilSize = gazeHandler.GetMeanPupilDilation();

			try
			{
//				Debug.Log("TimeSinceLastBlink: ");
//				Debug.Log(gazeHandler.GetTimeSinceLastBlink());
			}
			catch(System.Exception e)
			{
				Debug.Log (e, gameObject);
			}
			yield return null;
		}
	}

	private IEnumerator CollectMetrics()
	{
		while(true)
		{
			GA.API.Design.NewEvent("RandomTestPosition", Random.Range(20, 24), transform.position);
			
			yield return new WaitForSeconds(1f);
		}
	}

	private IEnumerator CollectEyetrackerMetrics()
	{
//		Vector3 screen_coord = Input.mousePosition;
//		Vector3 world_coord = Camera.main.ScreenToWorldPoint(new Vector3(screen_coord.x, screen_coord.y, transform.position.z - Camera.main.transform.position.z));
//		gazeHandler.transform.position = world_coord
//		float pos_x = world_coord.x;
//		pos_x =  Mathf.Clamp(pos_x, -MaxVelocityChange, MaxVelocityChange);
//		transform.position = new Vector3(pos_x, transform.position.y, transform.position.z);
		while(gazeHandler)
		{
			float pupilDilation = gazeHandler.GetMeanPupilDilation();
			if(pupilDilation > minPupilDilation)
			{
				GA.API.Design.NewEvent("PupilDilationMean", pupilDilation, transform.position);
				Debug.Log("Pupil Dilation Current: " + pupilDilation);
			}

			yield return new WaitForSeconds(1f);

			GA.API.Design.NewEvent("GazeCoordinates", gazePosWorld);
			
			yield return new WaitForSeconds(1f);
		}
	}

	public Ray GetCurrentGazeRay()
	{
		return this.gazeRay;
	}

	public string GetCurrentTargetName()
	{
		if(!currentTarget)
		{
			return "";
		}
		return this.currentTarget.gameObject.name;
	}

	public Vector3 GetCurrentHitPosition()
	{
		return this.poi;
	}

	public float PupilSize {
		get {
			return pupilSize;
		}
	}
}
