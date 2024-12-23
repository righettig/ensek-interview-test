# ensek-interview-test

## How to run
- using Docker Compose:
	- add ".env" file at root folder
	- adding defs for: METER_READING_DATABASE_NAME, COSMOSDB_CONNECTION_STRING, BACK_END_API_URL
	- for instance: 
		- BACK_END_API_URL=http://localhost:4000
		- METER_READING_DATABASE_NAME=ensek-spark-db
		- COSMOSDB_CONNECTION_STRING=AccountEndpoint=https://cosmosdb:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==
	- launch from root folder: docker compose up --build
	- React front-end is available at http://localhost:5000
	- A Postman collection can be found /tests/ENSEK.postman_collection.json for API testing

## User story:
	
	As an Energy Company Account Manager I want to be able to load a CSV file of Custom Meter Readings
	so that we can monitor their energy consumption
	and charge them accordingly
	
### Premiminary task decomposition:
		
	DONE create API placeholder
	
	DONE set up db (cosmosdb)
		DONE create docker compose file
		DONE create dockerfile

	DONE set up postman collection

	DONE seed placeholder backgroundservice to load data in db
		read user file
		use cosmos db SDK to upsert user account
	
	DONE create dbcontext for meter readinng
	DONE create meter reading repository service

	DONE implement endpoint to read data from csv file
		validations: 
			1) no duplicates
			2) no invalid account ids allowed
			3) check format to be NNNNN

	DONE update postman collection in order to test the endpoint

	DONE unit testing
		controller (return number of failures/success)
		service: validations
		other?

	DONE code cleanup & refactoring
		Program.cs
		/Data folder structure

	DONE validation: new read SHOULD NOT be OLDER than existing one

	DONE create react client
		load meter readings
		upload meter readings
		improve visuals
		format timestamp
		cleanup: create sub-components
		table for meter readings
		display validation results

	DONE improve documentation
	
	DONE add screenshots
	
### API design:
	
	POST /meter-reading-uploads
		validate
		if OK store in DB
		return numer of successful/failed readings
	
### Entities
	
	MeterReading 
		AccountId, MeterReading, DateTime, MeterReadValue,

	User
		AccountId, FirstName, LastName
	
### Improvements
	0) discuss about non-functional requirements and get more info about usage patterns, etc.
	1) consider splitting user account related code into separate microservice then introduce API gateway to route requests
	2) introduce event sourcing (to capture changes for auditing), show custom framework designed and implemented for AIRO	
	3) multipart upload (when file is too big / support resume scenario)
	4) discuss how to scale (e.g., introduce caching for validation checks, can we afford eventual consistency though?)
	5) added a note in MeterReadingService about a potential refactoring to support more validations
	6) add sql server db
	7) add architectural diagram
	