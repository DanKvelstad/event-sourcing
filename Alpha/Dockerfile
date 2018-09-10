FROM microsoft/dotnet:sdk as build
WORKDIR C:\\Alpha
COPY . .
RUN dotnet publish -c Release -o Publish

FROM microsoft/dotnet:runtime as runtime
WORKDIR C:\\Alpha
COPY --from=build C:\\Alpha\\Publish\\ .
ENTRYPOINT ["dotnet", "Alpha.dll"]