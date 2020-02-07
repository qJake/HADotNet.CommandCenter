cd ..

if (!(Test-Path 'HADotNet.CommandCenter.sln'))
{
    Write-Host 'Please run this script in the same working directory that the script file exists in.' -Fore Red
    return
}

try
{
    7z i | Out-Null
}
catch
{
    Write-Host 'Please ensure 7z.exe is in your PATH. You should be able to type "7z i" in a console window and get output.' -Fore Red
    return
}

try
{
    dotnet --version | Out-Null
}
catch
{
    Write-Host 'Please ensure you have installed the dotnet SDK and that "dotnet" is in your PATH. Try running "dotnet --version" from a console window.' -Fore Red
    return
}

# Clean folders
Write-Host "Cleaning..."
rm HADotNet.CommandCenter/obj/* -Recurse -Force
rm HADotNet.CommandCenter/bin/* -Recurse -Force

# Publish
Write-Host "Publishing..."
dotnet publish HADotNet.CommandCenter.sln /p:PublishProfile=HADotNet.CommandCenter/Properties/PublishProfiles/debian-10-arm.pubxml -c Release

 # Package
Write-Host 'Packaging .tar.gz release...'
cd "HADotNet.CommandCenter\bin\Release\netcoreapp3.1\debian.10-arm\publish\"
7z a -ttar HADotNet.CommandCenter-armhf.tar *
7z a -tgzip HADotNet.CommandCenter-armhf.tar.gz HADotNet.CommandCenter-armhf.tar
Move-Item HADotNet.CommandCenter-armhf.tar.gz ../../../../../../
cd ../../../../../../Docker

Write-Host 'Done!'