FROM microsoft/dotnet:sdk as build
WORKDIR /app
RUN dotnet new console --output . --name Lamp
RUN rm Program.cs
RUN dotnet add package EventStore.ClientAPI.NetCore
RUN dotnet add package Newtonsoft.Json
COPY src src
ADD https://raw.githubusercontent.com/DanKvelstad/event-sourcing/master/EventDefinitions/Events.cs src
COPY Lamp.csproj .
RUN dotnet publish -c Release -o publish

FROM microsoft/dotnet:runtime as runtime
WORKDIR /app
COPY --from=build /app/publish/ .
ENTRYPOINT ["dotnet", "Lamp.dll"]