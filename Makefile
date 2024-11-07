migration-generate:
	dotnet ef migrations add $(name) --project ./src/CityNexus.People.Infra -s ./src/CityNexus.People.Api -o Database/EF/Migrations   

migration-apply:
	dotnet ef database update --project ./src/CityNexus.People.Infra -s ./src/CityNexus.People.Api

migration-remove:
	dotnet ef migrations remove --project ./src/CityNexus.People.Infra -s ./src/CityNexus.People.Api 

    
