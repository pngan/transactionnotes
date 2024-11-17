# transactionnotes

An application that you can add your own notes and attachments to bank transactions.

## Reference

[https://youtu.be/BBGeousUHQU?si=UtCmni5sZBfo92qV](Create Aspire Blazor App using Visual Studio)
[https://learn.microsoft.com/en-us/dotnet/aspire/deployment/azure/aca-deployment-github-actions?tabs=windows&pivots=github-actions](Deploy App to Azure Container Services using GitHub Actions)

## Azure
To get the Aspire Dashboard showing in Azure
- Open the build deployment output and find the dashboard link
![](https://github.com/pngan/transactionnotes/blob/main/github-output.png)

- You must first give your user Contributor permission to the Container App Environment. Otherwise you will get the error: "Could not authenticate user with requested resource."
![](https://github.com/pngan/transactionnotes/blob/main/azure-iam.png)
