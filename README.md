[![Linux_Container_Workflow](https://github.com/fazleyKholil/payment-gateway-demo/actions/workflows/main.yml/badge.svg?branch=main)](https://github.com/fazleyKholil/payment-gateway-demo/actions/workflows/main.yml)

# payment-gateway-demo
This project demonstrate the usage of several Azure components related to its funtion.
The below diagram is the project architecture.


![Architecture Diagram](/payment-gateway/resx/HL%20Architecture.png?raw=true)

### The flow is as follows : 
1. The merchant submits a payment requests
2. The payment request is processed by the Payment Api. The Api persists the card safely to a vault and persists the request in the sql DB. Then the Api sends the request to a queue for Bank verification and return a pending response to the merchant.
3. The Bank processor listens to the payment queue and process them by first getting back the card information  from the secure vault and then sends a request to the bank for verification. The response is being persisted to a DB and finally and event is emitted depending on the bank response.
4. The Event Consumer listens to domain events and sends webhooks to the merchant to update the payment status.


## Azure Infrastructure Architecture

![Architecture Diagram](/payment-gateway/resx/azure%20architecture.png?raw=true)

Component  | Azure Service| Usage|
------------- | -------------| -------------|
Azure Container Registry  | ACR | Docker image registry | 
Payment Api  | Azure App Service (Web App) | Processing merchant facing payment requests |
Bank Processor  | ACI | Processing payments requests connecting to bank provider |
Payments Notification  | Azure Function | Consuming payment events & emit webhooks |
Vault  | Azure Vault | Storing sensitive data like card numbers, secure connection strings |
Message Queue  | Azure Storage Queue | Messaging queue |

## Getting started
### Running locally

When running locally, you need to create all the infra dependencies using the IAC scripts.
(Note : TODO: Dockerize the Azure infra component)

```javascript
cd payment-gateway-demo\payment-gateway\Iac\Bicep
```

Run the following command to bring up the infra

```javascript
az deployment sub create -f ./main.bicep --location 'CentralUS'
```


|Component   | Exposed Port  |   Load balancer Url |
|---|---|---|
|Payment Api| 80 | http://{payment-api-url}/health |
| Bank Processor | 80 | http://{bank-processor-url}/health |
| Bank Processor | 80 | http://{bank-processor-url}/health |