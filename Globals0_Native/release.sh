#! bash -uvx
set -uvx
set -e
ownerName=`cat ./OWNER.txt`
appName=`cat ./NAME.txt`
cwd=`pwd`
ts=`date "+%Y.%m%d.%H%M.%S"`
version=${ts}
#sed -i -e "s/<Version>.*<\/Version>/<Version>${version}<\/Version>/g" Exe/Exe.csproj
./build.sh
rm -rf ${appName}.v*.zip
cd Main.Exe/bin/Release/net462
7z a -r -tzip "$cwd/${appName}.v$ts.zip" *
cd $cwd
url="https://gitlab.com/${ownerName}/tools/-/raw/main/${appName}.v${version}.zip"
cat << EOS > ${appName}.xml
<?xml version="1.0" encoding="utf-8"?>
<software>
  <version>v${version}</version>
  <url>$url</url>
</software>
EOS
gitlab-console.exe --project ${ownerName}/tools --action upload "${appName}.v${ts}.zip" "${appName}.xml"

cd $cwd/Console
dir=`pwd`
dotnet restore Run.sln -p:Configuration=Release -p:Platform="Any CPU"
msbuild.exe Run.sln -p:Configuration=Release -p:Platform="Any CPU"
cd ./bin/Release/net462/
rm -rf System.IO.Compression.dll
ilmerge -wildcards -out:$dir/${appName}.exe -allowDup ${appName}.exe *.dll
#cp -rp ${appName}.exe $dir/${appName}.exe

cd $cwd/Window
dir=`pwd`
dotnet restore Run.sln -p:Configuration=Release -p:Platform="Any CPU"
msbuild.exe Run.sln -p:Configuration=Release -p:Platform="Any CPU"
cd ./bin/Release/net462/
rm -rf System.IO.Compression.dll
ilmerge -wildcards -out:$dir/${appName}.exe -allowDup ${appName}.exe *.dll
#cp -rp ${appName}.exe $dir/${appName}.exe

path64=${ownerName}.${appName}
digits=`my-random-digits.exe -d 32`
target=~/cmd/${appName}.exe
cp -rp $cwd/Console/${appName}.exe $target
echo [embed:$digits] >> $target
echo $path64 >> $target
echo [/embed:$digits] >> $target
target=~/cmd/${appName}-win.exe
cp -rp $cwd/Window/${appName}.exe $target
echo [embed:$digits] >> $target
echo $path64 >> $target
echo [/embed:$digits] >> $target

cd $cwd
git add .
git commit -m"${ownerName}.${appName}-v$version"
git tag -a "${ownerName}.${appName}-v$version" -m"${ownerName}.${appName}-v$version"
git push origin "${ownerName}.${appName}-v$version"
git push
git remote -v
