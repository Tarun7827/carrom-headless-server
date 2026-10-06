FROM ubuntu:22.04
WORKDIR /app
# Place the Linux headless Unity build in ./build during CI/CD.
COPY build/ /app/build/
EXPOSE 8080
ENTRYPOINT ["/app/build/CarromHeadlessServer.x86_64", "-batchmode", "-nographics", "-logFile", "/app/server.log"]
