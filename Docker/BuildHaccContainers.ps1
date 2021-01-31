$version = '1.1.0'

function Test-ExitCode ([int] $Expected = 0)
{
    if ($LASTEXITCODE -ne 0)
    {
        throw "Exit code from command is invalid, expected $Expected, got $LASTEXITCODE"
    }
}

$sw = [System.Diagnostics.Stopwatch]::StartNew()
Write-Host "Started: $((Get-Date))"

$prgAct = 'Building HACC Docker images...'
$prgStat = 'Building'

# Build Raw Images

Write-Progress -Activity $prgAct -Status $prgStat -PercentComplete 33 -CurrentOperation 'Ubuntu-AMD64'
docker build --build-arg TAG=$version --no-cache -t qjake/hacc:$version-linux-ubuntu-amd64 -f linux-ubuntu-amd64/Dockerfile . ; Test-ExitCode

Write-Progress -Activity $prgAct -Status $prgStat -PercentComplete 66 -CurrentOperation 'Debian-AMD64'
docker build --build-arg TAG=$version --no-cache -t qjake/hacc:$version-linux-debian-amd64 -f linux-debian-amd64/Dockerfile . ; Test-ExitCode

Write-Progress -Activity $prgAct -Status $prgStat -PercentComplete 90 -CurrentOperation 'Alpine-AMD64'
docker build --build-arg TAG=$version --no-cache -t qjake/hacc:$version-linux-alpine-amd64 -f linux-alpine-amd64/Dockerfile . ; Test-ExitCode

Write-Progress -Activity $prgAct -Completed


# Tag Latest Specific

docker tag qjake/hacc:$version-linux-ubuntu-amd64 qjake/hacc:latest-linux-ubuntu-amd64
docker tag qjake/hacc:$version-linux-alpine-amd64 qjake/hacc:latest-linux-alpine-amd64
docker tag qjake/hacc:$version-linux-debian-amd64 qjake/hacc:latest-linux-debian-amd64

# Tag main version and latest

docker tag qjake/hacc:$version-linux-debian-amd64 qjake/hacc:$version
docker tag qjake/hacc:$version qjake/hacc:latest


# Upload them all in order

$prgAct = 'Pushing Docker images...'
$prgStat = 'Pushing'

# The first 5 take awhile, the others are just copies and go faster.
Write-Progress -Activity $prgAct -Status $prgStat -PercentComplete 33 -CurrentOperation 'Alpine-AMD64'
docker push qjake/hacc:$version-linux-alpine-amd64

Write-Progress -Activity $prgAct -Status $prgStat -PercentComplete 66 -CurrentOperation 'Debian-AMD64'
docker push qjake/hacc:$version-linux-debian-amd64

Write-Progress -Activity $prgAct -Status $prgStat -PercentComplete 99 -CurrentOperation 'Ubuntu-AMD64'
docker push qjake/hacc:$version-linux-ubuntu-amd64

Write-Progress -Activity $prgAct -Completed

docker push qjake/hacc:latest-linux-alpine-amd64
docker push qjake/hacc:latest-linux-debian-amd64
docker push qjake/hacc:latest-linux-ubuntu-amd64
docker push qjake/hacc:$version
docker push qjake/hacc:latest

Write-Host "Cleaning up, this may take a moment..."
# Clean up... because you can get a "no space left on disk" error on some containers otherwise
docker image prune --force

Write-Host "Completed: $((Get-Date))"
$sw.Stop()
Write-Host "Took $($sw.Elapsed.Minutes) min(s) $($sw.Elapsed.Seconds) sec(s)."