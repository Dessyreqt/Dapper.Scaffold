# Dapper.Scaffold

A simple Dapper scaffolding tool. Extracts models from your database and creates simple CRUD operations as extension methods.

## Code License

This code is licensed under GPL-3.0. See the LICENSE file for more information.

## Usage

The tool is a command line tool. Example usage:

`dapper-scaffold.exe -g mssql -c "Server=localhost;Database=Project;Trusted_Connection=True;" -p "./Models" -n "Project.Models" -f -e -d`

This will generate models and basic Create/Read/Update/Delete operations for the Project database on the local SQL Server instance and place them in the `./Models` folder with the namespace `Project.Models`.

In the code, extension methods can be used like this:

```csharp
// Read

var existingCustomer = await connection.GetCustomerByIdAsync(1); // gets Customer with Id 1
var allCustomers = await connection.GetCustomerListAsync(); // gets all Customers
var texasCustomers = await connection.GetCustomerListAsync("[State] = @State", new { State = "TX" }); // gets all Customers from Texas



// Create

var newCustomer = new Customer { Name = "John Doe", State = "TX" };
await connection.InsertAsync(newCustomer); // inserts newCustomer. The CustomerId property will be updated with the value inserted.

var otherNewCustomer = new Customer { Name = "Jane Doe", State = "TX" };
await connection.SaveAsync(otherNewCustomer); // decides to either update or insert otherNewCustomer based on the value of the Id property. The CustomerId property will be updated with the value inserted since this is a new customer.



// Update

existingCustomer.Name = "John Smith";
await connection.UpdateAsync(existingCustomer); // updates existingCustomer

existingCustomer.Name = "John Doe";
await connection.SaveAsync(existingCustomer); // decides to either update or insert existingCustomer based on the value of the Id property. In this case an update is performed.



// Delete

await connection.DeleteAsync(existingCustomer); // deletes existingCustomer
```

### Command line arguments

`-g` Database script generator to use.

`-c` Connection string to the database.

`-p` Path to the folder where the models will be saved.

`-n` Namespace for the models.

`-f` Force overwrite of existing files.

`-e` Generate extension methods for CRUD operations.

`-d` Clear the folder before generating the models.

`--help` Show help.

### Supported databases

The `-g` flag support the following:

- SQL Server (`mssql`, or if `-g` is omitted)
- PostgreSQL (`postgres`)

Please let me know if you would like support for other databases.

### Supported features

- Generate models from tables.
- Generate extension methods for CRUD operations.

### Currently unsupported features

- Does not support views.
- Does not support stored procedures.
- Only generates async methods.

## Building the project

At the moment, only development on Windows is supported. This is due to the use of support tools that are only available on Windows. This may change in the future.

Your development environment should have the following installed:
- .NET 8.0 SDK [Download](https://dotnet.microsoft.com/download/dotnet/8.0)

Run the publish.bat script to publish the project. The output will be in the `./Dapper.Scaffold/bin/Release/net8.0/win-x64/publish/` folder.
