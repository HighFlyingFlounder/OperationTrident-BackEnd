FROM mono
MAINTAINER w2w
RUN mkdir /app
COPY . /app

RUN msbuild /app/OperationTridentBackEnd.sln /t:Build /p:Configuration=Release

COPY docker-entrypoint.sh /usr/local/bin/
RUN ln -s /usr/local/bin/docker-entrypoint.sh /entrypoint.sh # backwards compat
ENTRYPOINT ["docker-entrypoint.sh"]
EXPOSE 8000
# CMD ["/bin/bash", "-c", "\"while true;do echo hello docker; sleep 1; done\""]
# CMD ["/bin/echo", "this is a echo test"]
CMD ["mono", "/app/OperationTridentBackEnd/bin/Release/OperationTridentBackEnd.exe"]

