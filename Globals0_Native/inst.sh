#! /usr/bin/env bash
set -uvx
set -e
ownerName=`cat ./OWNER.txt`
appName=`cat ./NAME.txt`
buildMode=Release
cwd=`pwd`
./build.sh
cd $cwd/Main.Exe/bin/${buildMode}/net462/
rm -rf ~/cmd/${ownerName}.${appName}
mkdir -p ~/cmd/${ownerName}.${appName}
cp -rp * ~/cmd/${ownerName}.${appName}
