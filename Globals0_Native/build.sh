#! /usr/bin/env bash
set -vx
set -e
ownerName=`cat ./OWNER.txt`
appName=`cat ./NAME.txt`
slnName=${ownerName}.${appName}
buildMode=Release
cwd=`pwd`
find . -name bin -exec rm -rvf {} +
find . -name obj -exec rm -rvf {} +
dotnet restore ${slnName}.sln -p:Configuration=${buildMode} -p:Platform="Any CPU"
msbuild.exe ${slnName}.sln -p:Configuration=${buildMode} -p:Platform="Any CPU"

cd $cwd/Logic.Exe/bin/Release/net462/
#ilmerge -wildcards -internalize:$cwd/exclude.txt -out:$cwd/${ownerName}.${appName}.dll *.dll
ilmerge -wildcards -internalize -out:$cwd/${ownerName}.${appName}.dll \
  ${ownerName}.${appName}.dll \
  ${ownerName}.${appName}.runtime.dll

cd $cwd
dotnet restore Main.sln -p:Configuration=${buildMode} -p:Platform="Any CPU"
msbuild.exe Main.sln -p:Configuration=${buildMode} -p:Platform="Any CPU"

cd $cwd
