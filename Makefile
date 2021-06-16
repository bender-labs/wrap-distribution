# arch can be osx-arm64 osx-x64 linux-x64 win-x64

PUBLISH_DIR=publish

build: target=Debug
build: 
	dotnet build -c ${target}

publish: arch=osx-x64
publish:
	@mkdir -p ${PUBLISH_DIR}
	@dotnet publish src/Distribution.Console --no-restore -o ${PUBLISH_DIR} -r ${arch} -p:PublishSingleFile=true -c Release --self-contained true
	@mv ${PUBLISH_DIR}/Distribution.Console ${PUBLISH_DIR}/governance

.PHONY: build publish test