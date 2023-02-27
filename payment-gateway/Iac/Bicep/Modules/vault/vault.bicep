param keyVaultResourceName string
param resourceGroupName string
param principalId string

resource keyVault 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: keyVaultResourceName
  scope: resourceGroup(subscription().subscriptionId, resourceGroupName) 
}

output vaultUri string = keyVault.properties.vaultUri
