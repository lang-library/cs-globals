#! /usr/bin/env bash
set -uvx
set -e
cwd=`pwd`
cd $cwd/Globals.Slim
rm -rf Parser
java -jar ./antlr-4.13.1-complete.jar JSON5.g4 -Dlanguage=CSharp -package Globals.Parser.Json5 -o Parser/Json5
