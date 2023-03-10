@description('Specifies the location for resources.')
param location string = 'CentralUS'
param environment string = 'dev'
param resourceGroupName string = 'fazley-payment-gateway-rg-1'

// =========== main.bicep ===========

// Setting target scope
targetScope = 'subscription'

var defaultTags = {
  environment: environment
}


var PaymentApiEnvironmentVariables = [
  {
    name: 'Queueing__QueueConnection'
    value: 'DefaultEndpointsProtocol=https;AccountName=pgstorageaccountfaz1;AccountKey=13MaTPjcWNNVnsOrvJroNb6EWfbK12o04brIrZ+Us8fiN/iQiG2W00mSblZ/AMQAdBpbfDbkepmd+AStKbz0Gg==;EndpointSuffix=core.windows.net'
  }
  {
    name: 'Queueing__QueueName'
    value: 'payment-test-queue'
  }
  {
    name: 'Queueing__BatchCount'
    value: '5'
  }
  {
    name: 'KeyVault__VaultUri'
    value: 'https://vault-test-payment-2.vault.azure.net'
  }
  {
    name: 'DbConnection__ConnectionString'
    value: 'Server=tcp:sql-payment-gateway-demo-db1-dev-001.database.windows.net,1433;Initial Catalog=payment-gateway-demo-db1;Persist Security Info=False;User ID=sqlpaymentgatewaydemodb1root;Password=Objectivity@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
  }
  {
    name: 'ApplicationInsights__InstrumentationKey'
    value: '8d7037c7-d3ac-4846-8dda-343bfd741909'
  }
]




// Creating Bank Processor instace
module bankProcessorAci './Modules/container-instance/aci.bicep' = {
  name: 'bankProcessorContainerApp'
  scope: resourceGroup(resourceGroupName)
  params: {
    name: 'bankprocessorcontainerapp'
    location: location
    containerImage: 'bank-processor:3f22db6504f7ba66a144b1bc11d0d67d8bb0ba68'
    containerRegistryName: 'paymentdemoregistry'
    containerRegistryResourceGroupName:resourceGroupName
    containerRegistrySubscriptionId: subscription().subscriptionId
    environmentVariable: PaymentApiEnvironmentVariables
  }
}

