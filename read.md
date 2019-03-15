# Docker
## Build the image
* docker build . -t villasport:local
## See the image created 
* docker image list
## Run the container
* docker run -d -p 5000:80 villasport:local
## See the container running
* docker ps
## Run the api
* go to http://localhost:5000/api/villasport

## Other docker commands
* **warning**: remove all containers: docker container rm -f $(docker ps -aq)
* remove single container: docker container rm -f containerid
* remove single image: docker rmi imageid

# Kubernetes: 2 ways to deploy: Interactively and Via yml/json
##  Interactively:
### Create deployment
* kubectl run villasport-deployment --image=villasport:local --port=80 --replicas=3
### Create Service
* kubectl expose deployment villasport-service —type=NodePort

##  Via yml:
* kubectl create -f villasport-deploy.yml

# Verify deployment
* kubectl get svc
* check the ports and go to the browser with http://localhost:'port-here'/api/villasport
 
## Other kubernetes commands
## Delete resources created on yml
*  kubectl delete -f villasport-deploy.yml 

### Delete deployment
* kubectl delete deployment villasport-deployment
### Delete service
* kubectl delete service villasport-service


### Get pods
* kubectl get pods
### Get services
* kubectl get svc
### Get deployments
* kubectl get deployments
### Get replicas
* kubectl get rs

# Serilog and Application Insights
## Serilog: On Debug mode is logging into the console.
## ApplicationInsights: When is not Debug mode is logging into ApplicationInsights. Check appsettings.json to see the Serilog config.
