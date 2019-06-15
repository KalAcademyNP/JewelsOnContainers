FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["JewelsOnContainers/JewelsOnContainers.csproj", "JewelsOnContainers/"]
RUN dotnet restore "JewelsOnContainers/JewelsOnContainers.csproj"
COPY . .
WORKDIR "/src/JewelsOnContainers"
RUN dotnet build "JewelsOnContainers.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "JewelsOnContainers.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "JewelsOnContainers.dll"]