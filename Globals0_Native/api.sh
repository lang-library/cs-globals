#! /usr/bin/env bash
set -uvx
set -e
ownerName=`cat ./OWNER.txt`
appName=`cat ./NAME.txt`
buildMode=Release
cwd=`pwd`
#./build.sh
#cd $cwd/Exe/bin/x64/${buildMode}/net462/
#cp -rp ${ownerName}.${appName}.main.dll ~/cmd/
./inst.sh
cd $cwd
pip install cffi
python ./api.py
