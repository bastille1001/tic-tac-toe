FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY EmployCity.TicTacToe.API.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "EmployCity.TicTacToe.API.dll"]
EXPOSE 80