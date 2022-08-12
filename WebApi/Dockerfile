#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
#WORKDIR /app
#EXPOSE 35000
#EXPOSE 35001
#
#FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
#WORKDIR /src
#COPY ./ ./
#ENTRYPOINT ["dotnet", "WebApi.dll"]

FROM mcr.microsoft.com/dotnet/runtime-deps:6.0-alpine AS base
LABEL Maintainer="WebApi"
 
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build-env
 
WORKDIR /app
COPY ./publish ./
RUN dotnet publish ./HTTPie/HTTPie.csproj -c Release --self-contained --use-current-runtime -p:AssemblyName=http -p:PublishSingleFile=true -p:PublishTrimmed=true -p:EnableCompressionInSingleFile=true -o ./artifacts
 
FROM base AS final
COPY --from=build-env /app/artifacts/http /root/.dotnet/tools/http
ENV PATH="/root/.dotnet/tools:${PATH}"