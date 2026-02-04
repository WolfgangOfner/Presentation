## Prerequisites
az aks get-credentials --resource-group MicroserviceDemo --name microservice-aks
Set DNS for customerapi and *.customerapi
kubectl apply -f k8sdemo.yml
kubectl get service --> EXTERNAL_IP

## Docker Demo
Create new project
add Dockerfile
docker run 

docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=MySecretPassword1!" -p 1433:1433 --name sqlserver --hostname sqlserver -d mcr.microsoft.com/mssql/server:2025-latest

Visual Studio Code 

## Docker Compose Demo

## Self-heal Demo
kubectl get pod -n customerapi-prod
kubectl delete pod customerapi-XXX -n customerapi-prod
kubectl get pod -n customerapi-prod

## Load Balancer Demo
kubectl get pods
kubectl get service kubernetesdemo-service

.\KubernetesRequestDemo.exe EXTERNAL_IP

## Pull Request Deployment

## HPA
kubectl get hpa customerapi -n customerapi-prod
kubectl get pods -n customerapi-prod --watch
customer.programmingwithwolfgang.com
1000000

## Zero Downtime Deployment Demo
kubectl get pods --watch

wolfgangofner/kubernetesdeploymentdemo:end

.\KubernetesRequestDemo.exe EXTERNAL_IP

kubectl apply -f k8sdemo.yml

## Helm Demo

new application
add docker
add Helm

cd HelmDemo
mkdir charts
cd charts

docker image ls
update values.yaml

kubectl create namespace helmdemo
kubectl config set-context --current --namespace=helmdemo
helm install helm helmdemo

docker tag helmdemo:latest wolfgangofner/helmdemo:stable

docker push wolfgangofner/helmdemo:stable

helm upgrade helm helmdemo

Dashboard

change repository and tag in chart

docker tag helmdemo:latest wolfgangofner/helmdemo:latest

docker push wolfgangofner/helmdemo:latest

helm ls

kubectl port-forward helmdemo-6684667fb5-xlkg9 8080:80

helm uninstall helm

helm ls


# Quota
kubectl create ns quota
kubectl config set-context --current --namespace=quota
kubectl apply -f .\quotaCpu.yaml
kubectl apply -f .\podQuota.yaml
kubectl get pods
kubectl get replicaset
kubectl describe replicaset XXX

kubectl delete ResourceQuota cpu-quota
kubectl delete deployment nginx
kubectl apply -f .\quotaPod.yaml
kubectl apply -f .\podQuota.yaml
kubectl get pods
kubectl get replicaset
kubectl describe replicaset XXX

# Node selector
kubectl create ns nodeselector
kubectl config set-context --current --namespace=nodeselector
kubectl get nodes --show-labels
show labels in Azure portal
kubectl get nodes
kubectl label nodes XXX gpu=nvidia
kubectl apply -f .\podNodeSelector.yaml
kubectl describe pod nginx

# Affinity and anti-affinity
kubectl create ns affinity
kubectl config set-context --current --namespace=affinity
kubectl label nodes XXX gpu=nvidia
kubectl apply -f .\podAffinity.yaml
kubectl describe pod affinity-container

# Taints and Tolerations
kubectl get nodes
kubectl taint nodes aks-nodepool1-32070840-vmss000000 gpu=nvidia:NoSchedule
kubectl taint nodes aks-nodepool1-32070840-vmss000003 gpu=amd:NoSchedule
kubectl create ns tolerations
kubectl config set-context --current --namespace=tolerations
kubectl apply -f .\podTolerations.yaml
kubectl describe pod tolerations-container

update tolerations in podTolerations
kubectl delete pod tolerations-container
kubectl apply -f .\podTolerations.yaml
kubectl describe pod tolerations-container

# Workload Identitiy
$ResourceGroup="KubernetesDemo"
$AksName="WorkloadIdentityCluster"
$Location="canadacentral" # choose location close to you
$ServiceAccountNamespace="workload-identity"
$ServiceAccountName="workload-identity-sa"
$SubscriptionId="$(az account show --query id --output tsv)"
$UserAssignedIdentityName="userAssignedIdentity"
$FederatedIdentityCredentialName="federateIdentitiy"
$KeyVaultName="kubernetesdemowolfgang" # name must be unique
$KeyVaultSecret="WorkloadSecret"

az group create -n $ResourceGroup --location $Location

az keyvault create --resource-group "$ResourceGroup" --name "$KeyVaultName" --enable-rbac-authorization true
az role assignment create --role "Key Vault Administrator" --assignee <Your_Email> --scope /subscriptions/$SubscriptionId/resourcegroups/$ResourceGroup/providers/Microsoft.KeyVault/vaults/$KeyVaultName
az keyvault secret set --vault-name "$KeyVaultName" --name "$KeyVaultSecret" --value 'This is a secret!'
$KeyVaultUrl="$(az keyvault show -g "$ResourceGroup" -n $KeyVaultName --query properties.vaultUri -o tsv)"

az aks create -g "$ResourceGroup" -n $AksName --node-count 1 --enable-oidc-issuer --enable-workload-identity

$AksOidcIssuer="$(az aks show -n $AksName -g "$ResourceGroup" --query "oidcIssuerProfile.issuerUrl" -o tsv)"

az identity create --name "$UserAssignedIdentityName" --resource-group "$ResourceGroup" --subscription "$SubscriptionId"
$UserAssignedClientId="$(az identity show --resource-group "$ResourceGroup" --name "$UserAssignedIdentityName" --query 'clientId' -o tsv)"
az role assignment create --role "Key Vault Secrets User" --assignee "$UserAssignedClientId" --scope /subscriptions/$SubscriptionId/resourcegroups/$ResourceGroup/providers/Microsoft.KeyVault/vaults/$KeyVaultName

az aks get-credentials --resource-group $ResourceGroup --name $AksName --overwrite-existing
kubectl create ns workload-identity
kubectl config set-context --current --namespace=workload-identity
kubectl apply -f serviceAccountWorkloadIdentitiy.yaml

az identity federated-credential create --name $FederatedIdentityCredentialName --identity-name $UserAssignedIdentityName --resource-group $ResourceGroup --issuer $AksOidcIssuer --subject system:serviceaccount:${ServiceAccountNamespace}:${ServiceAccountName}

kubectl apply -f podWorkloadIdentitiy.yaml
kubectl get pods
kubectl logs quick-start

# ACR Access Image Pull Secret
az acr import -n kubernetestrainingwolfgang --source docker.io/library/nginx:latest --image nginx

az aks update -n microservice-aks -g MicroserviceDemo --attach-acr kubernetestrainingwolfgang
kubectl create ns acr
kubectl config set-context --current --namespace=acr

kubectl apply -f .\podAcr.yaml
show permissions in ACR

# ACR Access Repository Token
Switch AKS due to caching

kubectl create ns acr
kubectl config set-context --current --namespace=acr
kubectl create secret docker-registry registrytoken --docker-server=kubernetestrainingwolfgang.azurecr.io --docker-username=token --docker-password=XXX
kubectl apply -f .\podToken.yaml

# Key Vault Secret store
$ResourceGroup="MicroserviceDemo"
$ResourceGroupKV="KubernetesDemo"
$AksName="microservice-aks"
$KeyVaultName="kubernetesdemowolfgang"
$SubscriptionId="$(az account show --query id --output tsv)"

az aks enable-addons --addons azure-keyvault-secrets-provider --name $AksName --resource-group $ResourceGroup
kubectl get pods -n kube-system

az aks show -g $ResourceGroup -n $AksName --query addonProfiles.azureKeyvaultSecretsProvider.identity.objectId -o tsv
$UserAssignedClientID = $(az aks show -g $ResourceGroup -n $AksName --query addonProfiles.azureKeyvaultSecretsProvider.identity.clientId -o tsv)

kubectl create ns keyvault
kubectl config set-context --current --namespace=keyvault

Set $UserAssignedClientID as userAssignedIdentityID in secretProvider.yaml

az role assignment create --role "Key Vault Secrets User" --assignee $UserAssignedClientID --scope /subscriptions/$SubscriptionId/resourcegroups/$ResourceGroupKV/providers/Microsoft.KeyVault/vaults/$KeyVaultName

kubectl apply -f .\secretProvider.yaml
kubectl apply -f .\podSecret.yaml

kubectl exec busybox-secrets-store-inline-user-msi -- ls /mnt/secrets-store/
kubectl exec busybox-secrets-store-inline-user-msi -- cat /mnt/secrets-store/WorkloadSecret

# Private cluster
az aks command invoke --resource-group KubernetesDemo --name privatecluster --command "kubectl create ns private"
az aks command invoke --resource-group KubernetesDemo --name privatecluster --command "kubectl config set-context --current --namespace=private"
az aks command invoke --resource-group KubernetesDemo --name privatecluster --command "kubectl create secret docker-registry registrytoken --docker-server=kubernetestrainingwolfgang.azurecr.io --docker-username=token --docker-password=DzFJDj8vqZiI7TwU7PnRwsoAL+SiJMiqbfWO3ZW88i+ACRArxo3g -n registry"
az aks command invoke --resource-group KubernetesDemo --name privatecluster --command "kubectl apply -f podPrivate.yaml -n registry" --file podPrivate.yaml
az aks command invoke --resource-group KubernetesDemo --name privatecluster --command "kubectl get pods -n registry"

# Azure Policy
az aks enable-addons --addons azure-keyvault-secrets-provider --name microservice-aks --resource-group MicroserviceDemo
kubectl create ns policy
kubectl config set-context --current --namespace=policy
kubectl get constrainttemplates
kubectl apply -f .\podPrivileged.yaml
kubectl apply -f .\podUnprivileged.yaml