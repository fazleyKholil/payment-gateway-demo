@description('Name of the KeyVault resource ex. kv-myservice.')
param keyVaultResourceName string

@description('Principal Id of the Azure resource (Managed Identity).')
param principalId string




resource accessPolicies 'Microsoft.KeyVault/vaults/accessPolicies@2019-09-01' = {
  name: '${keyVaultResourceName}/add'
  properties: {
    accessPolicies: [
      {
        objectId: principalId
        tenantId: subscription().tenantId
        permissions: {
          secrets: [
            'all'
          ]
          certificates: [
            'list'
          ]
        }
      }
    ]
  }
}
