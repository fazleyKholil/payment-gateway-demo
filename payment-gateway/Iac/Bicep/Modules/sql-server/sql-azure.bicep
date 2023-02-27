
param applicationName string
param environment string = 'dev'
param instanceNumber string = '001'
param location string
param tags object
param serverName string = 'sql-${applicationName}-${environment}-${instanceNumber}'
param sqlDBName string = applicationName
param administratorLogin string = 'sql${replace(applicationName, '-', '')}root'
@secure()
param administratorPassword string 

resource sqlServer 'Microsoft.Sql/servers@2020-02-02-preview' = {
  name: serverName
  location: location
  tags: tags
  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorPassword
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2021-05-01-preview' = {
  parent: sqlServer
  name: sqlDBName
  location: location
  tags: tags
  sku: {
    name: 'GP_S_Gen5_1'
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    autoPauseDelay: 60
    minCapacity: '0.5'
  }
}

output db_connection_string string = 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${sqlDBName};Persist Security Info=False;User ID=${administratorLogin};Password=${administratorPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
output db_url string = sqlServer.properties.fullyQualifiedDomainName
output db_username string = administratorLogin
