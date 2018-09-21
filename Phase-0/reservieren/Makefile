# Makefile to build reservieren scs
IMAGE_VERSION=latest
IMAGE_NAME=containerbootcamp/reservieren:$(IMAGE_VERSION)

all: image-publish

image:
	docker build -t $(IMAGE_NAME) .
    
image-publish: image
	docker push $(IMAGE_NAME)
