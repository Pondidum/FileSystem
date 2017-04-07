#! /bin/sh

# First parameter is build mode, defaults to Debug

MODE=${1:-Debug}
NAME="FileSystem"

dotnet restore
dotnet build --configuration $MODE
(cd ./src/FileSystem.Tests/ && dotnet xunit)

dotnet pack ./src/$NAME --configuration $MODE --output ../../.build
