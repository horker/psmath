task . Build, ImportDebug

Set-StrictMode -Version 4

############################################################

$SOURCE_PATH = "$PSScriptRoot\source\Horker.DataAnalysis"
$SCRIPT_PATH = "$PSScriptRoot\scripts"

$MODULE_PATH = "$PSScriptRoot\HorkerDataAnalysis"
$MODULE_PATH_DEBUG = "$PSScriptRoot\debug\HorkerDataAnalysis"

$SOLUTION_FILE = "$PSScriptRoot\source\Horker.DataAnalysis.sln"

$OBJECT_FILES = @(
  "Accord.dll"
  "Accord.Math.Core.dll"
  "Accord.Math.dll"
  "Accord.Statistics.dll"
  "Horker.DataAnalysis.dll"
  "Horker.DataAnalysis.pdb"
)

#$HELP_INPUT =  "$SOURCE_PATH\bin\Release\Horker.DataAnalysis.dll"
#$HELP_INTERM = "$SOURCE_PATH\bin\Release\Horker.Data.dll-Help.xml"
#$HELP_OUTPUT = "$MODULE_PATH\Horker.Data.dll-Help.xml"
#$HELPGEN = "$PSScriptRoot\vendor\XmlDoc2CmdletDoc.0.2.10\tools\XmlDoc2CmdletDoc.exe"

############################################################

function New-Folder2 {
  param(
    [string]$Path
  )

  try {
    $null = New-Item -Type Directory $Path -EA Stop
    Write-Host -ForegroundColor DarkCyan "$Path created"
  }
  catch {
    Write-Host -ForegroundColor DarkYellow $_
  }
}

function Copy-Item2 {
  param(
    [string]$Source,
    [string]$Dest
  )

  try {
    Copy-Item $Source $Dest -EA Stop
    Write-Host -ForegroundColor DarkCyan "Copy from $Source to $Dest done"
  }
  catch {
    Write-Host -ForegroundColor DarkYellow $_
  }
}

function Remove-Item2 {
  param(
    [string]$Path
  )

  try {
    Remove-Item $Path -EA Stop
    Write-Host -ForegroundColor DarkCyan "$Path removed"
  }
  catch {
    Write-Host -ForegroundColor DarkYellow $_
  }
}

############################################################

#task Compile {
#  msbuild $SOLUTION_FILE /p:Configuration=Debug /nologo /v:quiet
#  msbuild $SOLUTION_FILE /p:Configuration=Release /nologo /v:quiet
#}

task Build {
  . {
    $ErrorActionPreference = "Continue"

    function Copy-ObjectFiles {
      param(
        [string]$targetPath,
        [string]$objectPath
      )
      New-Folder2 $targetPath

      Copy-Item2 "$SCRIPT_PATH\*" $targetPath
      $OBJECT_FILES | foreach {
        $path = Join-Path $objectPath $_
        Copy-Item2 $path $targetPath
      }
    }

    Copy-ObjectFiles $MODULE_PATH "$SOURCE_PATH\bin\Release"
    Copy-ObjectFiles $MODULE_PATH_DEBUG "$SOURCE_PATH\bin\Debug"
  }
}

#task BuildHelp `
#  -Inputs $HELP_INPUT `
#  -Outputs $HELP_OUTPUT `
#{
#  . $HELPGEN $HELP_INPUT
#
#  Copy-Item $HELP_INTERM $MODULE_PATH
#}

task Test Build, ImportDebug, {
  Invoke-Pester "$PSScriptRoot\tests"
}

task ImportDebug {
  Import-Module $MODULE_PATH_DEBUG -Force
}

task Clean {
  Remove-Item2 "$MODULE_PATH\*" -Force -Recurse -EA Continue
  Remove-Item2 "$MODULE_PATH_DEBUG\*" -Force -Recurse -EA Continue
}
