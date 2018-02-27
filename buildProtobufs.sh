#!/bin/bash
PROJECT_DIR=$PWD
REPO_DIR=$(readlink -m "../")
sharedProtos="$REPO_DIR/protobufs"
echo $sharedProtos
#projectProtos="$PROJECT_DIR/protobufs"
DIST_DIR="$PROJECT_DIR/models/Proto"
cd ~/.nuget/packages/grpc.tools/1.9.0/tools/linux_x64/
#/home/ivan/Apps/grpc/Grpc.Tools.1.4.1/tools/linux_x64/
protoc \
    --proto_path=$sharedProtos \
    --csharp_out=$DIST_DIR \
    --plugin=protoc-gen-grpc=grpc_csharp_plugin \
    --grpc_out=$DIST_DIR \
    $sharedProtos/DemoTypes.proto \
    $sharedProtos/DemoManagment.proto
