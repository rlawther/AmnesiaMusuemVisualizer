using UnityEngine;
using System.Collections;
using ReadWriteCsv;
//using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class VisualizerManager : MonoBehaviour {
	
	//private VisOptions visOps;
	[System.Serializable]
	public class Dataset {
		public string csvMetadataFile;
		public string imageDirectory;
		public string imageExtension;
	}

	public bool tvisLayout = false;

	public Dataset[] datasets;
	private string rootDir;
	//public string[] rootFolders;

	[HideInInspector]
	public Visualization[] visualizations;
	
	//public Mesh DoubleSidedMesh;
	public GameObject quadTemplate;
	//public LockToPath pather;
	public int currentlySelectedVis = 0;
	
	private bool showChartGUI = false;
	private GameObject currentChart = null;
	
	public Material lineMaterial;

	private bool pathwaysSynced = false;

	protected void Start() {
		//this.visOps = this.GetComponent<VisOptions>();
		this.visualizations = new Visualization[datasets.Length];
		for (int i = 0; i < datasets.Length; i++) {
			var dataset = datasets[i];
			dataset.imageDirectory = Path.GetFullPath(dataset.imageDirectory);
			dataset.csvMetadataFile = Path.GetFullPath(dataset.csvMetadataFile);

			Visualization v = this.createVisualization(datasets[i]);
			visualizations[i] = v;
		}
		//visOps.SetVisualizations(this.visualizations);
		if (this.visualizations.Length > 0) {
			this.setCurrentVisualization(0);
		}
	}
	
	protected Visualization createVisualization(Dataset dataset) { 
		GameObject go = new GameObject();
		//string folderName = new DirectoryInfo(rootFolder).Name;
		Visualization v = go.AddComponent<Visualization>();		
		go.name = dataset.csvMetadataFile;				
		AutographerParser parser = go.AddComponent<AutographerParser>();
		//parser.allowInterp = this.GetComponent<AutographerParser>().allowInterp;
		//parser.imageResolution = this.GetComponent<AutographerParser>().imageResolution;		
		
		v.projectName = dataset.csvMetadataFile;
		v.csvMetadataFile = dataset.csvMetadataFile;
		v.imageDirectory = dataset.imageDirectory;
		v.imageExtension = dataset.imageExtension;
		//v.tb = this;
		v.targetMetadataParser = v.GetComponent<MetadataParser>();
		//v.pather = this.pather;
		//v.DoubleSidedMesh = this.DoubleSidedMesh;
		v.quadTemplate = this.quadTemplate;
		v.tvisLayout = this.tvisLayout;
		//v.transform.parent = this.sceneParent;
		return v;
	}

	protected void Update() {
		this.handleInput();
		/*
		this.pathwaysSynced = true;
		if (!this.pathwaysSynced) {
			this.syncGPSPathways();
		}
		*/
			
	}

	protected void syncGPSPathways() {
		for (int i = 0; i < datasets.Length; i++) {
			if (!this.visualizations[i].QuadsHaveBeenCreated()) return;
		}

		for (int i = 1; i < datasets.Length; i++) {
			Visualization v = this.visualizations[i];
			Vector3 initialGPS = this.visualizations[0].calculateParentGPS();
			Vector3 result = v.calculateGroupOffset(initialGPS);
			v.transform.localPosition = result;
		}
		this.pathwaysSynced = true;
	}
	public void setCurrentVisualization(int num) {
		this.currentlySelectedVis = num;
		//this.pather.registerPath(this.visualizations[currentlySelectedVis].quadList);
	}
	protected void handleInput() {
		if (Input.GetKeyDown (KeyCode.Keypad0)) {
			this.setCurrentVisualization(0);
		}		
		if (Input.GetKeyDown (KeyCode.Keypad1)) {
			this.setCurrentVisualization(1);
		}
		if (Input.GetKeyDown (KeyCode.Keypad2)) {
			this.setCurrentVisualization(2);
		}
		if (Input.GetKeyDown (KeyCode.Keypad3)) {
			this.setCurrentVisualization(3);
		}
		if (Input.GetKeyDown (KeyCode.Keypad4)) {
			this.setCurrentVisualization(4);
		}
		if (Input.GetKeyDown (KeyCode.Keypad5)) {
			this.setCurrentVisualization(5);
		}
		if (Input.GetKeyDown (KeyCode.Keypad6)) {
			this.setCurrentVisualization(6);
		}
		if (Input.GetKeyDown (KeyCode.Keypad7)) {
			this.setCurrentVisualization(7);
		}
		if (Input.GetKeyDown (KeyCode.Keypad8)) {
			this.setCurrentVisualization(8);
		}
		if (Input.GetKeyDown (KeyCode.Keypad9)) {
			this.setCurrentVisualization(9);
		}
		if (Input.GetKeyDown (KeyCode.G)) {
			this.showChartGUI = !this.showChartGUI;
		}
	}
	void OnGUI() {
		if (this.showChartGUI) {
			List<float> result = this.visualizations[this.currentlySelectedVis].targetMetadataParser.showGraphOptions();
			if (result != null) {
				this.generateChart(result);
			}
		}
	}
	void generateChart(List<float> items) {
		if (this.currentChart != null) {
			Destroy (this.currentChart);
			this.currentChart = null;
		}
		this.currentChart = new GameObject("Statistics");
		this.currentChart.AddComponent<LineRenderer>();
		LineRenderer lr = this.currentChart.GetComponent<LineRenderer>();
		lr.material = this.lineMaterial;
		lr.SetVertexCount(items.Count);
		
		float min = items[0];
		float max = items[0];
		for (int i = 1; i < items.Count; i++) {
			if (items[i] < min) {
				min = items[i];
			} 
			if (items[i] > max) {
				max = items[i];
			}
		}
		float range = max - min;
		for (int i = 0; i < items.Count; i++) {
			float a = items[i];
			lr.SetPosition(i, new Vector3((float)i/items.Count * 100f,(a - min)/range * 100,0));
		}
		lr.SetColors(Color.blue,Color.blue);		
			
	}
	
	
}