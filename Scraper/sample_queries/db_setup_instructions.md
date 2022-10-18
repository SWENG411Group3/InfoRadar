1. Install Microsoft SQL Management Studio if you haven't already
2. Create a database called `info_radar`
3. Make a SQL server authenticated login
4. Make a .env file in this project's root directory with the following parameters:
    1. `DB_NAME=info_radar`
    2. `DB_HOST="(local)"`
    3. `DB_USER=<ACCOUNT_USERNAME>`
    4. `DB_PASSWORD=<ACCOUNT_PASSWORD>`
5. Open Visual Studio and execute Update-Database in your package manager console
6. Run the init_sample.sql file to populate the DB with a sample lighthouse
7. Install pip modules from requirements.txt
8. Install [OBDC SQL Server driver](https://docs.microsoft.com/en-us/sql/connect/odbc/download-odbc-driver-for-sql-server)
9. Running `py Scraper/main.py` should work now 
