using UnityEngine;
using System.Collections.Generic;
using ReadWriteCsv;
using System;

public class AutographerParser : MetadataParser {
	public class AutographerMetaDataItem : MetaDataItem {
		public string id;
		public string dt;
		public float AccuracyX;
		public float AccuracyY;
		public float AccuracyZ;
		public float MagnitudeX;
		public float MagnitudeY;
		public float MagnitudeZ;
		public float Red;
		public float Green;
		public float Blue;
		public float Luminance;
		public float Temperature;
		public float Gs;
		public float HorizontalError;
		public float VerticalError;
	}
	
	protected string lastFileParsed;
	public enum GraphField
	{
		dt = 0,
		id = 1,
		AccuracyX = 5,
		AccuracyY = 6,
		AccuracyZ = 7,
		MagnitudeX = 8,
		MagnitudeY = 9,
		MagnitudeZ = 10,
		Red = 11,
		Green = 12,
		Blue = 13,
		Luminance = 14,
		Temperature = 15,
		Latitude = 17,
		Longitude = 18,
		Altidude = 19,
		Gs = 20,
		HorizontalError = 21,
		VerticalError = 22,
		OrientationX = 28,
		OrientationY = 29,
		OrientationZ = 30		
	};
	
	protected override string encodingType {
		get {
			return "Autographer";
		}
	}
	public enum ImageResolution
	{
		Low,
		Medium,
		High
	};
	public ImageResolution imageResolution = ImageResolution.High;
	
	private string ImageResolutionPrefix(ImageResolution ir) {
		if (ir == ImageResolution.High) {
			return "";
		} else if (ir == ImageResolution.Medium) {
			return "640_480/";
		} else {
			return "256_192/";
		}
	}
	
	private string ImageResolutionSuffix(ImageResolution ir) {
		if (ir == ImageResolution.High) {
			return "e";
		} else if (ir == ImageResolution.Medium) {
			return "4";
		} else {
			return "2";
		}
	}
	
	private string HourFolder(string baseFileName) {
		return baseFileName.Substring(baseFileName.Length - 6, 2) + "/";
	}
	
	public bool allowInterp = true;
	sealed override protected List<MetaDataItem> doParseFile(string fileToParse) {
		this.lastFileParsed = fileToParse;
		List<MetaDataItem> result = new List<MetaDataItem>();
		//first open the file.
		using (CsvFileReader reader = new CsvFileReader(fileToParse)) 
		{
			CsvRow row = new CsvRow();
			while (reader.ReadRow(row))
			{
				AutographerMetaDataItem mdi = new AutographerMetaDataItem();
				if (float.TryParse(row[(int)GraphField.Latitude], out mdi.latitude)) {
					mdi.filename = HourFolder(row[(int)GraphField.id]) + this.ImageResolutionPrefix(this.imageResolution) 
								   + row[(int)GraphField.id] + this.ImageResolutionSuffix(this.imageResolution) + ".jpg";
					mdi.longitude = float.Parse(row[(int)GraphField.Longitude]);
					mdi.altitude = float.Parse(row[(int)GraphField.Altidude]);
					
					mdi.xOrientation = float.Parse (row[(int)GraphField.OrientationX]);
					mdi.yOrientation = float.Parse (row[(int)GraphField.OrientationY]);
					mdi.zOrientation = float.Parse (row[(int)GraphField.OrientationZ]);
					
					mdi.id = row[(int)GraphField.id];
					mdi.dt = row[(int)GraphField.dt];
					mdi.AccuracyX = float.Parse(row[(int)GraphField.AccuracyX]);
					mdi.AccuracyY = float.Parse(row[(int)GraphField.AccuracyY]);
					mdi.AccuracyZ = float.Parse(row[(int)GraphField.AccuracyZ]);
					mdi.MagnitudeX = float.Parse(row[(int)GraphField.MagnitudeX]);
					mdi.MagnitudeY = float.Parse(row[(int)GraphField.MagnitudeY]);
					mdi.MagnitudeZ = float.Parse(row[(int)GraphField.MagnitudeZ]);
					mdi.Red = float.Parse(row[(int)GraphField.Red]);
					mdi.Green = float.Parse(row[(int)GraphField.Green]);
					mdi.Blue = float.Parse(row[(int)GraphField.Blue]);
					mdi.Luminance = float.Parse(row[(int)GraphField.Luminance]);
					mdi.Temperature = float.Parse(row[(int)GraphField.Temperature]);
					mdi.Gs = float.Parse(row[(int)GraphField.Gs]);
					mdi.HorizontalError = float.Parse(row[(int)GraphField.HorizontalError]);
					mdi.VerticalError = float.Parse(row[(int)GraphField.VerticalError]);
					
					//dont add the first ones because they can't be lerped. They are USELESS!
					if (allowInterp)
					{
						if ((mdi.longitude == 0 && result.Count != 0) || mdi.longitude != 0)
						//if (mdi.longitude != 0) 
							result.Add(mdi);
					} else {
						if (mdi.longitude != 0) {
							result.Add (mdi);
						}
					}
				}
			}
		}
		
		int lastValueIndex = 0;		
		//now go through again, finding the last index with non-zero longitude
		//first item is guaranteed to be non-zero.
		//a "value" index is an index with a non zero value.
		this.min = new MetaDataItem();
		this.max = new MetaDataItem();
		
		for (int i = 0; i < result.Count; i++) {

			if (i == 0) {
				this.min.longitude = result[i].longitude;

				this.min.latitude = result[i].latitude;
				this.min.altitude = result[i].altitude;

				this.max.longitude = result[i].longitude;
				this.max.latitude = result[i].latitude;
				this.max.altitude = result[i].altitude;
			}
			if (result[i].longitude != 0.0) {
				//interpolate between previous value index and this value index.
				
				int startTerpIndex = lastValueIndex;
				int endTerpIndex = i;
				
				int numItems = i - (startTerpIndex + 1);
				
				for (int j = startTerpIndex + 1; j < i; j++ ) {
					
					int numerator = j - startTerpIndex; // starts from 1.
					int denominator = numItems + 1;
					float terpAmount = numerator/(float)denominator;
					result[j].latitude = Mathf.Lerp(result[startTerpIndex].latitude,result[endTerpIndex].latitude,terpAmount);
					result[j].longitude = Mathf.Lerp(result[startTerpIndex].longitude,result[endTerpIndex].longitude,terpAmount);
					result[j].altitude = Mathf.Lerp(result[startTerpIndex].altitude,result[endTerpIndex].altitude,terpAmount);					
				}
				lastValueIndex = i;
				
				this.min.latitude = result[i].latitude < this.min.latitude ? result[i].latitude : this.min.latitude;
				this.min.longitude = result[i].longitude < this.min.longitude ? result[i].longitude : this.min.longitude;
				this.min.altitude = result[i].altitude < this.min.altitude ? result[i].altitude : this.min.altitude;
				
				this.max.latitude = result[i].latitude > this.max.latitude ? result[i].latitude : this.max.latitude;
				this.max.longitude = result[i].longitude > this.max.longitude ? result[i].longitude : this.max.longitude;
				this.max.altitude = result[i].altitude > this.max.altitude ? result[i].altitude : this.max.altitude;
								
			} 

		}
		//now cull everything past the last index.
		while (result.Count > lastValueIndex+1) {
			result.RemoveAt (result.Count - 1);
		}

		return result;
		
	}
	
	delegate float Extractor(AutographerMetaDataItem mdi); //used in extractField
	public List<float> extractField(GraphField gf) {
		List<float> result = new List<float>();
		List<MetaDataItem> items = this.output;
		Extractor e;
		switch (gf) {			
			case GraphField.AccuracyX: e = delegate(AutographerMetaDataItem mdi) { return mdi.AccuracyX; }; break;
			case GraphField.AccuracyY: e = delegate(AutographerMetaDataItem mdi) { return mdi.AccuracyY; }; break;
			case GraphField.AccuracyZ: e = delegate(AutographerMetaDataItem mdi) { return mdi.AccuracyZ; }; break;
			case GraphField.MagnitudeX: e = delegate(AutographerMetaDataItem mdi) { return mdi.MagnitudeX; }; break;
			case GraphField.MagnitudeY: e = delegate(AutographerMetaDataItem mdi) { return mdi.MagnitudeY; }; break;
			case GraphField.MagnitudeZ: e = delegate(AutographerMetaDataItem mdi) { return mdi.MagnitudeZ; }; break;
			case GraphField.Red: e = delegate(AutographerMetaDataItem mdi) { return mdi.Red; }; break;
			case GraphField.Green: e = delegate(AutographerMetaDataItem mdi) { return mdi.Green; }; break;
			case GraphField.Blue: e = delegate(AutographerMetaDataItem mdi) { return mdi.Blue; }; break;
			case GraphField.Luminance: e = delegate(AutographerMetaDataItem mdi) { return mdi.Luminance; }; break;
			case GraphField.Temperature: e = delegate(AutographerMetaDataItem mdi) { return mdi.Temperature; }; break;
			case GraphField.Latitude: e = delegate(AutographerMetaDataItem mdi) { return mdi.latitude; }; break;
			case GraphField.Longitude: e = delegate(AutographerMetaDataItem mdi) { return mdi.longitude; }; break;
			case GraphField.Altidude: e = delegate(AutographerMetaDataItem mdi) { return mdi.altitude; }; break;
			case GraphField.Gs: e = delegate(AutographerMetaDataItem mdi) { return mdi.Gs; }; break;
			case GraphField.HorizontalError: e = delegate(AutographerMetaDataItem mdi) { return mdi.HorizontalError; }; break;
			case GraphField.VerticalError: e = delegate(AutographerMetaDataItem mdi) { return mdi.VerticalError; }; break;
			case GraphField.OrientationX: e = delegate(AutographerMetaDataItem mdi) { return mdi.xOrientation; }; break;
			case GraphField.OrientationY: e = delegate(AutographerMetaDataItem mdi) { return mdi.yOrientation; }; break;
			case GraphField.OrientationZ: e = delegate(AutographerMetaDataItem mdi) { return mdi.zOrientation; }; break;
			default: e = delegate(AutographerMetaDataItem mdi) { return mdi.AccuracyX; }; break;			
		}
		
		foreach(AutographerMetaDataItem mdi in items) {
			result.Add(e(mdi));
		}
		
		return result;
	}
	
	public override List<float> showGraphOptions() 
	{	
		int i = 0;
		GraphField[] enums = (GraphField[])GraphField.GetValues(typeof(GraphField));		
		foreach (GraphField gf in enums)
		{
			int startX = i < enums.Length/2 ? 0 : Screen.width - 300;
			int height = Screen.height/(enums.Length/2);
			if (GUI.Button(new Rect(startX,(i % (enums.Length/2))*height,300,height),gf.ToString())) {
				return this.extractField(gf);
			}
			i+= 1;
		}
		return null;
	}
}
