#! /usr/bin/env bash
set -uvx
set -e
appName=`cat ./NAME.txt`
cwd=`pwd`
rm -rvf ~/cmd/${appName}
