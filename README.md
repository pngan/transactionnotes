# transactionnotes

An application that you can add your own notes and attachments to bank transactions.

## Azure

### Build and deploy to Azure

- On GitHub, click `Actions`
- Open the github workflow `azure-dev.yaml`, and click the `Run workflow` button

![](https://github.com/pngan/transactionnotes/blob/main/github-deploy.png)

### Open the Aspire Dashboard and view App

To get the Aspire Dashboard showing in Azure
- Open the build deployment output and find the dashboard link

Or
    - Open Azure Portal
    - Click `rg-transactionnotes`
    - `Container Apps Environment`
    - `Open Dashboard`


![](https://github.com/pngan/transactionnotes/blob/main/github-output.png)

- You must first give your user Contributor permission to the `Container Apps Managed Environments`. Otherwise you will get the error: "Could not authenticate user with requested resource."
    - Open Azure Portal
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

## Prerequisites

- `azd` Azure Deploy command line tool -  `choco install azd`
 
## Reference

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

 - `cd transactionnotes\transactionnotes.AppHost`
 - `azd pipeline config`

 Once the secrets have been refreshed, try running the github action again.





