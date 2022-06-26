#!/bin/bash

# usage: 
# ./build-push.sh registry/repo/image-name:tag

set -e                  # fails if any command fails

podman build -t $1 .    # build and tag image
podman push $1          # push image to registry

