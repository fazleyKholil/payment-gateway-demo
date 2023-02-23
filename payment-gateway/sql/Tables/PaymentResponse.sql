CREATE TABLE PaymentResponse
(
    PaymentResponseId bigint IDENTITY(1,1) PRIMARY KEY NOT null,
    PaymentId bigint,
    Response          varchar(100),
    CreatedDate   DATETIME
)