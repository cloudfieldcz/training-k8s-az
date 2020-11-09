# k8s-workshop
This repo contains materials for one-day App Dev on Azure Kubernetes Service training.
Repository content is based on original repository: https://github.com/azurecz/k8s-workshop-developer

# Lab preparation

We will use shell.azure.com cloud shell (bash) for experiments.
Before you will start please CLONE our repo `git@github.com:cloudfieldcz/training-k8s.git`

## Clone repo

```bash
mkdir myexperiment
cd myexperiment

# clone repo first
git clone git@github.com:cloudfieldcz/training-k8s.git

# and now lets step into folder with project files.
cd training-k8s
```

## Create Azure Container Registry

We will use Azure container registry for our images, lets deploy ACR and push images there ..

Please use especially for ACR_NAME **your unique name** - it is unique in whole Azure environment in 'rc' file which contains all variable definition for our experiment. If you will run `. rc` first time it can produce few errors because not all resources exist yet.


```bash
# variables
. rc

# create resource group
az group create --location ${LOCATION} --name ${RESOURCE_GROUP}

# create ACR
az acr create --name $ACR_NAME --resource-group $RESOURCE_GROUP --sku Basic --location ${LOCATION} --admin-enabled true

# Get ACR_URL for future use with docker-compose and build
export ACR_URL=$(az acr show --name $ACR_NAME --resource-group $RESOURCE_GROUP --query "loginServer" --output tsv)
echo $ACR_URL

# Get ACR key for our experiments
export ACR_KEY=$(az acr credential show --name $ACR_NAME --resource-group $RESOURCE_GROUP --query "passwords[0].value" --output tsv)
echo $ACR_KEY
```

## Create Azure Kubernetes Service

```bash
# aks - create cluster
az aks create --resource-group ${RESOURCE_GROUP} --name ${AKS_CLUSTER_NAME} \
  --no-ssh-key --kubernetes-version 1.17.11 \
  --node-count 2 --node-vm-size Standard_B2s \
  --location ${LOCATION}
# kube config
az aks get-credentials --admin --name ${AKS_CLUSTER_NAME} --resource-group ${RESOURCE_GROUP}

# patch kubernetes configuration to be able to access dashboard (only for demonstration)
kubectl delete clusterrolebinding kubernetes-dashboard
kubectl create clusterrolebinding kubernetes-dashboard \
  -n kube-system --clusterrole=cluster-admin \
  --serviceaccount=kube-system:kubernetes-dashboard
```

## Setup access for AKS to ACR

```bash
# Get the id of the service principal configured for AKS
CLIENT_ID=$(az aks show --resource-group $RESOURCE_GROUP --name $AKS_CLUSTER_NAME --query "servicePrincipalProfile.clientId" --output tsv)

# Get the ACR registry resource id
ACR_ID=$(az acr show --name $ACR_NAME --resource-group $RESOURCE_GROUP --query "id" --output tsv)

# Create role assignment
az role assignment create --assignee $CLIENT_ID --role AcrPull --scope $ACR_ID
```

# Labs

## [01 - Building application containers](module01/README.md)

## [02 - Introduction to Azure Kubernetes Service](module02/README.md)

## [03 - Deploy application to Azure Kubernetes Service](module03/README.md)

## [04 - Optimizing deployment in Kubernetes](module04/README.md)

## [05 - DevOps with AKS and introduction to helm](module05/README.md)

