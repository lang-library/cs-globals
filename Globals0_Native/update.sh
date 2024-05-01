#! /usr/bin/env bash
set -uvx
set -e
cwd=`pwd`
#cd $cwd/Main
#./update.sh
cd $cwd/Console
./update.sh
cd $cwd/Window
./update.sh
