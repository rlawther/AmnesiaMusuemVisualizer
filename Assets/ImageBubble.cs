using UnityEngine;
using System.Collections;

public class ImageBubble : MonoBehaviour {

	public int offset = 1;
	public int divider = 10;

	private VisualizerManager visManager;
	// Use this for initialization
	void Start () {
		this.visManager = gameObject.GetComponent<VisualizerManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		foreach (var currentVis in this.visManager.visualizations) {
			if(currentVis.targetMetadataParser.output == null)
				continue;

			foreach (var imageItem in currentVis.targetMetadataParser.output) {
				//If the item doesn't have a transform, go to the next item. 
				if (imageItem.transform == null)
					continue;

				var distance = Vector3.Distance (imageItem.transform.position, gameObject.transform.position);
				var opacity = distance / divider - offset;
				imageItem.material.color = new Color (1.0f, 1.0f, 1.0f, opacity);
			}
		}
	}
}
