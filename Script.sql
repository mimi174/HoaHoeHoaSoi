CREATE TABLE ProductWishlist(
	Id int IDENTITY(1,1) PRIMARY KEY,
	ProductId int not null,
	UserId int not null,
	FOREIGN KEY (ProductId) REFERENCES Products(Id),
	FOREIGN KEY (UserId) REFERENCES UserInfo(Id)
)