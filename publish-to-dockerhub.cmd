@echo off
setlocal

REM Get today's date in YYYYMMDD format using PowerShell
for /f %%i in ('powershell -NoProfile -Command "Get-Date -Format yyyyMMdd"') do set DATE_TAG=%%i

cd transactionnotes.AppHost
aspirate generate --output-format compose
docker login --username pngan

docker tag webfrontend pngan/webfrontend:latest
docker push pngan/webfrontend:latest
docker tag webfrontend pngan/webfrontend:%DATE_TAG%
docker push pngan/webfrontend:%DATE_TAG%

docker tag apiservice pngan/apiservice:latest
docker push pngan/apiservice:latest
docker tag apiservice pngan/apiservice:%DATE_TAG%
docker push pngan/apiservice:%DATE_TAG%

docker tag centraldb-migration pngan/centraldb-migration:latest
docker push pngan/centraldb-migration:latest
docker tag centraldb-migration pngan/centraldb-migration:%DATE_TAG%
docker push pngan/centraldb-migration:%DATE_TAG%

endlocal
