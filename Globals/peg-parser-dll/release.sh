#! /usr/bin/env bash
set -uvx
set -e
cd "$(dirname "$0")"
cwd=`pwd`
swig.exe -csharp -c++ -cppext cpp PegParser.i
#g++ -o PegParserDLL.dll -shared -static -std=c++17 -I ~/common/include *.cpp
pro64 -s -f PegParserDLL.pro
pro32 -s -f PegParserDLL.pro
#cp PegParserDLL.dll $cwd/..
rm -rf x64 x32
mkdir -p x64
cp PegParserDLL.64.static/release/PegParserDLL.dll x64/
mkdir -p x32
cp PegParserDLL.32.static/release/PegParserDLL.dll x32/
rm -rf $cwd/../PegParserDLL.zip
7z a -tzip $cwd/../PegParserDLL.zip x64 x32
cd $cwd/..
rm -rf PegParser
mkdir -p PegParser
mv $cwd/*.cs PegParser/
