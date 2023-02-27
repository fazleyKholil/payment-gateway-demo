param storageAccountName string
param location string = resourceGroup().location
param queueName string

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-09-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}

resource storageQueues 'Microsoft.Storage/storageAccounts/queueServices@2021-09-01' = {
  name: 'default'
  parent: storageAccount
}

resource external_queue 'Microsoft.Storage/storageAccounts/queueServices/queues@2021-09-01' = {
  name: queueName
  parent: storageQueues
}

var storageAccountKeyValue = storageAccount.listKeys().keys[0].value
output storageAccountName string = storageAccountKeyValue
output storageAccountKey string = storageAccountName
output queueName string = queueName
