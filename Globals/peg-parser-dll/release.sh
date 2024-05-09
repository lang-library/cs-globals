#! /usr/bin/env bash
set -uvx
set -e
cd "$(dirname "$0")"
cwd=`pwd`
swig.exe -csharp -c++ -cppext cpp PegParser.i
g++ -o PegParserDLL.dll -shared -static -std=c++17 -I ~/common/include *.cpp
cp PegParserDLL.dll $cwd/..
cd $cwd/..
rm -rf PegParser.zip
7z a -tzip PegParserDLL.zip PegParserDLL.dll
rm -rf PegParser
mkdir -p PegParser
cp $cwd/*.cs PegParser/
