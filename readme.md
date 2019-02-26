# Pony event generator

## Running Event Store in docker

`docker run --name eventstore -d -p 2113:2113 -p 1113:1113 -e EVENTSTORE_RUN_PROJECTIONS=all -e EVENTSTORE_START_STANDARD_PROJECTIONS=true eventstore/eventstore`

## Generating events

### Generate 10,000 events (default)

`dotnet .\Pony.Generator.Console.dll`

### Generate a specific number of events

`dotnet .\Pony.Generator.Console.dll -n <number>`