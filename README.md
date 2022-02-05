# Scrummy

## Run the *Blazor Server App* in a Docker container

To run the *Blazor Server App* within a *Docker* container, follow the two steps below. 

1) Building the Docker image

   ```
   docker build -t scrummy-blazor-server .
   ```

2) Run the container

   ```
   docker run -p 8080:80 --name scrummy-blazer-server scrummy-blazor-server
   ```