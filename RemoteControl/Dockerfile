FROM microsoft/dotnet:sdk as build
WORKDIR /app
RUN apt-get update
RUN apt-get install unzip
RUN curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /bin/vsdbg
RUN dotnet new console --output . --name RemoteControl
RUN rm Program.cs
RUN dotnet add package EventStore.ClientAPI.NetCore
RUN dotnet add package Newtonsoft.Json
COPY src src
ADD https://raw.githubusercontent.com/DanKvelstad/event-sourcing/master/EventDefinitions/Events.cs src
COPY *.csproj .
RUN dotnet publish -c Debug -o publish

FROM microsoft/dotnet:runtime as runtime
WORKDIR /app
COPY --from=build /bin/vsdbg/ /bin/vsdbg/
COPY --from=build /app/publish/ .
ENTRYPOINT ["dotnet", "RemoteControl.dll"]