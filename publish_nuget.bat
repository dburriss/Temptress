dotnet restore src\Temptress\project.json
dotnet pack src\Temptress\project.json -c Release -o artifacts\bin\Temptress\Release

set /p version="Version: "
nuget push artifacts\bin\Temptress\Release\Temptress.%version%.nupkg