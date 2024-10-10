dotnet clean -c Release ./Dapper.Scaffold/Dapper.Scaffold.csproj
dotnet publish -c Release -r win-x64 ./Dapper.Scaffold/Dapper.Scaffold.csproj /p:PublishSingleFile=true /p:SelfContained=false /p:IncludeNativeLibrariesForSelfExtract=true
