param(
    [string]$rid = 'win-x64'
)

# Publish self-contained single-file release and zip the result
$project = '..\GOGE\GOGE.csproj'
$out = '..\publish'

Write-Host "Publishing for RID $rid to $out..."
dotnet publish $project -c Release -r $rid --self-contained true /p:PublishSingleFile=true -o $out

if ($LASTEXITCODE -ne 0) { Write-Error "Publish failed"; exit 1 }

# Zip the publish folder
$zip = "GOGE_publish_$rid.zip"
if (Test-Path $zip) { Remove-Item $zip }
Compress-Archive -Path "$out\*" -DestinationPath $zip
Write-Host "Created $zip"