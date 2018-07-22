$key = cat private\NugetApiKey.txt

Publish-Module -Path $PSScriptRoot\..\psmath -NugetApiKey $key -Verbose
