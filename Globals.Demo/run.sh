#! /usr/bin/env bash
set -uvx
set -e
cd "$(dirname "$0")"
cwd=`pwd`
ts=`date "+%Y.%m%d.%H%M.%S"`
version="${ts}"

cd $cwd/..
find . -name bin -exec rm -rf {} +
find . -name obj -exec rm -rf {} +

cd $cwd/..
dotnet build -p:Configuration=Release -p:Platform="Any CPU" Globals.sln

cd $cwd/../Globals.Demo
bin/Release/net462/Globals.Demo.exe 0
bin/Release/net462/Globals.Demo.exe 1
bin/Release/net462/Globals.Demo.exe 2
bin/Release/net462/Globals.Demo.exe 3
bin/Release/net462/Globals.Demo.exe 4
