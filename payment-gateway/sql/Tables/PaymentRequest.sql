CREATE TABLE PaymentRequest
(
    PaymentId       bigint IDENTITY(1,1) PRIMARY KEY NOT null,
    TransactionId   varchar(20),
    Amount          decimal,
    CardNumber      VARCHAR(20),
    VaultId         VARCHAR(40),
    TransactionDate DATETIME,
    CreatedDate     DATETIME
)