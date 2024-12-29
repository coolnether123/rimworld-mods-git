# PowerShell Script to Copy RimWorld Mods for Git Sharing

# Set paths
$modsConfigPath = "C:\Users\optim\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Config\ModsConfig.xml"
$workshopPath = "D:\SteamLibrary\steamapps\workshop\content\294100"
$destinationPath = "D:\rimworld-mods-git"

# Create destination directory if it doesn't exist
if (-Not (Test-Path -Path $destinationPath)) {
    New-Item -ItemType Directory -Path $destinationPath
}

# Parse ModsConfig.xml to extract mod names
[xml]$modsConfig = Get-Content -Path $modsConfigPath
$modNames = $modsConfig.ModsConfigData.activeMods.li

# Debugging: Print all mod names from ModsConfig.xml
Write-Host "Mod Names from ModsConfig.xml:" $modNames

# Map mod names, package IDs, and authors to numeric IDs
$idMap = @{}
Get-ChildItem -Path $workshopPath -Directory | ForEach-Object {
    $aboutPath = Join-Path -Path $_.FullName -ChildPath "About\About.xml"
    Write-Host "Checking: $aboutPath"
    if (Test-Path -Path $aboutPath) {
        [xml]$aboutXml = Get-Content -Path $aboutPath
        $modName = $aboutXml.ModMetaData.name
        $packageId = $aboutXml.ModMetaData.packageId
        $author = $aboutXml.ModMetaData.author
        Write-Host "Found Mod in About.xml: $modName (Package ID: $packageId, Author: $author) -> $_.Name"
        # Map both name and packageId to numeric ID
        $idMap[$modName] = $_.Name
        $idMap[$packageId] = $_.Name
    } else {
        Write-Host "About.xml not found in: $_.FullName"
    }
}

# Debugging: Print the full ID map
Write-Host "ID Map Contents:" $idMap

# Copy mods based on names, package IDs, and mapped IDs
foreach ($modName in $modNames) {
    Write-Host "Processing Mod: $modName"
    if ($idMap.ContainsKey($modName)) {
        $sourcePath = Join-Path -Path $workshopPath -ChildPath $idMap[$modName]
        $destPath = Join-Path -Path $destinationPath -ChildPath $idMap[$modName]

        # Create mod folder in destination if it doesn't exist
        if (-Not (Test-Path -Path $destPath)) {
            New-Item -ItemType Directory -Path $destPath
        }

        # Copy mod folder to destination
        Write-Host "Copying from: $sourcePath to $destPath"
        Copy-Item -Path "$sourcePath\*" -Destination $destPath -Recurse -Force
        Write-Host "Copied: $modName"
    } else {
        Write-Host "Not Found: $modName"
    }
}

# Copy ModsConfig.xml into the destination folder
Copy-Item -Path $modsConfigPath -Destination "$destinationPath\ModsConfig.xml" -Force
Write-Host "Copied ModsConfig.xml"

Write-Host "All mods copied successfully! Ready for Git upload."
