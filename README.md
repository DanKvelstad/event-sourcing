# event-sourcing
docker build RemoteControl -t remote_control
docker build LightBulb -t light_bulb

docker network create --driver bridge smart_home
docker run --rm -d --network=smart_home --name mongo -p 27017:27017 mongo:latest
docker run --rm -d --network=smart_home --name eventstore -p 2113:2113 -p 1113:1113 eventstore/eventstore
docker run --rm -d --network=smart_home --name remote_control remote_control
docker run --rm -d --network=smart_home --name light_bulb light_bulb

docker stop remote_control
docker stop light_bulb