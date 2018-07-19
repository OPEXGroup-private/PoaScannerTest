#!/usr/bin/env bash

build_directory="build"
project="PoaScannerTest"

set -e

if [ ! -f "PoaScannerTest.sln" ]; then
    echo "Script must be started from solution root directory"
    exit 1
fi

 if [ ! -x "$(which dotnet)" ] ; then
    echo "dotnet SDK >= 2.1.300 is required"
 fi

if [ ! -d "$build_directory" ]; then
    echo "Creating build directory $build_directory"
    mkdir $build_directory
fi

dotnet publish "src/$project/${project}.csproj" --configuration Release --force --output "../../${build_directory}"

echo "Project built"

dotnet "${build_directory}/${project}.dll" "$@"