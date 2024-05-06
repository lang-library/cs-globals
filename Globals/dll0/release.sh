#! /usr/bin/env bash
set -uvx
set -e
cd "$(dirname "$0")"
cwd=`pwd`

#pro64 -f -s dll0.pro
pro64 -s dll0.pro
dep ./dll0.64.static/release/dll0.dll
cp -rp ./dll0.64.static/release/dll0.dll ../dll0-x64.dll

#pro32 -f -s dll0.pro
pro32 -s dll0.pro
dep ./dll0.32.static/release/dll0.dll
cp -rp ./dll0.32.static/release/dll0.dll ../dll0-x32.dll
