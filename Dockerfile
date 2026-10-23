# Twilio Simulator container image (deployed to DigitalOcean App Platform as simulator-sms.colbek.app).
#
#   docker build -t twilio-simulator .
#   docker run --rm -p 8080:8080 twilio-simulator      → http://localhost:8080
#
# Data is in-memory: every restart or deploy starts empty.

ARG DOTNET_VERSION=10.0

# ---- css: compile TailwindCSS ------------------------------------------------------------
FROM node:22-alpine AS css
WORKDIR /src
COPY src/TwilioSimulator/package.json src/TwilioSimulator/package-lock.json ./
RUN npm ci
COPY src/TwilioSimulator/ ./
RUN npm run css:build

# ---- build -------------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /src
COPY src/TwilioSimulator/TwilioSimulator.csproj TwilioSimulator/
RUN dotnet restore TwilioSimulator/TwilioSimulator.csproj

COPY src/TwilioSimulator/ TwilioSimulator/
COPY --from=css /src/wwwroot/css/output.css TwilioSimulator/wwwroot/css/output.css
RUN dotnet publish TwilioSimulator/TwilioSimulator.csproj \
      --configuration Release \
      --no-restore \
      --output /app/publish \
      /p:UseAppHost=false

# ---- runtime -----------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_RUNNING_IN_CONTAINER=true

COPY --from=build /app/publish .

USER $APP_UID
EXPOSE 8080

ENTRYPOINT ["dotnet", "TwilioSimulator.dll"]
