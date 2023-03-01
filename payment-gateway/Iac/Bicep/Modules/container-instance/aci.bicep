
param name string 
param location string 
param containerImage string
param containerRegistrySubscriptionId string = subscription().subscriptionId
param containerRegistryResourceGroupName string = resourceGroup().name
param containerRegistryName string = 'paymentdemoregistry'
param environmentVariable array 

// Create identtiy
resource identity1 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview'  = {
  name: 'identityName'
  location: location
}

// Assign AcrPull permission
module roleAssignment 'container-registry-role-assignment.bicep' = {
  name: 'container-registry-role-assignment'
  scope: resourceGroup(containerRegistrySubscriptionId, containerRegistryResourceGroupName)
  params: {
    roleId: '7f951dda-4ed3-4680-a7ca-43fe172d538d' // AcrPull
    principalId: identity1.properties.principalId
    registryName: containerRegistryName
  }
}

// Get a reference to the container app environment
resource managedEnvironment 'Microsoft.App/managedEnvironments@2022-03-01'  existing = {
  name: 'managedEnvironment-fazleypaymentga-9226'
}


// create the container app
resource containerapp 'Microsoft.App/containerApps@2022-03-01' = {
  dependsOn:[
    roleAssignment
  ]
  name: name
  location: location
  identity: {
    type: 'SystemAssigned,UserAssigned'
    userAssignedIdentities: {
      '${identity1.id}': {}
    }
  }
  properties: {
    managedEnvironmentId: managedEnvironment.id
    configuration: {
      ingress: {
        external: true
        transport: 'auto'
        allowInsecure: true
        targetPort: 80
        traffic : [
          {
            weight: 100
            latestRevision: true
          }
      ]
      }
      registries: [
        {
          server: '${containerRegistryName}.azurecr.io'
          identity: identity1.id
        }
      ]
    }
    template: {
      containers: [
        {
          name: name
          image: '${containerRegistryName}.azurecr.io/${containerImage}'
          env: environmentVariable
        }
      ]
    }
  }
}
