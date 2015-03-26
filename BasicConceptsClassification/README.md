# DEPLOYMENT NOTES
The project requires Neo4j to be running on localhost:7474 to provide the NoSQL database. Additionally, the current connection string for SQL database needs to be changed within the root web.config file to:  
"Server=.\SQLEXPRESS;Database=<DBNAME>;Integrated Security=true"  
where <DBNAME> is whichever database is currently running on the machine for ASP.NET applications ("master" or "aspnet" usually). Ideally, the SQL database should be migrated from express to a more robust version of Microsoft SQL Server, but since we didn't really  have a budget for this project, it's what we've been using for now.
### "Foolproof" guide to deployment using Visual Studio
##### Before you start
* Download the AWS Toolkit from [this site](http://aws.amazon.com/visualstudio/) and install it. Note that I've had some trouble with this toolkit on my Windows 7 machine, but AFAIK it can be fixed by re-installing it.
* Get an RDP file from the AWS dashboard. If you're not on Windows, you'll also need to download an RDP client. Log onto Amazon Web Services, then find and click these links:
  * EC2
  * Running Instances
  * Select BccApplication-deploy from the list (there should only be one instance running).
  * Actions > Connect
  * Select "Download Remote Desktop File" to get the RDP, and "Get Password" to get the administrator password. You'll need the private key file that I emailed out a while back.
##### Deployment Steps