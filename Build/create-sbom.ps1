# Version: 2
# Note: The previous line must be the first line in the script
#   It provides version information to the caller, allowing the parameters to change.
#   Otherwise, changing the parameter would break old versions
# 
# Changelog:
#  - Version 2: Replaced $OutputPath by $OutputFolder
param(
    [string] $TempFolder = "Build\BuildOutput\temp\sbom",
    [Parameter(mandatory=$true)] [string] $NpmProjectJson,
    [Parameter(mandatory=$true)] [string] $Solution,
    [Parameter(mandatory=$true)] [string] $GithubUsername,
    [Parameter(mandatory=$true)] [string] $GithubAccessToken,
    [Parameter(mandatory=$true)] [string] $OutputFolder
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

$OutputFolder = [System.IO.Path]::GetFullPath($OutputFolder)
[System.IO.Directory]::CreateDirectory($OutputFolder)
Write-Host "Using output folder: $OutputFolder"

$OutputSbomXmlProd = "$OutputFolder\re-motion.sbom.xml"
$OutputSbomJsonProd = "$OutputFolder\re-motion.sbom.json"
$OutputSbomXmlDev = "$OutputFolder\re-motion.development.sbom.xml"
$OutputSbomJsonDev = "$OutputFolder\re-motion.development.sbom.json"

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

function Create-SBOM {
    param(
        $isProduction,
        $outputXml,
        $outputJson
    )

    if ($isProduction) {
        $suffix = "production"
    } else {
        $suffix = "development"
    }

    Write-Host "Analyzing NPM dependencies ($suffix)"
    if ($isProduction) {
        npx "@cyclonedx/cyclonedx-npm" $NpmProjectJson --omit dev --output-format xml --output-file "npm-dep-$suffix.xml"
    } else {
        npx "@cyclonedx/cyclonedx-npm" $NpmProjectJson --output-format xml --output-file "npm-dep-$suffix.xml"
    }

    Fail-On-Error "cyclonedx-npm"

    Write-Host "Analyzing Nuget dependencies ($suffix)"
    if ($isProduction) {
        .\dotnet-CycloneDX.exe -o . --disable-package-restore --exclude-dev --exclude-test-projects -gu $GithubUsername -gt $GithubAccessToken -f "dotnet-dep-$suffix.xml" $Solution
    } else {
        .\dotnet-CycloneDX.exe -o . --disable-package-restore -gu $GithubUsername -gt $GithubAccessToken -f "dotnet-dep-$suffix.xml" $Solution
    }

    Fail-On-Error "dotnet-CycloneDX.exe"
    
    Write-Host "Merging NPM and Nuget SBOMs ($suffix)"
    .\cyclonedx-cli.exe merge --input-files "npm-dep-$suffix.xml" "dotnet-dep-$suffix.xml" --output-file $outputXml
    Fail-On-Error "cyclonedx-cli.exe merge"

    Write-Host "Add JSON output ($suffix)"
    .\cyclonedx-cli.exe convert --input-file $outputXml --output-file $outputJson
    Fail-On-Error "cyclonedx-cli.exe convert"
}

Create-SBOM $true $OutputSbomXmlProd $OutputSbomJsonProd
Create-SBOM $false $OutputSbomXmlDev $OutputSbomJsonDev

cd $originalDirectory
