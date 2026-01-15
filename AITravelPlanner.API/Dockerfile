FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["AITravelPlanner.API/AITravelPlanner.API.csproj", "AITravelPlanner.API/"]
COPY ["AITravelPlanner.Services/AITravelPlanner.Services.csproj", "AITravelPlanner.Services/"]
COPY ["AITravelPlanner.Data/AITravelPlanner.Data.csproj", "AITravelPlanner.Data/"]
COPY ["AITravelPlanner.Domain/AITravelPlanner.Domain.csproj", "AITravelPlanner.Domain/"]
COPY ["AITravelPlanner.Common/AITravelPlanner.Common.csproj", "AITravelPlanner.Common/"]

RUN dotnet restore "AITravelPlanner.API/AITravelPlanner.API.csproj"

COPY . .
WORKDIR "/src/AITravelPlanner.API"
RUN dotnet publish "AITravelPlanner.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000
ENV ASPNETCORE_URLS=http://0.0.0.0:5000
ENTRYPOINT ["dotnet", "AITravelPlanner.API.dll"]
