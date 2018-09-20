# STEP 1 build executable binary

#FROM golang:1.9 as builder
#WORKDIR /go/src/github.com/container-bootcamp/einbuchen
#COPY . .

#ENV PKG github.com/container-bootcamp/einbuchen
#ENV VERSION "latest"

#RUN go get -u github.com/golang/dep/cmd/dep
#RUN dep ensure
#RUN GOARCH=amd64 CGO_enabled=0 go install \
#      -installsuffix 'static' \
#      -ldflags '-X $(PKG)/pkg/version.VERSION=$(VERSION)' \
#      ./...

#FROM scratch
#COPY --from=builder /go/bin/einbuchen .
#ADD resources/ /resources
#ENTRYPOINT ["./einbuchen"]

FROM golang:1.9 AS build

ENV VERSION latest
RUN go get -u github.com/golang/dep/cmd/dep
COPY . /go/src/github.com/container-bootcamp/einbuchen/
WORKDIR /go/src/github.com/container-bootcamp/einbuchen/
RUN dep ensure
RUN	GOARCH=amd64 CGO_ENABLED=0 go install \
    -installsuffix 'static' \
    -ldflags '-X $(PKG)/pkg/version.VERSION=$(VERSION)' \
    ./...

FROM scratch
WORKDIR /
COPY --from=build /go/bin/einbuchen /einbuchen
ADD resources/ /resources/
ENTRYPOINT ["/einbuchen"]
