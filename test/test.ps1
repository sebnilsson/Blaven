$projects = @(
    "Blaven.BlogSources.Blogger.Tests",
    "Blaven.DataStorage.EntityFramework.Tests",
    "Blaven.DataStorage.RavenDb.Tests",
    "Blaven.DataStorage.RavenDb2.Tests",
    "Blaven.Tests"
)

dotnet build ../Blaven.sln -v minimal -c Release

foreach ($project in $projects) {
    Write-Host "----- $project -----"
    Write-Host

    dotnet test $project --no-build -v minimal -c Release

    Write-Host
}