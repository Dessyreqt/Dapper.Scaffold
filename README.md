# Dapper.Scaffold

A simple Dapper scaffolding tool. Extracts models from your database and creates simple CRUD operations as extension methods.

## Code License

This code is licensed under GPL-3.0. See the LICENSE file for more information.

## Usage

The tool is a command line tool. Example usage:

`dapper-scaffold.exe -c "Server=localhost;Database=Project;Trusted_Connection=True;" -p "./Models" -n "Project.Models" -f -e -d`

### Command line arguments

`-c` Connection string to the database.

`-p` Path to the folder where the models will be saved.

`-n` Namespace for the models.

`-f` Force overwrite of existing files.

`-e` Generate extension methods for CRUD operations.

`-d` Clear the folder before generating the models.

`--help` Show help.

## Building the project

At the moment, only development on Windows is supported. This is due to the use of support tools that are only available on Windows. This may change in the future.

Your development environment should have the following installed:
- .NET 8.0 SDK [Download](https://dotnet.microsoft.com/download/dotnet/8.0)

Run the publish.bat script to publish the project. The output will be in the `./Dapper.Scaffold/bin/Release/net8.0/win-x64/publish/` folder.
