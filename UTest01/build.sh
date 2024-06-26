#! /usr/bin/env bash
set -vx
set -e

cd "$(dirname "$0")"

slnName=Test01
buildMode=Release
cwd=`pwd`

cd $cwd
find . -name bin -exec rm -rvf {} +
find . -name obj -exec rm -rvf {} +
find . -name packages -exec rm -rvf {} +
dotnet restore ${slnName}.sln -p:Configuration=${buildMode} -p:Platform="Any CPU"
msbuild.exe ${slnName}.sln -p:Configuration=${buildMode} -p:Platform="Any CPU"
vstest.console.exe "XUnit1\bin\Release\net462\XUnit1.dll"

cd $cwd
