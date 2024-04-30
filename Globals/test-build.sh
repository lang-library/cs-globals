#! /usr/bin/env bash
set -uvx
set -e
cwd=`pwd`
ts=`date "+%Y.%m%d.%H%M.%S"`
version="${ts}"

cd $cwd/Globals
#sed -i -e "s/<Version>.*<\/Version>/<Version>${version}<\/Version>/g" Globals.csproj
#cp -r $cwd/../Globals.Slim/Globals.Slim/*.cs $cwd/../Globals.Slim/Globals.Slim/Parser .
rm -rf obj bin
rm -rf *.nupkg
dotnet pack -o . -p:Configuration=Release -p:Platform="Any CPU" Globals.csproj

exit 0

tag="Globals-v$version"
cd $cwd
git add .
git commit -m"$tag"
git tag -a "$tag" -m"$tag"
git push origin "$tag"
git push
git remote -v
