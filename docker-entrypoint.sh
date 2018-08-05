#!/bin/bash
# replace config with env variable
sed -i "s/127.0.0.1/$MYSQL_SERVICE_HOST/" /app/OperationTridentBackEnd/bin/Release/OperationTridentBackEnd.exe.config 
sed -i "s/3306/$MYSQL_SERVICE_PORT/" /app/OperationTridentBackEnd/bin/Release/OperationTridentBackEnd.exe.config

exec "$@"
