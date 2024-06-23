ARG VERSION=0.1.0-local

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG VERSION
WORKDIR /app

COPY src/*.sln ./
COPY src/OSK.UML.CommandLine/*.csproj ./OSK.UML.CommandLine/
COPY src/OSK.UML/*.csproj ./OSK.UML/
COPY src/OSK.UML.Framework/*.csproj ./OSK.UML.Framework/
COPY src/OSK.UML.Exporters.PlantUML/*.csproj ./OSK.UML.Exporters.PlantUML/

RUN ls

RUN dotnet restore ./OSK.UML.sln

COPY ./src ./
RUN dotnet pack -c Release -p:VERSION=${VERSION} ./OSK.UML.CommandLine/OSK.UML.CommandLine.csproj

FROM mcr.microsoft.com/dotnet/sdk:8.0 as packer
ARG VERSION
WORKDIR /app

RUN groupadd -g 1000 -r umlgenerator && useradd --no-log-init -u 1000 -r -g umlgenerator umlgenerator && \
    mkdir -p /home/umlgenerator && \
    mkdir -p /home/umlgenerator/plantuml && \
    chown umlgenerator:umlgenerator /home/umlgenerator
ENV HOME=/home/umlgenerator PATH=/home/umlgenerator/.dotnet/tools:${PATH}
USER umlgenerator

COPY --from=build /app/OSK.UML.CommandLine/bin/Release/OSK.UML.CommandLine.*.nupkg ./
RUN dotnet tool install --global --add-source /app --version ${VERSION} OSK.UML.CommandLine

ENV PATH="/root/.dotnet/tools:${PATH}"

FROM mcr.microsoft.com/dotnet/runtime:8.0
RUN apt-get update && apt-get install -y \
 graphviz

ENV JAVA_HOME /usr/lib/jvm/msopenjdk-17-amd64
ENV PATH "${JAVA_HOME}/bin:${PATH}"
COPY --from=mcr.microsoft.com/openjdk/jdk:17-ubuntu $JAVA_HOME $JAVA_HOME

COPY --from=packer /home/umlgenerator/.dotnet/tools /opt/bin

ENV PATH="/opt/bin:${PATH}"