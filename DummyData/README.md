## Manually Importing Data 

Until there's an automated process, use this for now.

#### What's Included

There are directories organized by what they will import. They will have an import 'script', data stored in a .csv file, and test queries to show you some meaningful data.

There are also connecting directories which will connect the data of two import sections. Make sure you've imported the correct group of data before you start connecting them!

The ConversionProcess directory contains some (horrible) python scripts and raw data. More for record keeping than anything.


#### Importing Order
 
Possible Order:

1. importClassifiables - smaller dataset and is "proper".  
2. importBCC - much larger dataset. Should be good for wanting to view the classifiacation.  
3. importClassifiers - smaller dummy dataset that may change, not many changes from last time.    
4. connectingBCC_Classifiables - requires importClassifiables and importBCC to be completed.


Once there's an automated system, this won't matter as much.


#### How to manually import data into Neo4j.

1. (Download and) Start Neo4j and go to the Neo4j WebBrowser.
2. Delete Previous Data
        - Copy to delete previous data: MATCH (n) OPTIONAL MATCH (n)-[r]-() DELETE n,r
2. Copy each STEP of the Cypher statements ONE AT A TIME into the CLI 
   at the top of the Neo4j Web Browser and run them by clicking the arrow with the circle inside to the right hand side. **NOTE: You may have to edit the path of each of the '.csv' files so the CSV import can find it in the IMPORT steps**
3. To check that you have data, click the STAR icon on the left hand side of
   the Neo4j Web Browser and click "Get some data". Should get some data!
	- Additionally, you can run the testQueries commands in the testQueries files in the same directory as the data you just imported to get a feel of how the data is related.
