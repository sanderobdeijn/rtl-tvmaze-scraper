FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY *.sln ./
COPY */Rtl.TvMaze.csproj ./Rtl.TvMaze/
COPY */Rtl.TvMaze.Tests.csproj ./Rtl.TvMaze.Tests/
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet build -c Release --no-restore

# Run Tests
RUN dotnet test -c Release --no-build --no-restore

# Publish
RUN dotnet publish -c Release -o out --no-build --no-restore

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build-env /src/out .
ENTRYPOINT ["dotnet", "Rtl.TvMaze.dll"]