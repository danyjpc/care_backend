<<<<<<< HEAD
# Introduction 
TODO: Give a short introduction of your project. Let this section explain the objectives or the motivation behind this project. 

# Getting Started
TODO: Guide users through getting your code up and running on their own system. In this section you can talk about:
1.	Installation process
2.	Software dependencies
3.	Latest releases
4.	API references

# Build and Test
TODO: Describe and show how to build your code and run the tests. 

# Contribute
TODO: Explain how other users and developers can contribute to make your code better. 

If you want to learn more about creating good readme files then refer the following [guidelines](https://docs.microsoft.com/en-us/azure/devops/repos/git/create-a-readme?view=azure-devops). You can also seek inspiration from the below readme files:
- [ASP.NET Core](https://github.com/aspnet/Home)
- [Visual Studio Code](https://github.com/Microsoft/vscode)
- [Chakra Core](https://github.com/Microsoft/ChakraCore)
=======
# CARE - CORE 

Currently CARE-CORE is an implementation of .NET CORE project with support for C#/Angular as front end
.

## Background
The next stack has been used into pattern repository service 

- LINQ
- DbContext
- ASP.NET MVC
- JWT
- KESTREL
- SWAGGER

## Notes for development
Its recommended  use the next work directories

-  app
    - application
        -  /util to add change CONSTANTS and important CONFIG
        -  /controller to add interactivity between routes and controllers
        -  /resources  to add versions of database migration V1_1_1__description.sql
        -  /dto  to add classes as data transfer objects customizable
        -  /repository to add services(dao) bus that would be used by controllers
        -  /repository/interfaces to add interfaces for repositories
        -  /model to save principal entities 
    

## ENVIRONMENTS  
Check docker-compose.yml file , its necessary fill the server-variables.env to send ENVIRONMENTS

- SERVER_DB= database host  as localhost example 
- SERVER_DB_PORT= database port as 5432 example
- SERVER_DB_USER= database user 
- SERVER_DB_PASS= database password
- SERVER_DB_NAME= database name  is necessary to use "database_care" as default
- TERMINAL_MODE = DEV |PROD  to interchange the mode 
- CSN_PWD_ENCRYPT = csn password encrypt

the url it would complete with  "/rest/" 

##SWAGGER
Without docker use https://localhost:5001/swagger/index.html
with docker https://localhost:7000/swagger/index.html

## Register all Repostories , model  and interfaces do the next

To register models go  to util/EntityDbContext.cs and  register on DbSet of LINQ as the next example

```
/*Register models to use in dbSet */
public DbSet<AdmGame> admgames { get; set; }

```


To register repositories go  to Startup.cs and  register on services.AddTransient
```

/*Register Repositories and interfaces */
services.AddTransient<IAdmGame, AdmGameRepository>();

```



## Currently this project supports

- NewTonJson 3.1.3
- FrameWorkCore.Design 5.0
- Extensions 5.0
- Npgsql.EntityFramework.PostgreSQL 5.0
- SeriLog.Sink.Console 4.0
- Serilog 2.1
- Swashbuckle 5.5

## BUILD
you can use buldAndRun.sh to  download/build/run changes from git repository 


## RUN
for branch develop and master this project atumatically build  a docker image an put it inside our private registry  hub.mypeopleapps.com 

Docker image :  hub.mypeopleapps.com/care-core:${mode} ,
mode can be set beta or latest.  
To run this project for production or beta server  run this app inside docker image,  we don need clone repo in servers productions.
use this template yml for  running this app

```yaml
version: '3'
services:
  care-core:
    image: hub.mypeopleapps.com/care-core:${beta}
    container_name: care-core-${beta}
    restart: always
    env_file:
      - ./server-variables.env  # use this file to set all enivornments need
    ports:
      - "7000:80"
      - "8000:443"

  care-webapp:
    image: hub.mypeopleapps.com/care-webapp:${beta}
    container_name: care-webapp-${beta}
    restart: always
    ports:
      - "4200:80"
```


## TEST

Check the access using the  link test using `http://localhost:7000/rest/game`
>>>>>>> 2451c09bec34e917c7b05b84df2df0ebeaefe52a
