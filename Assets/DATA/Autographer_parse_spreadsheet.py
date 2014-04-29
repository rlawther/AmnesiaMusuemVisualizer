'''
Python script to parse an Autographer meta data text file

make sure the 1st and last row have actual lat,lon and alt information (not 0.0000)
https://googledrive.com/host/0ByrQ8FRGctRobVZGM3RNbng0UUE/

python Autographer_parse_spreadsheet.py -i image_table.txt -u https://googledrive.com/host/0ByrQ8FRGctRobVZGM3RNbng0UUE/
'''

import sys,os,csv, argparse

imgFile_ext = "4.jpg" #extension of autographer med resolution images 640x480

parser = argparse.ArgumentParser()
parser.add_argument("-i", type=str, help="Data file txt or csv")
parser.add_argument("-u", type=str, help="Optional: URL to folder with med resolution (640x480) Autographer images")
args = parser.parse_args()

if ((args.i == None) & (args.u == None)):
	print ""
	print "Pyton script to parse Autographer metadata."
	print "It interpolates over missing lat, lon and altitude data."
	print ""
	print "If run on the original image_table.txt, "
	print "it creates a new file in CSV format with added columns:"
	print "  number - consecutive number to identify the row"
	print "  imgURL - an URL path to the medium resolution (640x480) images,"
	print "      for instance: https://googledrive.com/host/0ByrQE/"
	print "  latitude longitude - combined lat lon"
	print "  locInterpolated - a flag to indicate if lat/long "
	print "      was captured by Autographer or interpolated by the script"
	print ""
	print "Usage:"
	print "  -i the input image_table.txt or csv file"
	print "  -u URL path for images"
	print ""
	print ""
	
if (args.i != None):
	AutographerDataFile = args.i
	try:
		f = open(AutographerDataFile, "r")
	except IOError:
		print "Error: can\'t find file or read data"
		sys.exit(0)
		#exit
else: 
	print "Error: no data file specified. Use -i and drag file onto console"
	sys.exit(0)
	#exit
if (args.u == None):
	print "no image URL provided"
	
filePath = os.path.abspath(AutographerDataFile)
path, filename = os.path.split(os.path.abspath(AutographerDataFile)) #path + filename of data

i = 0
data = []
longi = 151.303864
lati = -33.8310
latiIncr = -0.001
temp = 0
temp_interp = [] 

# create a list from data file
org_auto = 0
for line in f:
	if i == 0:
		items = line.rstrip('\r\n').split(',')   # strip new-line characters and split on column delimiter
		items = [item.strip() for item in items]  # strip extra whitespace off data items
		if items[0] == "dt":  # add columns if run 1st time on original autographer image_table.txt
			org_auto = 1
			items.insert(0, "num")
			items.insert(3, "imgURL")
			items.insert(4, "imgFile")
			items.insert(5, "episode")
			items.insert(23, "latitude longitude")
			items.insert(24, "locInterpolated")
		data.append(items)
	if i > 0:
		items = line.rstrip('\r\n').split(',')   # strip new-line characters and split on column delimiter
		items = [item.strip() for item in items]  # strip extra whitespace off data items
		if org_auto == 1:  # add columns if run 1st time on original autographer image_table.txt
			items.insert(0, i)
			items.insert(3,"") #image url
			items.insert(4,"") #image File
			items.insert(5, "") #episode
			items.insert(23, items[21] + " " + items[22]) # lat long
			if (items[21] == "0.00000"): items.insert(24, "1") #locInterpolated
			else: items.insert(24, "0")
		data.append(items)		
	i+=1

# get column indices for gps related data
for i in range(0, len(data[0])):
	if data[0][i] == 'lat': index_lat = i
	if data[0][i] == 'lon': index_lon = i
	if data[0][i] == 'latitude longitude': index_latlon = i
	if data[0][i] == 'alt': index_alt = i
	if data[0][i] == 'gps': index_gps = i
	if data[0][i] == 'imgURL': index_imgURL = i
	if data[0][i] == 'imgFile': index_imgFile = i
	if data[0][i] == 'id': index_id = i

if (args.u != None):  #image url
	print ""
	print ("Image URLs added: " + args.u + data[1][index_id] + imgFile_ext + " ...")
	for i in range(1, len(data) ): data[i][index_imgURL] = (args.u + data[i][index_id] + imgFile_ext)

#image File name
for i in range(1, len(data) ): data[i][index_imgFile] = (data[i][index_id] + imgFile_ext)

# interpolation between latitude,longitude ant altitude across two indices
def interpolate(startIndex, endIndex):
	indexList = []
	latList = []
	lonList = []
	altList = []
	latLonAltList = []
	latIncr = (float(data[endIndex][index_lat]) - float(data[startIndex][index_lat])) / (endIndex - startIndex)
	lonIncr = (float(data[endIndex][index_lon]) - float(data[startIndex][index_lon])) / (endIndex - startIndex)
	altIncr = (float(data[endIndex][index_alt]) - float(data[startIndex][index_alt])) / (endIndex - startIndex)
	
	for x in range(startIndex +1,endIndex ):
		indexList.append(x)
	latLonAltList.append(indexList)
	
	t = float(data[startIndex][index_lat])
	for x in range(0,(endIndex - startIndex -1)):
		t = t + latIncr
		latList.append(t)
	latLonAltList.append(latList)
	
	t = float(data[startIndex][index_lon])
	for x in range(0,(endIndex - startIndex -1)):
		t = t + lonIncr
		lonList.append(t)
	latLonAltList.append(lonList)	
	
	t = float(data[startIndex][index_alt])
	for x in range(0,(endIndex - startIndex -1)):
		t = t + altIncr
		altList.append(t)
	latLonAltList.append(altList)
	
	return latLonAltList

i1 = 0
i2 = 0
print ""
for i in range(2, len(data)-1):
	if ( (float(data[i][index_lat]) == 0.0) and ( float(data[i-1][index_lat]) != 0.0) ): i1 = i-1
	if ( (float(data[i][index_lat]) == 0.0) and ( float(data[i+1][index_lat]) != 0.0) ): i2 = i+1
	#print str(i1) + " " + str(i2)
	if ((i1 != 0) & (i2 != 0)): 
		print ("lat-lon interpolation between: " + str(i1) + " " + str(i2))
		temp_interp = interpolate(i1,i2)
		for k in range(0,len(temp_interp[0])): #loop through number of interpolations
			data[temp_interp[0][k]][index_lat] = temp_interp[1][k]
			data[temp_interp[0][k]][index_lon] = temp_interp[2][k]
			data[temp_interp[0][k]][index_alt] = temp_interp[3][k]
			data[temp_interp[0][k]][index_latlon] = (str(temp_interp[1][k]) + " " + str(temp_interp[2][k]))
		i1 = 0
		i2 = 0
		temp_interp = []
		
# print data[125][index_lat]
# print data[132][index_lat]



# write csv file
'''print path
print filename[0:len(filename)-4]
print os.path.join(path, (filename[0:len(filename)-4]) + "_1.CSV")
print filePath[0:len(os.path.split(filePath))]
print filePath
print os.path.split(filePath)[1]
print os.path.split(filePath)[0]
'''
with open( os.path.join(path, (filename[0:len(filename)-4]) + "_1.CSV") , 'wb') as myfile:
    wr = csv.writer(myfile, quoting=csv.QUOTE_NONE)
    for row in data:  wr.writerow(row)



