# Have to correspond to the setting in build.sbt
IMAGE_VERSION=latest
IMAGE_NAME=containerbootcamp/ausleihen:$(IMAGE_VERSION)

all: image-publish

image: 
	docker build -t $(IMAGE_NAME) .

image-publish: image
	docker push $(IMAGE_NAME)