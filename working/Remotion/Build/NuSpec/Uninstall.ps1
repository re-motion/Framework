param($installPath, $toolsPath, $package, $project)

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

$projectFolder = [System.IO.Path]::GetDirectoryName($project.FullName)
$resProjectFolder = [System.IO.Path]::Combine($projectFolder, 'res')
$packageID = $package.Id
$resPackageFolder =  [System.IO.Path]::Combine($installPath, 'res')

$resProjectItem = $project.ProjectItems| Where-Object { $_.Name -eq 'res' }
if ($resProjectItem -ne $null)
{
  $resPackageProjectItem = $resProjectItem.ProjectItems | Where-Object { $_.Name -eq $packageID }
  if ($resPackageProjectItem -ne $null)
  {
    $resPackageProjectItem.Delete()
  }

  if ($resProjectItem.ProjectItems.Count -eq 0)
  {
    $resProjectItem.Delete()
  }
}

$resPackageFolderInProject = [System.IO.Path]::Combine($resProjectFolder, $packageID)
if (Test-Path -Path $resPackageFolderInProject)
{
  Remove-Item -Recurse -Path $resPackageFolderInProject
}
