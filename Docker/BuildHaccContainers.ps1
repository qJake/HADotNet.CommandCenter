$version = '0.5.0'

docker build --build-arg TAG=$version --no-cache -t qjake/hacc:$version-linux-alpine-amd64 -f linux-alpine-amd64/Dockerfile .
docker build --build-arg TAG=$version --no-cache -t qjake/hacc:$version-linux-debian-amd64 -f linux-debian-amd64/Dockerfile .
docker build --build-arg TAG=$version --no-cache -t qjake/hacc:$version-linux-ubuntu-amd64 -f linux-ubuntu-amd64/Dockerfile .
docker build --build-arg TAG=$version --no-cache -t qjake/hacc:$version-linux-debian-arm32v7 -f linux-debian-arm32v7/Dockerfile .
docker build --build-arg TAG=$version --no-cache -t qjake/hacc:$version-linux-debian-arm64v8 -f linux-debian-arm64v8/Dockerfile .

docker tag qjake/hacc:$version-linux-debian-arm64v8 qjake/hacc-hassio-aarch64:$version
docker tag qjake/hacc:$version-linux-alpine-amd64 qjake/hacc-hassio-amd64:$version
docker tag qjake/hacc:$version-linux-debian-arm32v7 qjake/hacc-hassio-armhf:$version
docker tag qjake/hacc:$version-linux-debian-arm32v7 qjake/hacc-hassio-armv7:$version

docker tag qjake/hacc-hassio-aarch64:$version qjake/hacc-hassio-aarch64:latest
docker tag qjake/hacc-hassio-amd64:$version   qjake/hacc-hassio-amd64:latest
docker tag qjake/hacc-hassio-armhf:$version   qjake/hacc-hassio-armhf:latest
docker tag qjake/hacc-hassio-armv7:$version   qjake/hacc-hassio-armv7:latest

docker tag qjake/hacc:$version-linux-debian-amd64 qjake/hacc:$version
docker tag qjake/hacc:$version qjake/hacc:latest