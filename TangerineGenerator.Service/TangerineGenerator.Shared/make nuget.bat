setlocal

set solution=D:\source\repos\TangerineAuction\TangerineGenerator.Service\TangerineGenerator.Shared\TangerineGenerator.Shared.csproj
set configuration=release
set destination-dir=D:\source\repos\TangerineAuction\Nuget

dotnet clean %solution% --configuration %configuration%
dotnet build %solution% --configuration %configuration%
dotnet pack  %solution% --configuration %configuration% --no-build --include-symbols --output %destination-dir%

endlocal