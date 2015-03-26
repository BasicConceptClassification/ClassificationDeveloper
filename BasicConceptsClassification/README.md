# DEPLOYMENT NOTES
The project requires Neo4j to be running on localhost:7474 to provide the NoSQL database. Additionally, the current connection string for SQL database needs to be changed within the root web.config file to:  
```
Server=.\SQLEXPRESS;Database=<DBNAME>;Integrated Security=true  
```
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
1. Connect to the server using RDP.
  1. You need to configure the server to accept RDP from your IP address. Navigate to the EC2 instance in the AWS dashboard.
  2. In the details section for the instance, find "Security Groups" and select the first (and only) group.
  3. In the Security Group's details, click the Inbound tab, then select Edit.
  4. One of the rules is for RDP. In the source column, select "My IP" from the dropdown menu.
  5. Save the settings and initiate the connection to the server using your RDP file (double click the .rdp you downloaded earlier).
2. Deploy the current build from Visual Studio
  1. Build the solution in Release configuration (Change "Debug" to "Release" in the top toolbar).
  2. Right click the BccApplication project and select "Publish to AWS..."
  3. If you haven't already, enter you account credentials by clicking the "Add" button under the profile tab (it looks like a little dude with a plus sign).
  4. Under "Deployment Target", select "Redeploy to an existing environment".
  5. Select BccApplication-deploy from the list of options (it should be the only available option).
  6. Click through the next page and click finish.
3. Within the RDP, configure the application.
  1. Start Neo4j if it isn't running already.
  2. Open SQL Server Management Studio, and take a note of what the domain name is (Currently WIN-ICN8OQSDEE7)
  3. Open Internet Information Services Manager
  4. Navigate to Sites/Default Web Site/
  5. Double click Connection Strings, and select Default Connection.
  6. Select SQL Server, and fill in <DOMAIN_NAME>\SQLEXPRESS for Server and master for Database (<DOMAIN_NAME> is that domain you noted earlier)
  7. Press OK to save your changes.
4. The deployment should now be live. You should be able to navigate to bccapplication-deploy.elasticbeanstalk.com to test.