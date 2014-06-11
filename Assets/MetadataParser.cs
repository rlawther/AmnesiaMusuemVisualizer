using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public abstract class MetadataParser : MonoBehaviour {
	public string csvMetadataFile;

	private string projectName = "";
	public string ProjectName 
	{
		get {
			return this.projectName;
		}
		set {
			this.projectName = value;
		}
	} 
	private string projectRoot = "";
	
	private bool parsed = false;
	// Use this for initialization
	
	public List<MetaDataItem> output;
	public MetaDataItem min;
	public MetaDataItem max;
	
	abstract protected string encodingType {
		get;
	}
	
	void Start () {
		
	}
	public string ProjectRoot {
		get
		{
			return this.projectRoot;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!this.parsed && this.projectName != "") {
			this.parsed = true;
			/*
			string folder = Path.GetFullPath(this.projectName + "/");
			DirectoryInfo di = new DirectoryInfo(folder);
			FileInfo[] fi = di.GetFiles("*.txt");	
			if (fi.Length <= 0) {
				throw new FileNotFoundException("No .txt files in directory specified!");
			}
			*/
			//this.projectRoot = folder;			
			this.parseFile(this.csvMetadataFile);
		}
	}
	
	void parseFile(string fileToParse) {
		this.output = this.doParseFile(fileToParse);
		
	}
	
	//parse the file
	abstract protected List<MetaDataItem> doParseFile(string fileToParse);
	
	//return an array of floats representing a single data item.
	virtual public List<float> showGraphOptions(){ 
		return null;
	}
	
}

public class MetaDataItem {
	public string filename;
	public float latitude;
	public float longitude;
	public float altitude;
	
	public float xOrientation;
	public float yOrientation;
	public float zOrientation;

	public float heading;
	public int priority;
	
	public override string ToString ()
	{
		return filename + "\n" + "[" + latitude+","+longitude+","+altitude+"]" + "{"+xOrientation+","+yOrientation+","+zOrientation+"}";
	}
	public Transform transform;
	public Material material;
	public GameObject quad;
}
