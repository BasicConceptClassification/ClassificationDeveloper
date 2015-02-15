"""
Python 3 compliant. Not sure about python 2.?

Converting from a .owl from Protege into a CSV file.
Specifically, this is for the CMPUT 401 BCC project.

Usage:

	> python3 raw_GLAMdata_conver.py <RAW_DATA> <OUTPUT_FILE> 

	<OUTPUT_FILE> should exist. Can be a .csv.


<RAW_DATA> from the National Gallery (US) Hilights format for each GLAM object:

Name sep by spaces space (maybe)(a)(concept)(string)


<OUTPUT_FILE> (or CSV) format:

	ItemId,ItemLabel,ConceptString,Glam,ClassifierId

Regex Reference:
http://stackoverflow.com/questions/4666973/how-to-extract-a-substring-from-inside-a-string-in-python
"""

import sys
import re

# Some dubugging and limiting variables
# i is to limit how far the test data goes... 
DEBUG = False
LIMIT = False
i = 0
limit = 200

# Need input and output filenames provided
if len(sys.argv) < 3:
	print("Expected usage: owl_conversion.py <OWL_FILE> <OUTPUT_FILE>")
	print("Exiting program\n")
	sys.exit()


input_file = open(sys.argv[1])
output_file = open(sys.argv[2], "w")

output_file.write("\"ItemId\",\"ItemName\",\"ConceptString\",")
output_file.write("\"Glam\",\"ClassifierId\"\n")

output_line = ""

for line in input_file:

	partitions = line.partition("(")
	
	output_line = "\"" + str(i) + "\","

	output_line += "\"" + partitions[0].strip() + "\"" + "," + "\"" + partitions[1] + partitions[2] + "\""
	output_line += "," 


	output_line += "\"https://sites.google.com/a/ualberta.ca/rick-szostak/publications/appendix-to-employing-a-synthetic-approach-to-subject-classification-across-glam/archaeology-object-name-list-used-by-the-us-national-parks-service\",\"99999\"\n"

	output_file.write(output_line)

	print(output_line)
	if LIMIT:
		if i > limit:
			break
	i += 1
