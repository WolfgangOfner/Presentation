## Define Variables
```
$AksName="gateway-api-aks"
$Location="CanadaCentral"
$ResourceGroupName="gateway-api-rg"

$GatewayNamespace="envoy-gateway"
$GatewayName="gateway"
$GatewayClassName="envoy"

$DemoNamespace="demo-ns"
$DemoAppNameOne="demoone"
$DemoAppNameTwo="demotwo"
$DemoUrl="demo.programmingwithwolfgang.com" # change to your URL
```

## Create AKS
```
az group create `
  --name $ResourceGroupName `
  --location $Location

az aks create `
  --name $AksName `
  --resource-group $ResourceGroupName

az aks get-credentials `
  --resource-group $ResourceGroupName `
  --name $AksName `
  --overwrite-existing
```

## Deploy Envoy and create the Gateway and GatewayClass
```
helm install envoy oci://docker.io/envoyproxy/gateway-helm `
  --version v1.8.3 `
  --namespace $GatewayNamespace `
  --create-namespace `
  --set deployment.replicas=3

kubectl get gateway -A
kubectl get gatewayclass

$GatewayClass = @"
apiVersion: gateway.networking.k8s.io/v1
kind: GatewayClass
metadata:
  name: $GatewayClassName
spec:
  controllerName: gateway.envoyproxy.io/gatewayclass-controller
"@

$GatewayClass | kubectl apply -f -

$Gateway = @"
apiVersion: gateway.networking.k8s.io/v1
kind: Gateway
metadata:
  name: $GatewayName
  namespace: $GatewayNamespace
  annotations:
    cert-manager.io/cluster-issuer: $ClusterIssuerName
spec:
  gatewayClassName: $GatewayClassName
  listeners:
  - name: http-listener
    port: 80
    protocol: HTTP
    hostname: $DemoUrl
    allowedRoutes:
      namespaces:
        from: All 
"@

$Gateway | kubectl apply -f -

kubectl get gateway -n $GatewayNamespace 
kubectl describe gateway $GatewayName -n $GatewayNamespace
```

might take a minute to switch Programmed = True

```
kubectl get gatewayclass

kubectl get gateway $GatewayName -n $GatewayNamespace -o jsonpath='{.status.addresses[0].value}'
```

Update DNS

## Deploy Test App
```
kubectl create ns $DemoNamespace

kubectl run $DemoAppNameOne `
  --image=traefik/whoami:latest `
  -n $DemoNamespace `
  --expose --port=80

kubectl run $DemoAppNameTwo `
  --image=traefik/whoami:latest `
  -n $DemoNamespace `
  --expose --port=80

kubectl get all -n $DemoNamespace

$HttpRouteDemoApp = @"
apiVersion: gateway.networking.k8s.io/v1
kind: HTTPRoute
metadata:
  name: demo-http-route
  namespace: $DemoNamespace
spec:
  parentRefs:
    - name: $GatewayName
      namespace: $GatewayNamespace
  hostnames:
    - "$DemoUrl" 
  rules:
    - matches:
      backendRefs:
        - name: $DemoAppNameOne
          port: 80
"@

$HttpRouteDemoApp | kubectl apply -f -

curl http://$DemoUrl -UseBasicParsing
```

## Path Routing
```
$HttpRouteDemoApp = @"
apiVersion: gateway.networking.k8s.io/v1
kind: HTTPRoute
metadata:
  name: demo-http-route
  namespace: $DemoNamespace
spec:
  parentRefs:
    - name: $GatewayName
      namespace: $GatewayNamespace
  hostnames:
    - "$DemoUrl" 
  rules:
    - matches:
        - path:
            type: PathPrefix
            value: /routing 
        - path:
            type: Exact
            value: /exact 
      backendRefs:
        - name: $DemoAppNameOne
          port: 80
    - backendRefs:
      - name: $DemoAppNameTwo
        port: 80
"@

$HttpRouteDemoApp | kubectl apply -f -

curl http://$DemoUrl -UseBasicParsing
curl http://$DemoUrl/exact -UseBasicParsing
curl http://$DemoUrl/exact/abc -UseBasicParsing
curl http://$DemoUrl/routing/abc -UseBasicParsing
```

## Routing based on Weight
```
$HttpRouteDemoApp = @"
apiVersion: gateway.networking.k8s.io/v1
kind: HTTPRoute
metadata:
  name: demo-http-route
  namespace: $DemoNamespace
spec:
  parentRefs:
    - name: $GatewayName
      namespace: $GatewayNamespace
  hostnames:
    - "$DemoUrl"
  rules: 
  - backendRefs:
    - name: $DemoAppNameOne
      port: 80
      weight: 20
    - name: $DemoAppNameTwo
      port: 80
      weight: 80
"@

$HttpRouteDemoApp | kubectl apply -f -

DemoUrl="demo.programmingwithwolfgang.com"
watch -n 1 curl http://$DemoUrl

$HttpRouteDemoApp = @"
apiVersion: gateway.networking.k8s.io/v1
kind: HTTPRoute
metadata:
  name: demo-http-route
  namespace: $DemoNamespace
spec:
  parentRefs:
    - name: $GatewayName
      namespace: $GatewayNamespace
  hostnames:
    - "$DemoUrl"
  rules: 
  - backendRefs:
    - name: $DemoAppNameOne
      port: 80
      weight: 50
    - name: $DemoAppNameTwo
      port: 80
      weight: 50
"@

$HttpRouteDemoApp | kubectl apply -f -

$HttpRouteDemoApp = @"
apiVersion: gateway.networking.k8s.io/v1
kind: HTTPRoute
metadata:
  name: demo-http-route
  namespace: $DemoNamespace
spec:
  parentRefs:
    - name: $GatewayName
      namespace: $GatewayNamespace
  hostnames:
    - "$DemoUrl"
  rules: 
  - backendRefs:
    - name: $DemoAppNameOne
      port: 80
      weight: 99
    - name: $DemoAppNameTwo
      port: 80
      weight: 1
"@

$HttpRouteDemoApp | kubectl apply -f -
```