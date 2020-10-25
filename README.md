# TVMaze Scraper

This project scrapes the TV Maze API for show and cast info and stores it.
The results are then accessible by an OpenAPI Specification v3 compliant api.

# This solution

The solution is written in dotnet core 3.1. This is the current LTS version and is recommended at this moment.

# How to use

- Clone the repository
- In the root folder run 
```
docker-compose build
```
- In the root folder run 
```
docker-compose up -d
```
# Prerequisites

- docker
# To improve
- Add unit tests
- Add certificate to docker to enable https
- Check if calls are actually rate limited