# event-sourcing
docker build RemoteControl -t remote_control
docker build LightBulb -t light_bulb

docker network create --driver bridge smart_home
docker run -d --network=smart_home --name eventstore -p 2113:2113 -p 1113:1113 eventstore/eventstore
docker run -d --network=smart_home --name remote_control remote_control
docker run -d --network=smart_home --name light_bulb light_bulb