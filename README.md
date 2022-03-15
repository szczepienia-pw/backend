# Szczepienia.pw backend

## Setting up the environment
- Create new, empty database called "_io_" in MariaDB. **If database already exists, removed previously created tables!**
- Run migration to create tables and seed sample values.
- Import Postman collection (`app/Postman/IO.postman_collection.json`) and add global variable "IOHostURL" with value "https://localhost:7229".

## Fix for local database
Currently our project doesn't support automagic detection of local and remote environment. For the purposes of CI, appsettings.json contains the path to the VPS database, which should be change to local one.

In `app/appsettings.json` 
replace `"MySQLConnection": "server=mariadb;uid=root;pwd=;database=io"`
with
`"MySQLConnection": "server=localhost;uid=root;pwd=;database=io"`

## Database workflow cheatsheet
### Path to package console 
_Visual Studio 2022 / Tools / NuGet Package Manager / Package Manager Console_

### Generating migration
After introducing change in backend models, you have to prepare migration:
`add-migration [version]`

### Updating database
To propagate the changes to database, you have to update it:
`update-database`

### Reverting changes
It's also possible to revert changes in database by providing previous version of migration:
`update-database [version-1]`