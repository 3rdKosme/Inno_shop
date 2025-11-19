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
