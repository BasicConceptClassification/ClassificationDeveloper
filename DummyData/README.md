## Manually Importing Data 

Until there's an automated process, use this for now.

#### What's Included

Sample data files:
- Classifiables/GLAM objects:   sampleGLAM.csv
- Terms/Classification Tree:    sampleTerms.csv
- Classifiers:                  sampleClassifers.csb

Sample import commands:
- Classifiables/GLAM objects:   importGLAM.txt 
- Terms/Classification Tree:    importTerms.txt
- Classifiers                   importClassifier.txt

**Generally speaking, importGlam.txt and sampleGLAM.csv go together, 
similar for the Terms and Classifiers.**


#### Importing Order
 
Order which imports should be done:

1. Import the Classifiers
2. Import the GLAM itmes
3. Optional: Import the terms

Once there's an automated system, this won't matter as much.


#### How to manually import data into Neo4j.

1. (Download and) Start Neo4j and go to the Neo4j WebBrowser.
2. Delete Previous Data
        - Copy to delete previous data: MATCH (n) OPTIONAL MATCH (n)-[r]-() DELETE n,r
2. Copy each STEP of the Cypher statements ONE AT A TIME into the CLI 
   at the top of the Neo4j Web Browser and run them. **NOTE: You'll have to edit the path of each of the sampleDATA.csv files so the CSV import can find it in the IMPORT steps**
3. To check that you have data, click the STAR icon on the left hand side of
   the Neo4j Web Browser and click "Get some data". Should get some data!
