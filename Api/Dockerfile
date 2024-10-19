# Stage 1: Build Image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["Api/Api.csproj", "Api/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "Api/Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/Api"
RUN dotnet build "Api.csproj" -c Release -o /app/build

# Install dotnet-ef tool globally in the build stage
RUN dotnet tool install --global dotnet-ef

# Set the PATH to include the .dotnet/tools directory
ENV PATH="${PATH}:/root/.dotnet/tools"

# Publish image
FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copy dotnet-ef from build stage
COPY --from=build /root/.dotnet/tools /root/.dotnet/tools

# Ensure the path to dotnet tools is available
ENV PATH="${PATH}:/root/.dotnet/tools"

ENTRYPOINT ["dotnet", "Api.dll"]
