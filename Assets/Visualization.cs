using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ReadWriteCsv;
using System;

public class Visualization : MonoBehaviour {
	private bool createdQuads = false;
	public Vector3 visualizationScale = new Vector3(1.0f,1.0f,1.0f);
	private Vector3 _visualizationScale = new Vector3(1.0f,1.0f,1.0f);
	
	public Vector3 rotateAll = new Vector3(90f,0f,0f);
	private Vector3 _rotateAll = new Vector3(0f,0f,0f);
	public Vector3 rotateMult = new Vector3(1f,1f,1f);
	public bool canUpdateLive = false;
	
	public float quadScale = 1f;
	private float _quadScale = 1f;

	private float latitudeOrigin = -33.980f;
	private float longitudeOrigin = 151.230f;
	private float altitudeOrigin = 30.0f;

	
	public enum RotationMethod
	{
		Euler,
		LookRotation,
	};
	
	private RotationMethod _rotationMethod = RotationMethod.Euler;
	public RotationMethod rotationMethod = RotationMethod.Euler;
	//public LockToPath pather;
	
	public string projectName;
	public string csvMetadataFile;
	public string imageDirectory;
	public string imageExtension;
	//public ToolbeltManager tb;
	public MetadataParser targetMetadataParser;
	
	public string rootDir;
	public Mesh DoubleSidedMesh;
	public GameObject quadTemplate;
	public Transform[] quadList;
	// Use this for initialization
	private const double m_per_deg_lat = 111132.954f;
	private const double m_per_deg_lon = 111132.954f;
	public bool tvisLayout = false;

	void Start () {
	
	}
	public bool QuadsHaveBeenCreated() {
		return this.createdQuads;
	}
	// Update is called once per frame
	void Update () {
		if (this.targetMetadataParser.ProjectName == null || this.targetMetadataParser.ProjectName == "") {
			this.targetMetadataParser.ProjectName = this.projectName;			
			this.targetMetadataParser.csvMetadataFile = this.csvMetadataFile;			
		}
		
		if (this.targetMetadataParser.output != null && !this.createdQuads) {
			this.rootDir = this.projectName;			
			if (!(rootDir.EndsWith("/") || rootDir.EndsWith("//"))) {
				rootDir += "/";
			}
			this.createQuads();
			this.createdQuads = true;
		}
	
		if (this.visualizationScale != this._visualizationScale && this.canUpdateLive) {
			this._visualizationScale = this.visualizationScale;			
			this.calculateQuadPositions();
		}
		
		if (this.rotateAll != this._rotateAll && this.canUpdateLive) {
			this._rotateAll = this.rotateAll;			
			this.calculateQuadPositions();
		}
		
		if (this.quadScale != this._quadScale && this.canUpdateLive) {
			this._quadScale = this.quadScale;
			this.rescaleQuads();
		}
		
		if (this.rotationMethod != this._rotationMethod && this.canUpdateLive) {
			this._rotationMethod = this.rotationMethod;
			this.calculateQuadPositions();
		}
	}

	protected void createQuads() {
		removeLowPriorityItems();
		if (tvisLayout)
			createQuadsTVis();
		else
			createQuadsGPS();
	}

	protected void removeLowPriorityItems()
	{
		List<MetaDataItem> killList = new List<MetaDataItem> ();
		foreach (MetaDataItem mdi in this.targetMetadataParser.output) {
			if (mdi.priority <= 0) {
				killList.Add (mdi);
			}
		}
		foreach (MetaDataItem mdi in killList) {
			Debug.Log ("removing " + mdi.filename);
			this.targetMetadataParser.output.Remove (mdi);
		}
	}

	protected void createQuadsGPS() {
			//Shader transDiff = Shader.Find ("Transparent/Diffuse");
		this.quadList = new Transform[this.targetMetadataParser.output.Count];
		int i = 0;
		foreach (MetaDataItem mdi in this.targetMetadataParser.output) {
			/* Create a new quad for the image */
			GameObject q;
			q = (GameObject)Instantiate(quadTemplate);
			q.SetActive(true);
			StartCoroutine(WaitForTexture(q,mdi));
			mdi.material = q.renderer.material;
			/*
			if (mdi.priority <= 0)
			{
				q.renderer.enabled = false;
			}
			*/

			q.transform.parent = this.transform;
			// make them big enough to see easily
			q.transform.localScale = new Vector3(5.0f * 1.33f, 5.0f, 5.0f);
			/*
			 * This is just used so you can see the position of the quads more easily
			 * for debugging etc.
			 *
			q.transform.localScale = new Vector3(7.0f, 7.0f, 7.0f);
			q.renderer.sharedMaterial = quadTemplate.renderer.material;
			quadTemplate.renderer.material.color = Color.red;
			*/

			
			mdi.transform = q.transform;
			quadList[i] = q.transform;
			mdi.quad = q;

			i += 1;


		}
		this.canUpdateLive = true;
		this.calculateQuadPositions();
		
	}

	protected void createQuadsTVis() {
		this.quadList = new Transform[this.targetMetadataParser.output.Count];
		int i = 0;
		foreach (MetaDataItem mdi in this.targetMetadataParser.output) {
			/* Create a new quad for the image */
			GameObject q = (GameObject)Instantiate(quadTemplate);
			q.SetActive(true);
			
			q.transform.parent = this.transform;
			q.transform.localScale = new Vector3(1.33f, 1, 1);
			
			StartCoroutine(WaitForTexture(q,mdi));
			
			mdi.transform = q.transform;
			mdi.material = q.renderer.material;
			mdi.quad = q;
			quadList[i] = q.transform;
			
			i += 1;
			
		}
		this.canUpdateLive = true;
		this.calculateQuadPositions();
	}

	private IEnumerator WaitForTexture (GameObject q,MetaDataItem mdi)
	{
		WWW www = new WWW ("file:///" + this.imageDirectory + "/" + mdi.filename);
		yield return www;
		q.renderer.material.mainTexture = www.texture;
	}
	void calculateQuadPositions() {
		if (tvisLayout)
			calculateQuadPositionsTVis();
		else
			calculateQuadPositionsGPS();
	}

	void calculateQuadPositionsGPS() {
		//http://stackoverflow.com/a/19356480/1342750
//		float latMid = (targetMetadataParser.min.latitude + targetMetadataParser.max.latitude)/2.0f;
//		float m_per_deg_lat = (float)(111132.954 - 559.822 * Mathf.Cos( 2 * latMid ) + 1.175 * Mathf.Cos( 4 * latMid));
//		float m_per_deg_lon = (float)(111132.954 * Mathf.Cos ( latMid ));

		
		
		
		foreach (MetaDataItem mdi in this.targetMetadataParser.output) {
			try{

			Transform q = mdi.transform;
			
			Vector3 pos = q.localPosition;
			pos.x = (float)((mdi.latitude - latitudeOrigin) * m_per_deg_lat);
			pos.y = (float)((mdi.altitude - altitudeOrigin));
			pos.z = (float)((mdi.longitude - longitudeOrigin) * m_per_deg_lon);
			pos.x *= -1;

			/* allow us to alter the ypos by setting it on the template quad */
			pos.y += quadTemplate.transform.position.y;

			q.localPosition = pos;
			q.localRotation = Quaternion.Euler(0, mdi.heading + 90, 0);
			/*
			if (rotationMethod == RotationMethod.Euler) {
				q.localRotation = Quaternion.Euler(mdi.xOrientation  * rotateMult.x + rotateAll.x,
												mdi.yOrientation * rotateMult.y + rotateAll.y,
												mdi.zOrientation * rotateMult.z + rotateAll.z);	
			} else {
				q.localRotation = Quaternion.LookRotation(new Vector3(mdi.xOrientation * rotateMult.x + rotateAll.x,
														mdi.yOrientation * rotateMult.y + rotateAll.y,
														mdi.zOrientation * rotateMult.z + rotateAll.z));
			}
			*/
			} catch (NullReferenceException e)
			{
			}
			
		}
	}

	void calculateQuadPositionsTVis() {
		CsvRow row = new CsvRow();
		int i = 0;

		using (CsvFileReader reader = new CsvFileReader("Assets/TVis.layout")) 
		{
			foreach (MetaDataItem mdi in this.targetMetadataParser.output) {
				float ang, dist;
				
				reader.ReadRow(row);
				//Debug.Log (row[0] + ", " + row[1] + ", " + row[2]);
				Transform q = mdi.transform;
				Vector3 pos = q.localPosition;
				ang = float.Parse(row[0]) * Mathf.PI * 2;
				dist = float.Parse(row[2]);
				pos.x = Mathf.Cos (ang) * dist;
				pos.y = float.Parse(row[1]);
				pos.z = Mathf.Sin (ang) * dist;
				
				q.localPosition = pos;
				q.LookAt(q.localPosition * 2);
				i++;
				
				/* Only displays the first 350 quads */
				if (i >= 350)
					break;				
			}
		}

	}

	public Vector3 calculateParentGPS() {
		Vector3 pos = new Vector3();
		pos.x = this.targetMetadataParser.min.latitude;
		pos.y = this.targetMetadataParser.min.altitude;
		pos.z = this.targetMetadataParser.min.longitude;
		return pos;
	}

	// This function assumes that their visualizationScales are the same
	public Vector3 calculateGroupOffset(Vector3 otherGPS) {
		Vector3 thisGps = this.calculateParentGPS();
		Vector3 difference = otherGPS-thisGps;
		Vector3 result = new Vector3();

		Debug.Log (otherGPS);
		Debug.Log (thisGps);
		Debug.Log (difference);
		result.x = (float)(difference.x * m_per_deg_lat * this.visualizationScale.x);
		result.y = (float)(difference.y * this.visualizationScale.y);
		result.z = (float)(difference.z * m_per_deg_lon * this.visualizationScale.z);

		return result;
	}

	void rescaleQuads() {
		foreach (MetaDataItem mdi in this.targetMetadataParser.output) {
			// FIXME : need to reimplement scaling
			/*
			Transform q = mdi.transform;
			icImageMaterial icm = q.GetComponent<icImageMaterial>();
			icm.SetBaseScale(this.quadScale);
			*/
		}
	}
	
	
}
