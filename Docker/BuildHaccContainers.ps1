$version = '0.5.0'

function Test-ExitCode ([int] $Expected = 0)
{
    if ($LASTEXITCODE -ne 0)
    {
        throw "Exit code from command is invalid, expected $Expected, got $LASTEXITCODE"
    }
}

# Build Raw Images

docker build --build-arg TAG=$version --no-cache -t qjake/hacc:$version-linux-alpine-amd64 -f linux-alpine-amd64/Dockerfile . ; Test-ExitCode
docker build --build-arg TAG=$version --no-cache -t qjake/hacc:$version-linux-debian-amd64 -f linux-debian-amd64/Dockerfile . ; Test-ExitCode
docker build --build-arg TAG=$version --no-cache -t qjake/hacc:$version-linux-ubuntu-amd64 -f linux-ubuntu-amd64/Dockerfile . ; Test-ExitCode
docker build --build-arg TAG=$version --no-cache -t qjake/hacc:$version-linux-debian-arm32v7 -f linux-debian-arm32v7/Dockerfile . ; Test-ExitCode
docker build --build-arg TAG=$version --no-cache -t qjake/hacc:$version-linux-debian-arm64v8 -f linux-debian-arm64v8/Dockerfile . ; Test-ExitCode

# Re-tag Hassio Architecture Tags

docker tag qjake/hacc:$version-linux-debian-arm64v8 qjake/hacc-hassio-aarch64:$version
docker tag qjake/hacc:$version-linux-alpine-amd64 qjake/hacc-hassio-amd64:$version
docker tag qjake/hacc:$version-linux-debian-arm32v7 qjake/hacc-hassio-armhf:$version
docker tag qjake/hacc:$version-linux-debian-arm32v7 qjake/hacc-hassio-armv7:$version

# Tag Hassio Latest

docker tag qjake/hacc-hassio-aarch64:$version qjake/hacc-hassio-aarch64:latest
docker tag qjake/hacc-hassio-amd64:$version   qjake/hacc-hassio-amd64:latest
docker tag qjake/hacc-hassio-armhf:$version   qjake/hacc-hassio-armhf:latest
docker tag qjake/hacc-hassio-armv7:$version   qjake/hacc-hassio-armv7:latest

# Tag main version and latest

docker tag qjake/hacc:$version-linux-debian-amd64 qjake/hacc:$version
docker tag qjake/hacc:$version qjake/hacc:latest

# Upload them all in order

docker push qjake/hacc:$version-linux-alpine-amd64
docker push qjake/hacc:$version-linux-debian-amd64
docker push qjake/hacc:$version-linux-ubuntu-amd64
docker push qjake/hacc:$version-linux-debian-arm32v7
docker push qjake/hacc:$version-linux-debian-arm64v8
docker push qjake/hacc-hassio-aarch64:$version
docker push qjake/hacc-hassio-amd64:$version
docker push qjake/hacc-hassio-armhf:$version
docker push qjake/hacc-hassio-armv7:$version
docker push qjake/hacc-hassio-aarch64:latest
docker push qjake/hacc-hassio-amd64:latest
docker push qjake/hacc-hassio-armhf:latest
docker push qjake/hacc-hassio-armv7:latest
docker push qjake/hacc:$version
docker push qjake/hacc:latest