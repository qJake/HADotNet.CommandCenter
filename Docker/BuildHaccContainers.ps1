$version = '0.5.0'

cd W:\GitHub\HADotNet.CommandCenter\Docker\

docker build --no-cache -t qjake/hacc:$version-linux-alpine-amd64 -f linux-alpine-amd64/Dockerfile .
docker build --no-cache -t qjake/hacc:$version-linux-debian-amd64 -f linux-debian-amd64/Dockerfile .
docker build --no-cache -t qjake/hacc:$version-linux-ubuntu-amd64 -f linux-ubuntu-amd64/Dockerfile .
docker build --no-cache -t qjake/hacc:$version-linux-alpine-arm64v8 -f linux-alpine-arm64v8/Dockerfile .

docker tag qjake/hacc:$version-linux-debian-amd64 qjake/hacc:latest