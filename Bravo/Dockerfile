FROM microsoft/dotnet:sdk as build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o publish

FROM microsoft/dotnet:runtime as runtime
WORKDIR /app
COPY --from=build /app/publish/ .
#RUN dotnet add package EventStore.ClientAPI.NetCore
ENTRYPOINT ["dotnet", "Bravo.dll"]