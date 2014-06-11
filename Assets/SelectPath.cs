using UnityEngine;
using System.Collections;
using System;

public class SelectPath : MonoBehaviour {

	private VisualizerManager mVisManager;
	private Visualization[] mVisualizations;
	private Visualization mCurrentVis;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		bool changeVis = false;
		int changeTo = -1;

		if (Input.GetKeyDown(KeyCode.Alpha0)) {
			changeVis = true;
			changeTo = 0;
		} else if (Input.GetKeyDown(KeyCode.Alpha1)) {
			changeVis = true;
			changeTo = 1;
		} else if (Input.GetKeyDown(KeyCode.Alpha2)) {
			changeVis = true;
			changeTo = 2;
		} else if (Input.GetKeyDown(KeyCode.Alpha3)) {
			changeVis = true;
			changeTo = 3;
		} else if (Input.GetKeyDown(KeyCode.Alpha4)) {
			changeVis = true;
			changeTo = 4;
		} else if (Input.GetKeyDown(KeyCode.Alpha5)) {
			changeVis = true;
			changeTo = 5;
		} else if (Input.GetKeyDown(KeyCode.Alpha6)) {
			changeVis = true;
			changeTo = 6;
		} 

		if (changeVis)
		{
			mVisManager = gameObject.GetComponent<VisualizerManager>();
			mVisualizations = mVisManager.visualizations;
			mCurrentVis = mVisualizations[mVisManager.currentlySelectedVis];

			if (changeTo == 0)
			{
				/* All paths visible */
				foreach (Visualization vis in mVisualizations) {
					foreach (MetaDataItem mdi in vis.targetMetadataParser.output) {
						mdi.quad.renderer.enabled = true;
					}
				}
			} else {
				/* All paths invisible */
				foreach (Visualization vis in mVisualizations) {
					foreach (MetaDataItem mdi in vis.targetMetadataParser.output) {
						mdi.quad.renderer.enabled = false;
					}
				}
				/* make selected path visible */
				try {
					foreach (MetaDataItem mdi in mVisualizations[changeTo - 1].targetMetadataParser.output) {
						mdi.quad.renderer.enabled = true;
					}
				} catch (Exception e) {
					Debug.Log ("Could not make path " + changeTo + " visible.");
				}
				mVisManager.setCurrentVisualization(changeTo - 1);
			}
		}	
	}
}
