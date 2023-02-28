@description('Specifies the location for resources.')
param location string = 'CentralUS'
param environment string = 'dev'
param vaultName string = 'vault-test-payment-2'

// =========== main.bicep ===========

// Setting target scope
targetScope = 'subscription'

var defaultTags = {
  environment: environment
}


// Creating resource group
resource rg 'Microsoft.Resources/resourceGroups@2022-09-01' = {
  name: 'fazley-payment-gateway-rg-1'
  location: location
}

// Creating application insights for instrumentation
module instrumentation 'modules/application-insights/app-insights.bicep' = {
  name: 'instrumentation'
  scope: resourceGroup(rg.name)
  params: {
    location: location
    applicationName: 'payment-gateway-demo'
    environment: 'dev'
    instanceNumber: '001'
    resourceTags: defaultTags
  }
}

// Creating database
module database 'modules/sql-server/sql-azure.bicep' = {
  name: 'sqlDb'
  scope: resourceGroup(rg.name)
  params: {
    location: 'eastus'
    applicationName: 'payment-gateway-demo-db1'
    administratorPassword: 'Objectivity@123'
    environment: environment
    tags: defaultTags
    instanceNumber: '001'
  }
}

// Creating storage queue
module stg './Modules/storage/storage.bicep' = {
  name: 'pgstorageaccountfaz1'
  scope: rg  
  params: {
    storageAccountName: 'pgstorageaccountfaz1'
    queueName: 'payment-test-queue'
     location: location
  }
}


var PaymentApiEnvironmentVariables = [
  {
    name: 'Queueing__QueueConnection'
    value: 'DefaultEndpointsProtocol=https;AccountName=${stg.outputs.storageAccountName};AccountKey=${stg.outputs.storageAccountKey};EndpointSuffix=core.windows.net'
  }
  {
    name: 'Queueing__QueueName'
    value: stg.outputs.queueName
  }
  {
    name: 'Queueing__BatchCount'
    value: 5
  }
  {
    name: 'KeyVault__VaultUri'
    value: 'https://${vaultName}.vault.azure.net'
  }
  {
    name: 'DbConnection__ConnectionString'
    value: database.outputs.db_connection_string
  }
  {
    name: 'ApplicationInsights__InstrumentationKey'
    value: instrumentation.outputs.appInsightsInstrumentationKey
  }
]


module webApp 'modules/app-service/app-service.bicep' = {
  name: 'webApp'
  scope: rg
  params: {
    location: location
    applicationName: 'payment-gateway-demo-api'
    environment: environment
    resourceTags: defaultTags
    instanceNumber: '001'
    environmentVariables: PaymentApiEnvironmentVariables
    imageName: 'fazleysharedcr.azurecr.io/paymentapi'
    containerRegistryName: 'fazleysharedcr'
    containerRegistryId: '/subscriptions/875307ef-d9a6-4807-a12c-be3587c3ea53/resourceGroups/SharedRg/providers/Microsoft.ContainerRegistry/registries/fazleySharedCr'
    containerRegistryApiVersion: '2023-01-01-preview'
  }
}

//Creating Vault
module vault 'modules/vault/vault.bicep' = {
  name: vaultName
  scope: rg
  params: {
    keyVaultName: vaultName
    location: location
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: false
    keysPermissions: [
      'list'
      'get'
      'create'
    ]
    secretsPermissions: [
      'list'
      'get'
      'set'
    ]
    tenantId: subscription().tenantId
    skuName:  'standard'
    objectId: '557dda39-c039-4445-a970-2f4ff987f88d'
  }
}

module keyVault 'Modules/vault/keyvaultpolicy.bicep' = {
  dependsOn: [
    webApp
  ]
  scope: rg
  name: 'keyVault'
  params: {
      keyVaultResourceName: vaultName
      principalId: webApp.outputs.principalId
  }
}

// Creating Bank Processor instace
module bankProcessorAci './Modules/container-instance/aci.bicep' = {
  name: 'bankProcessorAci1'
  scope: rg
  params: {
    name: 'bankprocessorcontainer1'
    location: location
    image: 'mcr.microsoft.com/azuredocs/aci-helloworld'
    port: 80
    cpuCores: 1
    memoryInGb: 1
    restartPolicy: 'Always'
    environmentVariables: PaymentApiEnvironmentVariables
  }
}

