#! /bin/sh

find ./src -iname "*.Tests.csproj" -type f -exec dotnet test "{}" \;
