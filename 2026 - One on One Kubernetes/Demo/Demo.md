## Prerequisites
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
kubectl get pod -n aksdemoapp-prod
kubectl delete pod aksdemoapp-prod-XXX -n aksdemoapp-prod
kubectl get pod -n aksdemoapp-prod

## Load Balancer Demo
kubectl get pods
kubectl get service kubernetesdemo-service

.\KubernetesRequestDemo.exe 20.220.40.231

## Pull Request Deployment

## HPA
kubectl get hpa -n aksdemoapp-prod
kubectl get pod -n aksdemoapp-prod --watch
aks-demo.programmingwithwolfgang.com

## Zero Downtime Deployment Demo
kubectl get pods --watch

wolfgangofner/kubernetesdeploymentdemo:end

.\KubernetesRequestDemo.exe 20.220.40.231

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
kubectl taint nodes aks-nodepool1-19549440-vmss000003 gpu=nvidia:NoSchedule
kubectl taint nodes aks-nodepool1-19549440-vmss000004 gpu=amd:NoSchedule
kubectl create ns tolerations
kubectl config set-context --current --namespace=tolerations
kubectl apply -f .\podTolerations.yaml
kubectl describe pod tolerations-container

update tolerations in podTolerations
kubectl delete pod tolerations-container
kubectl apply -f .\podTolerations.yaml
kubectl describe pod tolerations-container