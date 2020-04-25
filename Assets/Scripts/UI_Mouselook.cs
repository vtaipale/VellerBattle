using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;

public class UI_Mouselook : MonoBehaviour {

	public FreeLookCam MyCamera;

	public GameObject Pivot;

	public bool MouseLookOn = false;

	public int ZoomLevelCurrent = 3;


	// Use this for initialization
	void Start () {
		MyCamera = FindObjectOfType<FreeLookCam> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton (1)) {
			MouseLookOn = true;
		} else
			MouseLookOn = false;

		int ZoomDelta;

		if (Input.GetMouseButtonDown (2)) {
			ZoomDelta = 3;
		}
		else 
			ZoomDelta = ZoomLevelCurrent + Mathf.RoundToInt(Input.mouseScrollDelta.y);
		

		if (MouseLookOn)
			MyCamera.enabled = true;
		else
			MyCamera.enabled = false;


		if (ZoomDelta != ZoomLevelCurrent) {
			SetZoomLevel (ZoomDelta);
		}

	}

	public void SetZoomLevel (int LevelToBe)
	{
		if (LevelToBe <= 0)
			Zoom (ZoomLevels [0]);
		else if (LevelToBe >= ZoomLevels.Length)
			Zoom (ZoomLevels [ZoomLevels.Length - 1]);
		else	
			Zoom(ZoomLevels[LevelToBe]);

		ZoomLevelCurrent = Mathf.Max (1,LevelToBe);
		ZoomLevelCurrent = Mathf.Min (ZoomLevelCurrent,ZoomLevels.Length-1);
	}


	public float[] ZoomLevels = new float[] {
				0.1f,
				0.5f,
				1f,
				2f,
				3f,
				5f,
				10f,
				100f,
				1000f,
				3000f
			};


	private void Zoom(float ZoomLVL)
	{
		Pivot.transform.localScale = new Vector3(ZoomLVL,ZoomLVL,ZoomLVL);

		if (ZoomLVL < 0.5f) {
			Pivot.transform.localPosition = new Vector3 (0f, 0.1f, 0f);
		}	
		else if (ZoomLVL == 0.5f) {
			Pivot.transform.localPosition = new Vector3 (0f, 0.5f, 0f);
		}
		else {
			Pivot.transform.localPosition = new Vector3 (0f, 1f, 0f);
		}
	}

}
