# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY ImageStubber/*.csproj ./ImageStubber/
COPY ImageStubber.Tests/*.csproj ./ImageStubber.Tests/
RUN dotnet restore -v d

# copy everything else and build app
COPY ImageStubber/. ./ImageStubber/
COPY ImageStubber.Tests/. ./ImageStubber.Tests/
WORKDIR /source/ImageStubber
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal-amd64

RUN apt-get update && apt-get install -y apt-utils libgdiplus libc6-dev
WORKDIR /app
COPY --from=build /app ./

COPY ImageStubber/font/JetBrainsMonoNL-Medium.ttf /usr/share/fonts/truetype/JetBrainsMonoNL/

ENTRYPOINT ["dotnet", "ImageStubber.dll"]