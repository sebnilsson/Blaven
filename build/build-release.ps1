$OutputPath = "../build-artifacts/"
$RestorePath = "../Blaven.sln"
$SrcPath = "../src/"

function Build-Project {
    param(
        [String]$ProjectName,
        [String]$Framework)

    $Path = $SrcPath + "$ProjectName/$ProjectName.csproj"
    $BuildOutput = $OutputPath + "$Framework/"

    Write-Host "Building project"
    Write-Host "- Path: '$Path'"
    Write-Host "- Framework: '$Framework'"
    Write-Host "- BuildOutput: '$BuildOutput'"

    dotnet build $Path -c Release -o "../$BuildOutput" -f $Framework --no-restore -v m
}

function Write-Section {
    param($Text)

    Write-Host
    Write-Host "--- $Text ---"
    Write-Host
}

Write-Section "Ensuring output-path: '$OutputPath'"

if (Test-Path $OutputPath) {
    Remove-Item -path $OutputPath -recurse
}
New-Item -ItemType Directory -Path $OutputPath | out-null

Write-Section "Restoring dependencies"

dotnet restore $RestorePath

Write-Section "Building projects"

Build-Project "Blaven" "netstandard1.3"
Build-Project "Blaven.BlogSources.Blogger" "netstandard1.3"
Build-Project "Blaven.DataStorage.EntityFramework" "netstandard2.0"
Build-Project "Blaven.DataStorage.RavenDb" "netstandard1.3"