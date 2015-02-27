"""
Python 3 compliant. Not sure about python 2.?

Converting from a .owl from Protege into a CSV file.
Specifically, this is for the CMPUT 401 BCC project.

Usage:

	> python3 owl_conversion.py <OWL_FILE> <OUTPUT_FILE> 

	<OUTPUT_FILE> should exist. Can be a .csv.

<OWL_FILE> from protege format for each GLAM object:

<!-- comment with TERM_ID -->
<owl:ObjectProperty rdf:about"TERM_ID">
	<rdfs:label>TERM_LABEL</rdfs:label>
	<rdfs:subPropertyOf rdf:resource="TERM_SUBPROPERTY_ID"/>
</owl:ObjectProperty>

<OUTPUT_FILE> (or CSV) format:

	TermId,TermLabel,PropertyType,TermSubPropertyId

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
items_found = 0
limit = 200

# Need input and output filenames provided
if len(sys.argv) < 3:
	print("Expected usage: owl_conversion.py <OWL_FILE> <OUTPUT_FILE>")
	print("Exiting program\n")
	sys.exit()


owl_file = open(sys.argv[1])
output_file = open(sys.argv[2], "w")

output_file.write("\"TermId\",")
output_file.write("\"TermType\",")
output_file.write("\"TermLabel\",")
output_file.write("\"TermSubProperty\"\n")

found_glam = False
output_line = ""


object_types = {}
current_type = ""
type_count = 0

for line in owl_file:
	# indicating start and end of a GLAM object
	if "<!--" in line:
		found_glam = True
		output_line = ""
	elif (found_glam) and ("</owl:" in line):
		found_glam = False
		output_line += "\n"
		output_file.write(output_line)
		items_found += 1
		if DEBUG:
			print(output_line)

	# If the Id line...
	if (found_glam) and ("<owl:" in line ) and ("rdf:about" in line):
		match = re.search('about=(.+?)>', line)
		if match:
			output_line = output_line + match.group(1) + ","
		else:
			print("rdf:about - Missing something here:")
			print(line)
			sys.exit()

		match = re.search('<owl:(.+?) rdf:about', line)
		if match:
			output_line = output_line + match.group(1) + ","
			
			if match.group(1) != current_type:

				object_types[current_type] = type_count
				type_count = 0
				current_type = match.group(1)
			
			else:
				type_count += 1
		else:
			print("rdf:about - Missing this type:")
			print(line)
			sys.exit()
	
	# If the label line...
	if (found_glam) and ("<rdfs:label" in line):
		match = re.search('<rdfs:label>(.+?)</rdfs:label>', line)
		if match:
			output_line = output_line + "\"" + match.group(1) + "\""

	# If the subpropertyId line...
	if (found_glam) and ("<rdfs:" in line) and ("rdf:resource=" in line):
		match = re.search('rdf:resource=(.+?)/>', line)
		if match:
			output_line += "," + match.group(1)

	if LIMIT:
		if i > limit:
			break
	i += 1

object_types[current_type] = type_count

print("Total items: {0}\n".format(items_found))

actual_items = 0
for key in object_types.keys():
	print("Type: {0}\tCount: {1}\n".format(key, object_types[key]))
	actual_items += object_types[key]

print("Total items actually found: {0}\n".format(actual_items))
