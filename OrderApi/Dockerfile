FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app
EXPOSE 80

COPY . .
RUN dotnet restore OrderApi.csproj

FROM build AS publish
RUN dotnet publish "OrderApi.csproj" -c Release -o /app

WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "OrderApi.dll"]