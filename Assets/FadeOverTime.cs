using UnityEngine;
using System.Collections;
using System;

public class FadeOverTime : MonoBehaviour {

	public bool fadeOut = false;
	public float fadeOutMin = 0.2f;
	public int beforeFadeOut = 1;
	private VisualizerManager mVisManager;
	private Visualization[] mVisualizations;
	private Visualization mCurrentVis;

	private Transform firstPersonTransform;
	private float timeSincePathStart;
	public float picsPerSecond = 1;

	enum States {NONE, FADING};

	private States mState = States.NONE; 

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		if (mState == States.NONE)
		{
			/* Press F to start fading */
			if (Input.GetKeyDown(KeyCode.F))
			{
				mVisManager = gameObject.GetComponent<VisualizerManager>();
				mVisualizations = mVisManager.visualizations;
				mCurrentVis = mVisualizations[mVisManager.currentlySelectedVis];
				timeSincePathStart = 0;
				//firstPersonTransform.position = mCurrentVis.targetMetadataParser.output[0].transform.position;
				mState = States.FADING;
			}	
		}
		else if (mState == States.FADING)
		{
			/* Press F again to stop fading */
			if (Input.GetKeyDown(KeyCode.F))
			{
				mState = States.NONE;
			}	

			int prevIndex;
			int nextIndex;
			timeSincePathStart += Time.deltaTime;

			prevIndex = (int)Mathf.Floor(timeSincePathStart * picsPerSecond);
			nextIndex = (int)Mathf.Ceil(timeSincePathStart * picsPerSecond);

			mCurrentVis.targetMetadataParser.output[prevIndex].material.color = new Color(1.0f, 1.0f, 1.0f, (timeSincePathStart * picsPerSecond) - prevIndex);
			try {
				if (fadeOut)
				{
					float alpha = 1 - ((timeSincePathStart * picsPerSecond) - prevIndex);
					if (alpha < fadeOutMin)
						alpha = fadeOutMin;

					mCurrentVis.targetMetadataParser.output[prevIndex - beforeFadeOut].material.color = new Color(1.0f, 1.0f, 1.0f, alpha);
					mCurrentVis.targetMetadataParser.output[prevIndex - beforeFadeOut - 1].material.color = new Color(1.0f, 1.0f, 1.0f, fadeOutMin);
				}
			} 
			catch (Exception e) 
			{
				//Debug.Log ("exception" + e);
			}

		
		}
	}
}
