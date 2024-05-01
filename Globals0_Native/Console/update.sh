#! /usr/bin/env bash
set -uvx
set -e
#cd common
rm -rf common/*
cp -rp ~/cs/globals/Globals.Slim/Globals.Slim/*.cs ~/cs/globals/Globals.Slim/Globals.Slim/Parser common/
rm -rf common/Extensions.cs common/MyJS.cs common/JintScript.cs common/LiteDBProps.cs
# common/Internal.cs
find common -name "*.cs" -exec sed -i -e "s/Globals/nuget_tools.Globals0_Native/g" {} +
