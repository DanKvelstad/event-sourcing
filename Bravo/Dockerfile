FROM microsoft/dotnet:sdk as build
WORKDIR C:\\Bravo
COPY . .
RUN dotnet publish -c Release -o Publish

FROM microsoft/dotnet:runtime as runtime
WORKDIR C:\\Bravo
COPY --from=build C:\\Bravo\\Publish\\ .
ENTRYPOINT ["dotnet", "Bravo.dll"]