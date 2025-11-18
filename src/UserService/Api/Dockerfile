#FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
#WORKDIR /app
#
#COPY . .
#
#RUN dotnet restore /src/UserService/Api/UserService.Api.csproj
#RUN dotnet publish /src/UserService/Api/UserService.Api.csproj -c Release -o /out
#
#FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
#WORKDIR /app
#COPY --from=build /out .
#
##RUN apt-get update && apt-get install -y bash
##RUN apt-get install -y postgresql-client
##
##SHELL ["/bin/bash", "-c"]
#RUN apk add --no-cache postgresql-client bash
#
#ENTRYPOINT ["dotnet", "UserService.Api.dll"]
#

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore "src/UserService/Api/UserService.Api.csproj"
RUN dotnet publish "src/UserService/Api/UserService.Api.csproj" -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

COPY --from=build /out .

EXPOSE 8080

ENTRYPOINT ["dotnet", "UserService.Api.dll"]
