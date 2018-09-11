# event-sourcing
docker build Alpha -t alpha
docker build Bravo -t bravo

docker run alpha
docker run bravo

docker run -d --name eventstore-node -it -p 2113:2113 -p 1113:1113 eventstore/eventstore

