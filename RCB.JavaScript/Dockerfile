#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal AS base
RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -sL https://deb.nodesource.com/setup_12.x | bash \
    && apt-get install nodejs -yq
WORKDIR /app
# EXPOSE 80
# EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -sL https://deb.nodesource.com/setup_12.x | bash \
    && apt-get install nodejs -yq
WORKDIR /src
COPY RCB.JavaScript.csproj RCB.JavaScript/
RUN dotnet restore "RCB.JavaScript/RCB.JavaScript.csproj"
COPY . RCB.JavaScript/
WORKDIR "/src/RCB.JavaScript"
RUN dotnet build "RCB.JavaScript.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RCB.JavaScript.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# COPY --from=build "/src/RCB.JavaScript/ExtraFiles" ./ExtraFiles
# ENTRYPOINT ["dotnet", "RCB.JavaScript.dll"]
CMD ASPNETCORE_URLS=http://*:$PORT dotnet RCB.JavaScript.dll