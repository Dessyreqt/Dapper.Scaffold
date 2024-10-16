# Dapper.Scaffold

A simple Dapper scaffolding tool. Extracts models from your database and creates simple CRUD operations as extension methods.

## Minimal example

The only required argument is the connection string to the database:

`dapper-scaffold -c "Server=localhost;Database=Project;Trusted_Connection=True;"`

This will generate models in the current folder for the Project database on the local SQL Server instance.

For more information, see the [documentation on GitHub](https://github.com/Dessyreqt/Dapper.Scaffold).

### Supported databases

The `-g` flag supports the following:

- SQL Server (`mssql`, or if `-g` is omitted)
- PostgreSQL (`postgres`)

Please let me know if you would like support for other databases.
