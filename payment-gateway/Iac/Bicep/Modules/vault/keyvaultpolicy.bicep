@description('Name of the KeyVault resource ex. kv-myservice.')
param keyVaultResourceName string
@description('Principal Id of the Azure resource (Managed Identity).')
param principalId string
@description('Assigned permissions for Principal Id (Managed Identity)')
param keyVaultPermissions object
@allowed([
  'add'
  'remove'
  'replace'
])
@description('Policy action.')
param policyAction string

resource keyVault 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: keyVaultResourceName
  resource keyVaultPolicies 'accessPolicies' = {
    dependsOn:[
      keyVault
    ]  
    name: policyAction
    properties: {    
      accessPolicies: [
        {
          objectId: principalId
          permissions: keyVaultPermissions
          tenantId: subscription().tenantId
        }
      ]
    }
  }  
}
