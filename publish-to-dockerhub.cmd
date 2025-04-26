cd transactionnotes.AppHost
aspirate generate --output-format compose
docker login --username pngan
docker tag webfrontend pngan/webfrontend:latest
docker push pngan/webfrontend:latest
docker tag apiservice pngan/apiservice:latest
docker push pngan/apiservice:latest
docker tag centraldb-migration pngan/centraldb-migration:latest
docker push pngan/centraldb-migration:latest