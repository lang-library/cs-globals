set -uvx
set -e
ownerName=`cat ./OWNER.txt`
appName=`cat ./NAME.txt`
slnName=${ownerName}.${appName}
buildMode=Release
cwd=`pwd`

dotnet new sln --name ${slnName} --force
dotnet sln ${slnName}.sln add ./Logic.Exe/Logic.Exe.csproj
dotnet sln ${slnName}.sln add ./Logic/Logic.csproj

dotnet new sln --name Main --force
dotnet sln Main.sln add ./Main.Exe/Main.Exe.csproj
dotnet sln Main.sln add ./Main/Main.csproj

ilmerge -out:${ownerName}.${appName}.runtime.dll ~/cmd/slim.runtime.dll
#ilmerge -out:${ownerName}.${appName}.runtime.dll ~/cmd/fat.runtime.dll
