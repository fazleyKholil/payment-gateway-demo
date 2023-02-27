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
  name: 'fazley-payment-gateway-rg-8'
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
  
// Creating ACR for Payment Api
module paymentApiAcr './Modules/container-registry/acr.bicep' = {
  name: 'PaymentApi'
  scope: rg
  params: {
    location: location
    acrName: 'PaymentApi'
    acrSku: 'Basic'
  }
}

// Creating ACR for Bank Processor
module bankProcessorAcr './Modules/container-registry/acr.bicep' = {
  name: 'BankProcessor'
  scope: rg
  params: {
    location: location
    acrName: 'BankProcessor'
    acrSku: 'Basic'
  }
}

module webApp 'modules/app-service/app-service.bicep' = {
  name: 'webApp'
  scope: rg
  params: {
    location: location
    applicationName: 'payment-gateway-demo-api'
    environment: environment
    resourceTags: defaultTags
    instanceNumber: '001'
    environmentVariables: applicationEnvironmentVariables
    
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




// // Deploying storage account using module
// module stg './Modules/storage.bicep' = {
//   name: 'storageDeployment'
//   scope: rg  
//   params: {
//     storageAccountName: 'fazbicepstorageacc'
//   }
// }

