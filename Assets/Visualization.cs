using UnityEngine;
using System.Collections;

public class Visualization : MonoBehaviour {
	private bool createdQuads = false;
	public Vector3 visualizationScale = new Vector3(1.0f,1.0f,1.0f);
	private Vector3 _visualizationScale = new Vector3(1.0f,1.0f,1.0f);
	
	public Vector3 rotateAll = new Vector3(0f,0f,0f);
	private Vector3 _rotateAll = new Vector3(0f,0f,0f);
	public Vector3 rotateMult = new Vector3(1f,1f,1f);
	public bool canUpdateLive = false;
	
	public float quadScale = 1f;
	private float _quadScale = 1f;
	
	public enum RotationMethod
	{
		Euler,
		LookRotation,
	};
	
	private RotationMethod _rotationMethod = RotationMethod.Euler;
	public RotationMethod rotationMethod = RotationMethod.Euler;
	//public LockToPath pather;
	
	public string projectName;
	//public ToolbeltManager tb;
	public MetadataParser targetMetadataParser;
	
	public string rootDir;
	public Mesh DoubleSidedMesh;
	public Transform[] quadList;
	// Use this for initialization
	private const double m_per_deg_lat = 111132.954f;
	private const double m_per_deg_lon = 111132.954f;
	void Start () {
	
	}
	public bool QuadsHaveBeenCreated() {
		return this.createdQuads;
	}
	// Update is called once per frame
	void Update () {
		if (this.targetMetadataParser.ProjectName == null || this.targetMetadataParser.ProjectName == "") {
			this.targetMetadataParser.ProjectName = this.projectName;			
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

	/*
	IEnumerator loadImageUrl(WWW www)
	{
		yield return www;
		
		if (www.error != null)
		{
			Debug.LogError("WWW Error: "+ www.error);
		}    
	} 
	*/
	
	protected void createQuads() {
		//Shader transDiff = Shader.Find ("Transparent/Diffuse");
		this.quadList = new Transform[this.targetMetadataParser.output.Count];
		int i = 0;
		foreach (MetaDataItem mdi in this.targetMetadataParser.output) {
			// FIXME : just creating a cube for now. Needs to be a quad with an image
			GameObject q = GameObject.CreatePrimitive(PrimitiveType.Cube);
			
			q.transform.parent = this.transform;

			// make them big enough to see easily
			q.transform.localScale = new Vector3(50, 50, 50);

			/* Ignore texturing for now */
			/*
			if (i < 40)
			{
				//Texture tex = (Texture)Resources.LoadAssetAtPath(rootDir + "\\" + mdi.filename, typeof(Texture));
				WWW www = new WWW("file:///D:/Amnesia/2013_12_19/09/B00000000_21I4N2_20131219_091515E.JPG");
				Texture2D texTmp = new Texture2D(128, 128, TextureFormat.DXT1, false);
				//LoadImageIntoTexture compresses JPGs by DXT1 and PNGs by DXT5     
				www.LoadImageIntoTexture(texTmp);
				///loadImageUrl(www);
				//Texture tex = (Texture)Resources.LoadAssetAtPath("D:/Amnesia/2013_12_19/09/B00000000_21I4N2_20131219_091515E.JPG", typeof(Texture));
				q.renderer.material.mainTexture = www.texture;
			}
			*/
			//q.GetComponent<MeshFilter>().mesh = this.DoubleSidedMesh;
			//q.renderer.material.shader = transDiff;
			/*
			icImageMaterial icm = q.AddComponent<icImageMaterial>();
			icm.filePath = "file:///" + rootDir + mdi.filename;			
			icm.icColour = Color.clear;
			FadeColour fc = q.AddComponent<FadeColour>();
			
			fc.duration = 0;
			fc.enabled = false;
			fc.removeWhenFinished = true;
			fc.targetColour = Color.white;
			
			icm.mediaReadyCallbacks.Add(fc);
			*/
			mdi.transform = q.transform;
			quadList[i] = q.transform;
			i += 1;

		}
		this.canUpdateLive = true;
		this.calculateQuadPositions();
		
	}
	
	void calculateQuadPositions() {
		//http://stackoverflow.com/a/19356480/1342750
//		float latMid = (targetMetadataParser.min.latitude + targetMetadataParser.max.latitude)/2.0f;
//		float m_per_deg_lat = (float)(111132.954 - 559.822 * Mathf.Cos( 2 * latMid ) + 1.175 * Mathf.Cos( 4 * latMid));
//		float m_per_deg_lon = (float)(111132.954 * Mathf.Cos ( latMid ));

		
		
		
		foreach (MetaDataItem mdi in this.targetMetadataParser.output) {
			Transform q = mdi.transform;
			
			Vector3 pos = q.localPosition;
			pos.x = (float)((mdi.latitude - targetMetadataParser.min.latitude) * m_per_deg_lat * this.visualizationScale.x);
			pos.y = (float)((mdi.altitude - targetMetadataParser.min.altitude) * this.visualizationScale.y);
			pos.z = (float)((mdi.longitude - targetMetadataParser.min.longitude) * m_per_deg_lon * this.visualizationScale.z);
			
			q.localPosition = pos;
			
			if (rotationMethod == RotationMethod.Euler) {
				q.localRotation = Quaternion.Euler(mdi.xOrientation  * rotateMult.x + rotateAll.x,
												mdi.yOrientation * rotateMult.y + rotateAll.y,
												mdi.zOrientation * rotateMult.z + rotateAll.z);	
			} else {
				q.localRotation = Quaternion.LookRotation(new Vector3(mdi.xOrientation * rotateMult.x + rotateAll.x,
														mdi.yOrientation * rotateMult.y + rotateAll.y,
														mdi.zOrientation * rotateMult.z + rotateAll.z));
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
