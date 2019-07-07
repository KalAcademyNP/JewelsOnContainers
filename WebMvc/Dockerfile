FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app
EXPOSE 80

COPY . .
RUN dotnet restore WebMvc.csproj

FROM build AS publish
RUN dotnet publish "WebMvc.csproj" -c Release -o /app

WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "WebMvc.dll"]