#! /usr/bin/env bash
set -uvx
set -e
cd "$(dirname "$0")"
cwd=`pwd`
swig.exe -csharp -c++ -cppext cpp NLJsonParser.i
#g++ -o NLJsonParserDLL.dll -shared -static -std=c++17 -I ~/common/include *.cpp
pro64 -s -f NLJsonParserDLL.pro
pro32 -s -f NLJsonParserDLL.pro
#cp NLJsonParserDLL.dll $cwd/..
rm -rf x64 x32
mkdir -p x64
cp NLJsonParserDLL.64.static/release/NLJsonParserDLL.dll x64/
mkdir -p x32
cp NLJsonParserDLL.32.static/release/NLJsonParserDLL.dll x32/
rm -rf $cwd/../NLJsonParserDLL.zip
7z a -tzip $cwd/../NLJsonParserDLL.zip x64 x32
cd $cwd/..
rm -rf NLJsonParser
mkdir -p NLJsonParser
mv $cwd/*.cs NLJsonParser/
