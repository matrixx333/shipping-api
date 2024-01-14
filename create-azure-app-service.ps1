# Define variables
$resourceGroupName = "shipping-api"
$location = "westus2"  # Set this to your preferred Azure region
$appServicePlanName = "shipping-api-plan"
$appServiceName = "shipping-api"

# Create a new resource group
az group create --name $resourceGroupName --location $location

# Create a new App Service plan with Free tier
az appservice plan create --name $appServicePlanName --resource-group $resourceGroupName --sku F1

# Create a new App Service
az webapp create --name $appServiceName --resource-group $resourceGroupName --plan $appServicePlanName