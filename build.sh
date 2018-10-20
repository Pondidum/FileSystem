#! /bin/sh

set -e  # stop on errors

MODE=${1:-Debug}  # First parameter is build mode, defaults to Debug
NAME=$(basename $(ls *.sln | head -n 1) .sln) # Find the solution file

dotnet build \
  --configuration $MODE

# appveyor has find pointing to /c/windows/system32/find for some reason
/usr/bin/find ./src -iname "*.Tests.csproj" | xargs -L1 dotnet test \
  --no-restore \
  --no-build \
  --configuration $MODE

dotnet pack \
  --no-restore \
  --no-build \
  --configuration $MODE \
  --output ../../.build
