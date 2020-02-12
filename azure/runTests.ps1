param (
    [string] $type, # this is the type of test, ie unit vs integration, vs e2e etc
    [string] $projectLocation = '.',
    [string] $Configuration = 'Release',
    [string] $DebugPreference = 'Continue'
)

Push-Location $projectLocation\Tests

switch -regex ($type) {
    "[iI]ntegration" { $projectEnd = "Integration$"; break; }
    "[uU]nit" { $projectEnd = "Unit$"; break; }
    default { Pop-Location; throw 'Unknown test type' }
}

$tests = Get-ChildItem | Where-Object {$_.Name -match $projectEnd }

foreach($testProject in $tests) {
    Push-Location $testProject

    dotnet clean -c $Configuration | Write-Debug
    dotnet build -c $Configuration

    if (!$?) {
        Pop-Location
        throw 'Build failed. See build output'
    }

    dotnet test -c $Configuration --no-build --logger:trx /p:CollectCoverage=true $testProject
    if (!$?) {
        Pop-Location
        throw 'Test failed. See test output'
    }

    Pop-Location
}

Pop-Location