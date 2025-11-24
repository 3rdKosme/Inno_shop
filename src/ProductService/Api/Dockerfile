FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore "src/ProductService/Api/ProductService.Api.csproj"
RUN dotnet publish "src/ProductService/Api/ProductService.Api.csproj" -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

COPY --from=build /out .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Inno_Shop.ProductService.Api.dll"]
