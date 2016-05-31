cd /d %~dp0
SET OUTPUT=packages
SET ENDPOINT=https://shipwreck.jp/nuget/
SET OPTIONS=-OutputDirectory %OUTPUT% -IncludeReferencedProjects -Build -Properties Configuration=Release
del /q %OUTPUT%
mkdir %OUTPUT%
nuget pack ..\..\src\Shipwreck.ImasCGImages.Models\Shipwreck.ImasCGImages.Models.csproj %OPTIONS%
nuget push %OUTPUT%\*.nupkg -Source %ENDPOINT%