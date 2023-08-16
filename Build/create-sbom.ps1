param(
    [string] $TempFolder = "Build\BuildOutput\temp\sbom",
    [Parameter(mandatory=$true)] [string] $NpmProjectJson,
    [Parameter(mandatory=$true)] [string] $Solution,
    [Parameter(mandatory=$true)] [string] $GithubUsername,
    [Parameter(mandatory=$true)] [string] $GithubAccessToken,
    [Parameter(mandatory=$true)] [string] $OutputPath
)
# temp folder: Use BuildOutput\temp as default -> set on TC
# check if tools already exist in the temp folder before downloading again

$ErrorActionPreference = "Stop"
$originalDirectory = Get-Location

trap {
    Write-Host "Unexpected script exception: $($_.InvocationInfo.ScriptName)@$($_.InvocationInfo.ScriptLineNumber): '$($_.InvocationInfo.Line)'"
    Write-Host $_
    cd $originalDirectory
    exit 1
}

function Fail-On-Error {
    param(
        $description
    )

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Executing '$description' failed with exit code $LASTEXITCODE."
        exit 1
    }
}

# Set .NET current directory to enable correct path resolution
[System.Environment]::CurrentDirectory = Get-Location

$TempFolder = [System.IO.Path]::GetFullPath($TempFolder)
[System.IO.Directory]::CreateDirectory($TempFolder)
Write-Host "Using temp folder: $TempFolder"

$NpmProjectJson = [System.IO.Path]::GetFullPath($NpmProjectJson)
Write-Host "Using NPM project: $NpmProjectJson"

$Solution = [System.IO.Path]::GetFullPath($Solution)
Write-Host "Using solution file: $Solution"

$OutputPath = [System.IO.Path]::GetFullPath($OutputPath)
[System.IO.Directory]::CreateDirectory([System.IO.Path]::GetDirectoryName($OutputPath))
Write-Host "Using output path: $OutputPath"

cd $TempFolder

Write-Host "# Installing dependencies"

# Download cyclonedx-npm
Write-Host "Installing cyclonedx-npm"
npm install --save-dev @cyclonedx/cyclonedx-npm

# Download CycloneDX
if (Test-Path "dotnet-CycloneDX.exe") {
    Write-Host "CycloneDX already installed."
} else {
    Write-Host "Installing CycloneDX"
    dotnet tool install CycloneDX --tool-path .
}

# Download cyclonedx-cli
if (Test-Path "cyclonedx-cli.exe") {
    Write-Host "cyclonedx-cli already installed."
} else {
    Write-Host "Installing cyclonedx-cli"
    $releases = Invoke-RestMethod -Uri "https://api.github.com/repos/CycloneDX/cyclonedx-cli/releases" -Headers @{ Authorization="Bearer $GithubAccessToken" }
    $asset = ($releases | Select-Object -First 1).assets | ? { $_.name -eq "cyclonedx-win-x86.exe" }
    Invoke-WebRequest -Uri ($asset.browser_download_url) -OutFile "cyclonedx-cli.exe" -Headers @{ Authorization="Bearer $GithubAccessToken" }
}

# Run the analysis
Write-Host "# Starting analysis"

Write-Host "Ensuring NPM dependencies are up-to-date"
cd ([System.IO.Path]::GetDirectoryName($NpmProjectJson))
npm install
Fail-On-Error "npm install"
cd $TempFolder

Write-Host "Analyzing NPM dependencies"
npx "@cyclonedx/cyclonedx-npm" $NpmProjectJson --omit dev --output-format xml --output-file npm-dep.xml
Fail-On-Error "cyclonedx-npm"

Write-Host "Analyzing Nuget dependencies"
.\dotnet-CycloneDX.exe -o . -gu $GithubUsername -gt $GithubAccessToken -f dotnet-dep.xml $Solution
Fail-On-Error "dotnet-CycloneDX.exe"

Write-Host "Merging NPM and Nuget SBOMs"
.\cyclonedx-cli.exe merge --input-files "npm-dep.xml" "dotnet-dep.xml" --output-file $OutputPath
Fail-On-Error "cyclonedx-cli.exe merge"

Write-Host "Add JSON output"
$JsonOutputPath = [System.IO.Path]::ChangeExtension($OutputPath, ".json")
.\cyclonedx-cli.exe convert --input-file $OutputPath --output-file $JsonOutputPath
Fail-On-Error "cyclonedx-cli.exe convert"

cd $originalDirectory
