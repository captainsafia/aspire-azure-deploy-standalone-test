# DeployApp - .NET Aspire Azure Deployment Sample

A sample .NET Aspire application demonstrating cloud-native development with Azure deployment capabilities. This project showcases a multi-service application with a web frontend, API service, and Azure integrations including Cosmos DB and Blob Storage.

## 🏗️ Project Structure

This solution contains the following projects:

- **`DeployApp.AppHost`** - The Aspire AppHost that orchestrates the application and defines cloud resources
- **`DeployApp.Web`** - Blazor web frontend application
- **`DeployApp.ApiService`** - ASP.NET Core Web API that provides weather forecast data
- **`DeployApp.ServiceDefaults`** - Shared service defaults and configurations

### Infrastructure Components

- **Azure Container App Environment** - Hosting environment for containerized services
- **Azure Cosmos DB** - NoSQL database with preview emulator support for local development
- **Azure Blob Storage** - Object storage for images and files
- **Bicep Templates** - Infrastructure as Code definitions located in individual folders

## 🚀 Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for local development)
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) (for deployment)
- [.NET Aspire CLI](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling)

### Installation

1. **Clone the repository:**
   ```bash
   git clone https://github.com/captainsafia/aspire-azure-deploy-standalone-test.git
   cd aspire-azure-deploy-standalone-test
   ```

2. **Install .NET Aspire CLI:**
   ```bash
   curl -sSL https://aspire.dev/install.sh | bash
   ```

## 🔧 Local Development

### Running the Application Locally

Use the Aspire CLI to run the entire application stack locally with orchestration:

```bash
aspire run
```

This command will:
- Start the Aspire dashboard at `http://localhost:15000`
- Launch all services with proper service discovery
- Initialize local emulators for Azure services (Cosmos DB, Storage)
- Provide real-time monitoring and logging capabilities

## ☁️ Azure Deployment

### Manual Deployment

Deploy the application to Azure using the Aspire CLI:

```bash
aspire deploy
```

This command will:
- Build and containerize your applications
- Deploy infrastructure using the generated Bicep templates
- Deploy application containers to Azure Container Apps
- Configure service connections and environment variables

### Required Azure Configuration

Before deploying, ensure you have authenticated with Azure via the `az` CLI, the Azure extension in VS Code, or in Visual Studio.

### Deployment Output

After successful deployment, you'll receive:
- URLs for your deployed applications
- Resource group information
- Connection strings and endpoints

## 🔄 Continuous Deployment

### GitHub Actions Workflow

The repository includes a GitHub Actions workflow (`.github/workflows/deploy.yml`) that automatically deploys the application when changes are pushed to the main branch.

#### Workflow Features

- **Triggers:** Pushes to `main`, pull requests, and manual dispatch
- **Environment:** Runs on Ubuntu latest
- **Security:** Uses OpenID Connect for secure Azure authentication
- **Deployment:** Automated Aspire CLI deployment to Azure

#### Setting Up Azure Authentication

The pipeline uses federated credentials to authenticate with Azure in the pipeline.

1. Create an Azure app registration using `az ad app create --display-name "GitHub-Actions-App"`.
2. Create a service principal using `az ad sp create --id <app-id-from-step-1>`.
3. Configure the federated credentials:
```
az ad app federated-credential create \
  --id <app-id> \
  --parameters '{
    "name": "GitHub-Actions",
    "issuer": "https://token.actions.githubusercontent.com",
    "subject": "repo:your-org/your-repo:ref:refs/heads/main",
    "audiences": ["api://AzureADTokenExchange"]
  }'
```
4. Configure the service principal with permissions to configure role assignments on the target subscription.

```
az role assignment create \
  --assignee <your-service-principal-id> \
  --role "User Access Administrator" \
  --scope "/subscriptions/<subscription-id>"
```

#### Required GitHub Secrets

Configure the following secrets in your GitHub repository:

| Secret Name | Description | Example |
|-------------|-------------|---------|
| `AZURE_TENANT_ID` | Azure Active Directory Tenant ID | `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx` |
| `AZURE_CLIENT_ID` | Service Principal Client ID | `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx` |
| `AZURE_SUBSCRIPTIONID` | Azure Subscription ID | `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx` |
| `AZURE_LOCATION` | Azure region for deployment | `eastus` |
| `AZURE_RESOURCEGROUP` | Target resource group name | `my-aspire-app-rg` |
| `WEATHER_API_KEY` | API key for weather forecast service | `test-api-key` |

### Workflow Steps

The deployment workflow performs the following steps:

1. **Code Checkout** - Retrieves the latest code
2. **Environment Setup** - Installs .NET SDK based on `global.json`
3. **Azure Authentication** - Securely authenticates using OIDC
4. **Aspire CLI Installation** - Downloads and installs the latest Aspire CLI
5. **Deployment** - Executes `aspire deploy` with environment variables