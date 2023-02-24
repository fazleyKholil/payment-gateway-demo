@description('Specifies the location for resources.')
param location string = 'CentralUS'

// =========== main.bicep ===========

// Setting target scope
targetScope = 'subscription'

// Creating resource group
resource rg 'Microsoft.Resources/resourceGroups@2022-09-01' = {
  name: 'fazley-payment-gateway-rg-1'
  location: location
}


// Creating ACR for Payment Api
module paymentApiAcr './Modules/acr.bicep' = {
  name: 'PaymentApi'
  scope: rg
  params: {
    location: location
    acrName: 'PaymentApi'
    acrSku: 'Basic'
  }
}

// Creating ACR for Bank Processor
module bankProcessorAcr './Modules/acr.bicep' = {
  name: 'BankProcessor'
  scope: rg
  params: {
    location: location
    acrName: 'BankProcessor'
    acrSku: 'Basic'
  }
}

module paymentApiAci './Modules/aci.bicep' = {
  name: 'paymentApiAci'
  scope: rg
  params: {
    name: 'paymentapicontainer'
    location: location
    image: 'mcr.microsoft.com/azuredocs/aci-helloworld'
    port: 80
    cpuCores: 1
    memoryInGb: 1
    restartPolicy: 'Always'
    environmentVariables: [
      {
        //TODO: use secrets
        name: 'DbConnection__ConnectionString'
        secureValue: 'Server=tcp:fazley-sql-server.database.windows.net,1433;Initial Catalog=fazley-test-db;Persist Security Info=False;User ID=fazley;Password=Objectivity@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
      }
    ]
  }
}

module bankProcessorAci './Modules/aci.bicep' = {
  name: 'bankProcessorAci'
  scope: rg
  params: {
    name: 'bankprocessorcontainer'
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

