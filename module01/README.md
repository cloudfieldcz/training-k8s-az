# 01 - Building application containers

For your development you can use docker-compose configuration which is ready there in subfolder `src/` / commands `docker-compose build` and `docker-compose up` can be used for local testing.

## Build image in Azure Container Registry

```bash
# enter directory with source codes
cd module01

# variables 
. ../rc

# build SPA application in ACR - build has to be done from folder with source codes: k8s-workshop-developer
echo v1 > ./src/myappspa/version
az acr build --registry $ACR_NAME --image myappspa:v1 ./src/myappspa

# for purpose of lab create v2 of your app by only changing from v1 to v2 in version file and build container with v2 tag
echo v2 > ./src/myappspa/version
az acr build --registry $ACR_NAME --image myappspa:v2 ./src/myappspa

# build JAVA microservice (spring-boot)
az acr build --registry $ACR_NAME --image myapptodo:v1 ./src/myapptodo

# build NetCore testing API service for perf test
az acr build --registry $ACR_NAME --image webstress:v1 ./src/webstress
```

## Try to run some container in Azure Container Instances

```bash
az container create -g ${RESOURCE_GROUP} -l ${LOCATION} \
  --name myapp --image ${ACR_URL}/myappspa:v1 \
  --ports 8080 --ip-address public \
  --registry-username ${ACR_NAME} --registry-password "${ACR_KEY}"
```

Grab public IP address from output and now you can test it with your browser `http://YOUR-IP-ADDRESS:8080` ..

And finally we can delete container instance from Azure Portal ...

## Another option - Web App

Don't forget about Azure Web App (App Service) where we can host our docker images too. If you want you can deploy Azure Web App (Linux) and host there your image - you have to chose during Web App configuration existing Azure Container Registry and your image with application. If you will experiment with our image myappspa - don't forget to configure docker container entry point to port 8080.

### Deploy container to Web App for Linux

Now we will create simple Web app which will host our .NetCore docker container connected to CosmosDB. Please replace **Web App name** for your unique name.

```bash
export WEBAPPNAME="valdaakssecwebapp001"

# Create service plan
az appservice plan create --name ${WEBAPPNAME} --resource-group ${RESOURCE_GROUP} --sku B1 --is-linux

# Create Web App
az webapp create --resource-group ${RESOURCE_GROUP} --plan ${WEBAPPNAME} \
  --name ${WEBAPPNAME} \
  --deployment-container-image-name ${ACR_URL}/myappspa:v1

```

Now you can access application on HTTPS endpoint (check azure portal for URL name)
