#!/bin/sh

echo "### RESTORING NUGET PACKAGES ###"
dotnet restore

echo "### BUILDING RELEASE BUILD ###"
dotnet build --configuration Release
