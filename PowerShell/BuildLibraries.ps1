# Build libraries and nugets
Write-Host "Build libraries (dlls and nugets)..."
$PrevPath = Get-Location
Set-Location $PSScriptRoot

$PublishFolder = "$PSScriptRoot\..\Publish"
$WindowsPublishFolder = "$PublishFolder\Windows"
$LibraryPublishFolder = "$PublishFolder\Libraries"
$NugetPublishFolder = "$PublishFolder\Nugets"

# Publish Loose Libraries
$PublishLibraries = @(
    "StandardLibraries\CentralSnippets\CentralSnippets.csproj"
    "StandardLibraries\CLI\CLI.csproj"
    "StandardLibraries\CSV\CSV.csproj"
    "StandardLibraries\Matrix\Matrix.csproj"
    "StandardLibraries\ODBC\ODBC.csproj"
    "StandardLibraries\Pipeline\Pipeline.csproj"
    "StandardLibraries\Plot\Plot.csproj"
    "StandardLibraries\Python\Python.csproj"
    "StandardLibraries\Razor\Razor.csproj"
    "StandardLibraries\Vector\Vector.csproj"
)
foreach ($Item in $PublishLibraries) {
    dotnet publish $PSScriptRoot\..\$Item --use-current-runtime --output $LibraryPublishFolder
}
# Publish Nugets
$PublishNugets = @(
    "Core\Core.csproj"
    "StandardLibraries\CorePackage\CorePackage.csproj"

    "StandardLibraries\CentralSnippets\CentralSnippets.csproj"
    "StandardLibraries\CLI\CLI.csproj"
    "StandardLibraries\CSV\CSV.csproj"
    "StandardLibraries\Matrix\Matrix.csproj"
    "StandardLibraries\ODBC\ODBC.csproj"
    "StandardLibraries\Pipeline\Pipeline.csproj"
    "StandardLibraries\Python\Python.csproj"
    "StandardLibraries\Razor\Razor.csproj"
    "StandardLibraries\Vector\Vector.csproj"
)
foreach ($Item in $PublishNugets) {
    dotnet pack $PSScriptRoot\..\$Item --output $NugetPublishFolder
}

Set-Location $PrevPath