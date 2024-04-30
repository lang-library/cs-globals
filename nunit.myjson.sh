#! /usr/bin/env bash
set -uvx
set -e
cwd=`pwd`
cd $cwd/MyJson.Test
dotnet test MyJson.Test.csproj
