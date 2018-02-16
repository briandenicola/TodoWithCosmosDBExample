FROM microsoft/dotnet:2.0-runtime
RUN mkdir -p /usr/src/app
WORKDIR /usr/src/app
COPY publish /usr/src/app
EXPOSE 5000/tcp
ENTRYPOINT dotnet /usr/src/app/todo.core.dll
