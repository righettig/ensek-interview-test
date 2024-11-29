# ensek-interview-test

User story:
	
	As an Energy Company Account Manager I want to be able to load a CSV fiel of Custom Meter Readings
	so that we can monitor their energy consumption
	and charge them accordingly
	
Premiminary task decomposition:
		
	DONE create API placeholder
	
	WIP seed db using backgroundservice

	unit test controller (return number of failures/success)
	create [repository] service
	create dbcontext 
	
	validations: 
		1) no duplicates (discuss how to scale using cache?)
		2) no invalid ids allowed
		3) new read SHOULD NOT be OLDER than existing one
		4) check format
		
	create angular client
	create react client
	
API design:
	
	POST /meter-reading-uploads
		validate
		if OK store in DB
		return numer of successful/failed readings
	
Entities
	
	MeterReading 
		AccountId, MeterReading, DateTime, MeterReadValue,

	User
		AccountId, FirstName, LastName
	

BONUS material ?
	1) discuss how to scale
	2) event sourcing 

---

docker compose to 
	launch front-end(s)
	launch db(s)
