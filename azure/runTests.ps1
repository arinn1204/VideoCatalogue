param (
    [string] $type, # this is the type of test, ie unit vs integration, vs e2e etc
    [string] $useEnvironmentConnectionString = 'false',
    [string] $projectLocation = '.',
    [string] $Configuration = 'Release',
    [string] $DebugPreference = 'Continue',
    [string] $TheMovieDatabaseKey = '',
    [string] $dbCatalog = '',
    [string] $dbUsername = '',
    [string] $dbPassword = '',
    [string] $dbSource = ''
)

Push-Location $projectLocation\Tests

switch -regex ($type) {
    "[iI]ntegration" { $projectEnd = "Integration$"; break; }
    "[uU]nit" { $projectEnd = "Unit$"; break; }
    default { Pop-Location; throw 'Unknown test type' }
}

$tests = Get-ChildItem | Where-Object {$_.Name -match $projectEnd }

$Env:TheMovieDatabase:Authorization = $TheMovieDatabaseKey
$Env:environment_connection_string = $useEnvironmentConnectionString;
$Env:db_source = $dbSource;
$Env:db_catalog = $dbCatalog;
$Env:db_username = $dbUsername;
$Env:db_password = $dbPassword;

foreach($testProject in $tests) {
    Push-Location $testProject

    dotnet clean -c $Configuration | Write-Debug
    dotnet build -c $Configuration

    if (!$?) {
        Pop-Location
        throw 'Build failed. See build output'
    }

    dotnet test -c $Configuration --no-build --logger:trx /p:CollectCoverage=true "$testProject.csproj"
    if (!$?) {
        Pop-Location
        throw 'Test failed. See test output'
    }

    Pop-Location
}

Pop-Location