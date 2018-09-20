# STEP 1 build executable binary

FROM golang:latest as builder
COPY einbuchen .
#WORKDIR $GOPATH/src/mypackage/myapp/

#get dependancies
#you can also use dep

RUN go get -u github.com/golang/dep/cmd/dep && dep ensure
RUN GOARCH=amd64 CGO_enabled=0 go install \
      -installsuffix 'static' \
      -ldflags '-X $(PKG)/pkg/version.VERSION=$(VERSION)' \
      -o /go/bin/einbuchen
#RUN go get -d -v

#build the binary
#RUN go build -o /go/bin/hello

# STEP 2 build a small image

# start from scratch
FROM scratch

# Copy our static executable
COPY --from=builder /go/bin/einbuchen /go/bin/einbuchen
ENTRYPOINT ["/go/bin/einbuchen"]
