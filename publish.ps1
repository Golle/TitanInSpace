
&dotnet publish src/Space -c Release -o release/ -r win-x64 --self-contained true -p:PublishTrimmed=True -p:PublishAot=True -p:DebugType=None -p:DebugSymbols=false -p:ConsoleWindow=true -p:ConsoleLogging=true -p:Trace=false

if($LASTEXITCODE -ne 0){
    Write-Host "Failed to publish the project with code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Copy-Item assets/bin/manifest.titanpak release/ -Force
Copy-Item ../TitanEngine/assets/bin/builtin.titanpak release/ -Force