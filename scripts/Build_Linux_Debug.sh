#!/bin/bash
# Check if dotnet is installed
if ! command -v dotnet &> /dev/null
then
    echo "dotnet SDK could not be found. Please install .NET 6.0 SDK."
    exit 1
fi

# Get the directory of the script
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

# Navigate to project root (assuming script is in scripts/ folder)
cd "$DIR/.."

# Publish for Linux x64
echo "Building and publishing HermesProxy for Linux x64..."
dotnet publish HermesProxy/HermesProxy.csproj -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true -p:DebugType=embedded -p:DebugSymbols=true

if [ $? -eq 0 ]; then
    echo "--------------------------------------------------------"
    echo "Build SUCCESSFUL!"
    echo "The executable and config can be found in:"
    echo "HermesProxy/bin/Release/linux-x64/publish/"
    echo "--------------------------------------------------------"
else
    echo "Build FAILED!"
fi
