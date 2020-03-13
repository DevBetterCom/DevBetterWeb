NOTE (13 March 2020): We're not using containers currently, nor Azure DevOps. We're using GitHub Actions and Azure App Service.

Azure Container Registry Instructions

devbetterregistry.azurecr.io

docker login:
docker login -u DevBetterRegistry -p [key] devbetterregistry.azurecr.io

docker tag
docker tag web devbetterregistry.azurecr.io/devbetterweb:v1

docker push
docker push devbetterregistry.azurecr.io/devbetterweb:v1

Making Updates

1. Make changes in solution
2. From folder with .sln file:
docker build --pull -t web -f src/DevBetterWeb.Web/Dockerfile .

3. docker login (as above - if needed)

4. docker tag web devbetterregistry.azurecr.io/devbetterweb:vN

5. docker push devbetterregistry.azurecr.io/devbetterweb:vN

6. Go to Azure Portal - Container Settings - Tag -> vN
