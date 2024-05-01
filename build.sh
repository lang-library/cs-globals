#! /usr/bin/env bash
set -vx
set -e
#ownerName=`cat ./OWNER.txt`
#appName=`cat ./NAME.txt`
slnName=ClassLibrary1
buildMode=Release
cwd=`pwd`
cd $cwd/$slnName
find . -name bin -exec rm -rvf {} +
find . -name obj -exec rm -rvf {} +
find . -name packages -exec rm -rvf {} +
dotnet restore ${slnName}.sln -p:Configuration=${buildMode} -p:Platform="Any CPU"
dotnet test ${slnName}.sln -p:Configuration=${buildMode} -p:Platform="Any CPU"
#msbuild.exe ${slnName}.sln -p:Configuration=${buildMode} -p:Platform="Any CPU"

cd $cwd
