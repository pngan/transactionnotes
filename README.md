# transactionnotes

An application that you can add your own notes and attachments to bank transactions.

## Docker Compose

### Build and deploy to Docker Compose

This app is built using  .Net Aspire. By default these app are deployed publically to Azure Container Services. However using a tool called `aspirate` they can be deployed to container targets such as
Docker Compose. To deploy to docker compose, you first need `Docker` installed on the developer machine, and have it running.

Then run the command script [publish-to-dockerhub.cmd](publish-to-dockerhub.cmd) . The contents of this script needs to be changed to your particular dockerhub account. The aspirate secrets are 
stored in my password manager.

This command will create a set of docker containers that should be pushed to a docker image repository - such as dockerhub - and a `docker-compose.yaml` file. This file needs a few adjustments 
and has been saved as [docker-compose.yaml](docker-compose.yaml).

## Azure

The most convenient way to deploy .Net Aspire apps to the public is to host them on Azure Container Services. In this repository, GitHub actions has been setup to deploy to Azure.

### Build and deploy to Azure

- On GitHub, click `Actions`
- Open the github workflow `azure-dev.yaml`, and click the `Run workflow` button

![](https://github.com/pngan/transactionnotes/blob/main/github-deploy.png)

### Open the Aspire Dashboard and view App

To get the Aspire Dashboard showing in Azure
- Open the build deployment output and find the dashboard link

Or
    - [Open Azure Portal](https://portal.azure.com)
    - Click `rg-transactionnotes`
    - `Container Apps Environment`
    - `Open Dashboard`


![](https://github.com/pngan/transactionnotes/blob/main/github-output.png)

- You must first give your user Contributor permission to the `Container Apps Managed Environments`. Otherwise you will get the error: "Could not authenticate user with requested resource."
    - [Open Azure Portal](https://portal.azure.com)
    - Click `rg-transactionnotes`
    - Access Control (IAM)
    - `Add Role Assignment`
    - `Privileged administrator roles`
    - `Contributor` > `Next`
    - `Members` > `User, group, or service principal` 
    - `Select Members` > <your login email> > `Select`
    - `Review + assign`
    - `Review + assign`


![](https://github.com/pngan/transactionnotes/blob/main/azure-iam.png)


### Tear down the deployment

 - `cd transactionnotes\transactionnotes.AppHost`
 - azd down

### Prerequisites

- `azd` Azure Deploy command line tool -  `choco install azd`
 
### Reference

- [Create Aspire Blazor App using Visual Studio](https://youtu.be/BBGeousUHQU?si=UtCmni5sZBfo92qV)
- [Deploy App to Azure Container Services using GitHub Actions](https://learn.microsoft.com/en-us/dotnet/aspire/deployment/azure/aca-deployment-github-actions?tabs=windows&pivots=github-actions)

## Troubleshooting

If the github actions workflow fails with this error:
```
 Deploying service apiservice (Logging in to registry)
  (x) Failed: Deploying service apiservice

ERROR: failed deploying service 'apiservice': logging in to registry: failed logging into container registry, token: failed getting ACR token:
```

It is because some secret has expired in github secrets.
Refresh the credentials, by own your Windows machine
```
cd transactionnotes\transactionnotes.AppHost
azd pipeline config
```

 Once the secrets have been refreshed, try running the github action again.

----


If the github actions workflow fails with this error:
```
panic: don't know how to prompt for type *survey.Password
```
This is error arises when a new Aspire parameter has been added, e.g.
```
var password = builder.AddParameter("keycloak-password", secret: true);
```
`azd` prompts for that missing paramter when it is running in github actions, but cannot interactively prompt.

The solution is to re-run on your local developer machine:
```
cd transactionnotes\transactionnotes.AppHost
azd pipeline config
```
This will prompt you for the missing secret values and store them with the Actions in the source repo.