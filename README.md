# Cvent-Webhooks-Endpoint
Webhooks Endpoint

The code in this repository is used in a hosted REST API Endpoint that listens for webhook calls from the Cvent Registration Platform .
What are Webhooks? 
Webhooks let you collect data about Cvent events in near real-time. Provide at least one URL, select which actions you want to trigger a data transfer, and Cvent will send data to the URL whenever one of the actions occurs.
The code in this API accepts attendee registration data, logs the information in a database and then pushes the registration information to a CRM system. 
Please note that, although the project was created using Visual Studio, the project files in this repository are not complete and will not run ‘as is’. The code is meant to be a helpful reference for others attempting to set up an endpoint for Cvent woobooks. I also removed sensitive information from all web config files.

Few things to keep in mind;
•	The URL for the endpoint must be an SSL secured (HTTPS) URL
•	You must set up an authentication token that your application can use to verify every call from Cvent. This can be stored in the web config file.
•	One must gather and track all custom Cvent question GUIDs. These can be saved in the web config file.
•	One can specify or change an endpoint by logging into Cvent, go to Integrations, then Push API Integrations. Cvent also has webhook documentation they can provide to the developer.
•	One can set up multiple endpoints in Cvent. For this implementation, the item called "Push APAP Reg" is used to keep track of the default endpoint as well as the authentication token Cvent should use to communicate with that endpoint.
•	The endpoint ending in "CventReg" is what will accept a JSON object from Cvent every time a person registers and triggers a webhooks.
•	Much of the registration information is not provided by this JSON object. One may need to pull additional information from Cvent’s API, which this implementation includes.
•	This implementation of integration pushes registration information to a custom client web service, one that was created by Protech, which in turn creates an attendee staging record in MS Dynamics CRM
•	To link an endpoint to a specific event, go to Events, click on the Cvent event after logging in, go to  -> Event Details -> Push API, click: Add Webhooks, Edit Webhooks, select "Yes", select configuration, Synced Object "Attendees".
•	Keep in mind that the endpoint must be able to accept and process multiple registration records at once. One off webhook calls will only include information for the most recent registrant, but the “Synch” method in Cvent will push all registrations at once in one JSON object.
•	The following is the endpoint that listens for the webhook call; The file “CventRegController.cs” in Controllers has a Post method, which is invoked when the Cvent Webhooks posts JSON data to this endpoint.
•	If the header contains a valid token, iterate through each registration that is not a ‘guest’ registration, check to see if they have already been processed in a previous call, and if they have not, pull additional information from the Cvent API and post to the APAP_UpdateMember Web Service.

