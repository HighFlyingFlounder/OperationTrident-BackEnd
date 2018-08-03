FROM mono
MAINTAINER w2w
RUN mkdir /app
COPY . /app

RUN msbuild /app/OperationTridentBackEnd.sln /t:Build /p:Configuration=Release

CMD ["mono", "/app/OperationTridentBackEnd/bin/Release/OperationTridentBackEnd.exe"]

EXPOSE 8000