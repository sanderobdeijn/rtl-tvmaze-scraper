# TVMaze Scraper

This project scrapes the TV Maze API for show and cast info and stores it.
The results are then accessible by an OpenAPI Specification v3 compliant api.

# This solution

The solution is written in dotnet core 3.1. This is the current LTS version and is recommended at this moment.
The TvMaze documentation suggests that the show endpoint are behind the edge cache and aren't rate limited. For added safety the request are within a polly retry policy

# How to use

- Clone the repository
- In the that contains the .csproj file run
```
dotnet run
```

# Prerequisites

- docker
# To improve
- Add Docker
- Add unit tests
- Add certificate to docker to enable https
- Check if calls are actually rate limited