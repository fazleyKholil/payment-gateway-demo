
param name string 
param location string 
param image string
param port int
param cpuCores int
param memoryInGb int

@description('The behavior of Azure runtime if container has stopped.')
@allowed([
  'Always'
  'Never'
  'OnFailure'
])
param restartPolicy string = 'Always'

param environmentVariables array

resource containerGroup 'Microsoft.ContainerInstance/containerGroups@2022-09-01' = {
  name: name
  location: location
  properties: {
    containers: [
      {
        name: name
        properties: {
          image: image
          ports: [
            {
              port: port
              protocol: 'TCP'
            }
          ]
          environmentVariables: environmentVariables
          resources: {
            requests: {
              cpu: cpuCores
              memoryInGB: memoryInGb
            }
          }
        }
      }
    ]
    osType: 'Linux'
    restartPolicy: restartPolicy
    ipAddress: {
      type: 'Public'
       dnsNameLabel: 'pg-${name}'
      ports: [
        {
          port: port
          protocol: 'TCP'
        }
      ]
    }
  }
}


output containerIPv4Address string = containerGroup.properties.ipAddress.ip
