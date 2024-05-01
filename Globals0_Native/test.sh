#! /usr/bin/env bash
set -vx
set -e
ownerName=`cat ./OWNER.txt`
appName=`cat ./NAME.txt`
buildMode=Release
cwd=`pwd`
cd $cwd
runlib-p "$cwd/Main.Exe/bin/${buildMode}/net462/${ownerName}.${appName}.native.dll" "$@"
