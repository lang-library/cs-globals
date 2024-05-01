#! /usr/bin/env bash
set -vx
set -e
#ownerName=`cat ./OWNER.txt`
#appName=`cat ./NAME.txt`
slnName=Globals0
buildMode=Release
cwd=`pwd`
find . -name bin -exec rm -rvf {} +
find . -name obj -exec rm -rvf {} +
dotnet restore ${slnName}.sln -p:Configuration=${buildMode} -p:Platform="Any CPU"
msbuild.exe ${slnName}.sln -p:Configuration=${buildMode} -p:Platform="Any CPU"

cd $cwd
