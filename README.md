# event-sourcing
docker build Dimmer -t dimmer
docker build Lamp -t lamp

docker network create --driver bridge smart_home
docker run -d --network=smart_home --name eventstore -p 2113:2113 -p 1113:1113 eventstore/eventstore
docker run -d --network=smart_home --name dimmer dimmer
docker run -d --network=smart_home --name lamp lamp