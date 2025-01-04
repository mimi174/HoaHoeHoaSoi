ALTER TABLE Ordered
ADD PaymentMethod int default 1

ALTER TABLE Ordered
ADD PaymentNote nvarchar(max) null

ALTER TABLE Ordered
ADD ResultCode varchar(100) null

EXEC sp_RENAME 'Ordered.MomoOrderId' , 'PaymentOrderId', 'COLUMN'
