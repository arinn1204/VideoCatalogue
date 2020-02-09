param (
    [string] $projectLocation = '.',
    [string] $Configuration = 'Release',
    [string] $DebugPreference = 'Continue'
)
$global:ProgressPreference = 'SilentlyContinue'

Write-Host "Changing to... $projectLocation"
Push-Location $projectLocation

function createArtifact() {
    param (
        [string] $Project
    )

    Write-Host "`nStarting on Project: $Project"

    Push-Location $Project

    dotnet clean -c $Configuration | Write-Debug
    dotnet build -c $Configuration | Write-Debug

    if ( ! $? ) {
        Pop-Location
        throw "Build failed, see previous errors."
    }

    Write-Host "Creating archive..."
    Compress-Archive -Path "bin\$Configuration\netcoreapp3.0\*" -DestinationPath "..\$Project.zip" -Force | Write-Debug

    Pop-Location


    Write-Host "Archive $Project.zip created."
    Get-Item "$Project.zip"
}


createArtifact -Configuration $Configuration -DebugPreference $DebugPreference -Project "Silo"
createArtifact -Configuration $Configuration -DebugPreference $DebugPreference -Project "RenamerClient"

Pop-Location