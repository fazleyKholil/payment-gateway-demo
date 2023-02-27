@description('Specifies the location for resources.')
param location string = 'CentralUS'
param environment string = 'dev'

// =========== main.bicep ===========

// Setting target scope
targetScope = 'subscription'

var defaultTags = {
  environment: environment
}


// Creating resource group
resource rg 'Microsoft.Resources/resourceGroups@2022-09-01' = {
  name: 'fazley-payment-gateway-rg-4'
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
    applicationName: 'payment-gateway-demo-db'
    environment: environment
    tags: defaultTags
    instanceNumber: '001'
  }
}

// Creating storage queue
module stg './Modules/storage/storage.bicep' = {
  name: 'pgstorageaccountfaz'
  scope: rg  
  params: {
    storageAccountName: 'pgstorageaccountfaz'
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
    value: 'https://fazley-test-vault.vault.azure.net'
  }
  {
    name: 'DbConnection__ConnectionString'
    value: database.outputs.db_url
  }
  {
    name: 'ApplicationInsights__InstrumentationKey'
    value: instrumentation.outputs.appInsightsInstrumentationKey
  }
]


var applicationEnvironmentVariables = [
        {
          name: 'ApplicationInsights_InstrumentationKey'
          value: instrumentation.outputs.appInsightsInstrumentationKey
        }
        {
          name: 'DbConnection__ConnectionString'
          value: database.outputs.db_url
        }
  ]


// // Creating ACR for the project
// module bankProcessorAcr './Modules/container-registry/acr.bicep' = {
//   name: 'PaymentGatewayAcr'
//   scope: rg
//   params: {
//     location: location
//     acrName: 'PaymentGatewayAcr'
//     acrSku: 'Basic'
//   }
// }

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
    environmentVariables: [
      {
        name: 'DbConnection__ConnectionString'
        secureValue: 'Server=tcp:fazley-sql-server.database.windows.net,1433;Initial Catalog=fazley-test-db;Persist Security Info=False;User ID=fazley;Password=Objectivity@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
      }
    ]
  }
}

