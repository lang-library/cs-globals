#! /usr/bin/env bash
set -uvx
set -e
cd "$(dirname "$0")"
cwd=`pwd`
dotnet test -p:Configuration=Release -p:Platform="Any CPU" MyJson.sln
