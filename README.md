# transactionnotes

An application that you can add your custom notes and attachments to downloaded bank transactions.

## Running Locally using Visual Studio

Press `F5` to run. This will open the Aspire dashboard and the Web App. 

## Running Locally using Rider

Rider needs to be set up to run Aspire. Install the Aspire Plugin. To run, open the `Services` Window, and Right
Click the `Aspire` plugin, and choose `Debug`.
[](readme-images/rider-services.png)

## Deploy the App to Azure
The app is deployed to Azure using the workflow `azure-dev.yml`.
Once the changes have been pushed to the `main` branch, deploy by clicking the `Run workflow` button on the 
GitHub site under the `Actions` section.

## View Azure Aspire Dashboard
To view the Aspire Dashboard showing in Azure
[Open the dashboard link in the Azure portal](readme-images/azure-cae.png)

When you attempt to view the dashboard but get the error `"Could not authenticate user with requested resource."` then [you must first give your user Contributor permission to the Container App Environment](readme-images/azure-iam.png)

## To Remove Azure Resource
You may want to tear down the App resources deployed in Azure when they are not in use and you want to conserve using Azure credits.
From the command line
```
cd <repo>\transactionnotes.AppHost
azd down
```

## Reference

[Create Aspire Blazor App using Visual Studio](https://youtu.be/BBGeousUHQU?si=UtCmni5sZBfo92qV)
[Deploy App to Azure Container Services using GitHub Actions](https://learn.microsoft.com/en-us/dotnet/aspire/deployment/azure/aca-deployment-github-actions?tabs=windows&pivots=github-actions)

