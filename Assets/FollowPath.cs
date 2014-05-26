using UnityEngine;
using System.Collections;

public class FollowPath : MonoBehaviour {

	private VisualizerManager mVisManager;
	private Visualization[] mVisualizations;
	private Visualization mCurrentVis;

	private Transform firstPersonTransform;
	private float timeSincePathStart;
	public float picsPerSecond = 1;

	enum States {NONE, FOLLOWING};

	private States mState = States.NONE; 

	// Use this for initialization
	void Start () {
		firstPersonTransform = gameObject.GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {

		if (mState == States.NONE)
		{
			/* Press SPACE to start path following */
			if (Input.GetKeyDown(KeyCode.Space))
			{
				mVisManager = gameObject.GetComponent<VisualizerManager>();
				mVisualizations = mVisManager.visualizations;
				mCurrentVis = mVisualizations[mVisManager.currentlySelectedVis];
				timeSincePathStart = 0;
				firstPersonTransform.position = mCurrentVis.targetMetadataParser.output[0].transform.position;
				mState = States.FOLLOWING;
			}	
		}
		else if (mState == States.FOLLOWING)
		{
			/* Press SPACE again to stop following */
			if (Input.GetKeyDown(KeyCode.Space))
			{
				mState = States.NONE;
			}	

			int prevIndex;
			int nextIndex;
			timeSincePathStart += Time.deltaTime;

			prevIndex = (int)Mathf.Floor(timeSincePathStart * picsPerSecond);
			nextIndex = (int)Mathf.Ceil(timeSincePathStart * picsPerSecond);

			if (prevIndex == nextIndex)
			{
				/* If we're right on a waypoint use that waypoint ...*/
				firstPersonTransform.position = mCurrentVis.targetMetadataParser.output[prevIndex].transform.position;
			}
			else
			{
				/* ... otherwise lerp between adjacent waypoints */
				Vector3 diff = mCurrentVis.targetMetadataParser.output[nextIndex].transform.position -
					mCurrentVis.targetMetadataParser.output[prevIndex].transform.position;
				firstPersonTransform.position = mCurrentVis.targetMetadataParser.output[prevIndex].transform.position + 
					(diff * ((timeSincePathStart * picsPerSecond) - prevIndex));
			}
		}
	}
}
