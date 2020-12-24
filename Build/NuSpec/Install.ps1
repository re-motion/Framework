param($installPath, $toolsPath, $package, $project)

function AddChildItem ($fileSystemItem, $projectItem)
{
  if ($fileSystemItem -eq $null)
  {
    return
  }

  if ($projectItem -eq $null)
  {
    return
  }

  if ($fileSystemItem -is [System.IO.DirectoryInfo])
  {
    $childProjectItem = $projectItem.ProjectItems.AddFolder($fileSystemItem.Name)
    foreach ($childItem in Get-ChildItem $fileSystemItem.FullName)
    {
      AddChildItem $childItem $childProjectItem
    }
  }
  else
  {
    $projectItem.ProjectItems.AddFromFile($fileSystemItem.FullName) > $null
  }
}

function GetProjectTypeGuids($project)
{
  $solutionComObject = [Microsoft.VisualStudio.Shell.Package]::GetGlobalService([Microsoft.VisualStudio.Shell.Interop.IVsSolution])
  $solution = Get-Interface $solutionComObject ([Microsoft.VisualStudio.Shell.Interop.IVsSolution])

  $aggregatableProjectAsComObject = $null
  $result = $solution.GetProjectOfUniqueName($project.UniqueName, [ref] $aggregatableProjectAsComObject)
  if ($result -ne 0)
  {
    return $project.Kind.Split(';')
  }

  $aggregatableProject = Get-Interface $aggregatableProjectAsComObject ([Microsoft.VisualStudio.Shell.Interop.IVsAggregatableProject])

  $projectTypeGuids = $null
  $result = $aggregatableProject.GetAggregateProjectTypeGuids([ref] $projectTypeGuids)
  if ($result -ne 0)
  {
    return $project.Kind.Split(';')
  }

  return $projectTypeGuids.Split(';')
}

$projectTypeGuids = GetProjectTypeGuids $project
$webApplicationProjectTypeGuid = '{349c5851-65df-11da-9384-00065b846f21}'
if ($projectTypeGuids -notcontains $webApplicationProjectTypeGuid)
{
  return
}

write-host Installing resource files...

$projectFolder = [System.IO.Path]::GetDirectoryName($project.FullName)
$resProjectFolder = [System.IO.Path]::Combine($projectFolder, 'res')
$packageID = $package.Id
$resPackageFolder =  [System.IO.Path]::Combine($installPath, 'res')

New-Item -Force -ItemType directory -Path $resProjectFolder > $null
$resProjectItem = $project.ProjectItems.AddFromDirectory($resProjectFolder)

$existingResPackageProjectItem = $resProjectItem.ProjectItems | Where-Object { $_.Name -eq $packageID }
if ($existingResPackageProjectItem -ne $null)
{
  $existingResPackageProjectItem.Delete()
}
$existingResPackageFolderInProject = [System.IO.Path]::Combine($resProjectFolder, $packageID)
if (Test-Path -Path $existingResPackageFolderInProject)
{
  Remove-Item -Recurse -Path $existingResPackageFolderInProject
}

$packageProjectItem = $resProjectItem.ProjectItems.AddFolder($packageID)
foreach ($childItem in Get-ChildItem $resPackageFolder)
{
  AddChildItem $childItem $packageProjectItem
}

write-host Resource files have been installed.