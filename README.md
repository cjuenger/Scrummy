# Scrummy

This project shall allow to request several metrics that are important for a Scrum Team from platforms like GitLab, GitHub, etc.

**NOTE:** This project is currently in progress! Right now it is at the very beginning and supports only GitLab.

## Settings

At the moment the first necessary step is to enter the *Project ID* of the GitLab project of concern as well as the GitLab *Access Token*. Both will not be persisted and thus will get lost on a page refresh.

![](../Scrummy/images/settings.png)

## Metrics

### Release Report

The first relatively good - *unfortunatley slow loading at the moment* - working metric is the *Release Report*. As mentioned in the *Settings* section you must perform the necessary settings before.

![](../Scrummy/images/release_report.png)


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