# Timed Session Logger

C# .NET console app using Spectre.Console designed to log occurences of an event and duration to monitor a "timed session". Implements CRUD operations using a dedicated web API. 

## Demo
![Demo Animation](../demo/demo_app.gif?raw=true)

## Features

* Display logged timed sessions in table with following details:
    * Event type
    * Date
    * Duration
* Create new timed session from user input
* Update timed session from user input
   * Select from persisted sessions - can filter displayed option list by date
* Delete timed session from user input
    * Select from persisted sessions - can filter displayed option list by date

## Pre-requsites 

### Dependencies 

* .NET 10.0 installation
* [TimedSessionAPI](https://github.com/cesca2/TimedSessionAPI) - web API to be hosted locally, see setup instructions in the linked repo

## Run Locally

Clone the project

```bash
  git clone git@github.com:cesca2/TimedSessionConsole.git
```

Go to the project directory

```bash
  cd TimedSessionConsole
```

Ensure web API is running locally (see pre-requisites above)

Run the application

```bash
  dotnet run
```


## Helpful Resources

 - Project inspiration from [Habit Logger](https://www.thecsharpacademy.com/project/12/habit-logger) and [Shifts Logger](https://www.thecsharpacademy.com/project/17/shifts-logger) projects
 - [Http requests - Microsoft learn](https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/console-webapiclient)


