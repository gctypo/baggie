#!/bin/sh

echo -e "\n### RESTORING NUGET PACKAGES ###"
dotnet restore

echo -e "\n### BUILDING RELEASE BUILD ###"
dotnet build --configuration Release

echo -e "\n### RUNNING TESTS ###"
dotnet test

echo -e "\n### DONE ###"
