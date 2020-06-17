
# Azure Log Analytics FunctionApp for generic :) custom logging

``` local.settings.json
{
    "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "LogAnalyticWorkSpaceId": "",
    "WorkSpaceSharedKey": "",
    "GenericLogName": ""
  }
}
```


# Json example

```json example for posting
 [
    {
       "Name":"DEMO",
       "Categorie":"DEMO",
       "Location":"DEMO",
       "CurrentValue":"DEMO",
       "Limit":1,
       "SubscriptionName":"",
       "SubscriptionId": "",
       "UsagePercentage": 12
	}
 ]
