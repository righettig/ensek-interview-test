# ensek-interview-test

## How to run
- using Docker Compose:
	- add ".env" file at root folder adding defs for: METER_READING_DATABASE_NAME, COSMOSDB_CONNECTION_STRING
	- launch from root folder: docker compose up --build
	- test API using Postman collection located at /tests/ENSEK.postman_collection.json
	- or 
		- POST http://localhost:4000/meter-reading-uploads
		- GET http://localhost:4000/meter-readings

- using VS
	- TODO

- using Angular/React client
	- TODO


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

	validation: new read SHOULD NOT be OLDER than existing one

	create angular client | create react client
		load meter readings
		filter by account Id
	
		upload meter readings
			display validation results

	integration testing

	improve documentation
	
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
	
### BONUS material ?
	1) add architectural diagram
	2) use sql server db
	3) discuss how to scale (check for validations use a cache?)
	4) event sourcing
	5) split user account related code into separate microservice
	6) AWS resources?
