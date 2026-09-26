----create database [Trade2]
----go
--use [Trade2]
--go

--Справочник
--create table [Ed_iz]
--(
--	Ed_iz_ID int primary key identity(1,1),
--	Ed_Name nvarchar(100) not null,
--	Crat_Name nvarchar(15) not null
--)
--go

--create table [Status_zakaz]
--(
--	Status_zakaz_ID   int primary key identity(1,1),
--	Status_zakaz_Name nvarchar(100) not null
--)
--go

--create table [Nomenclator]
--(
--	Nomenclator_ID int primary key identity(1,1),
--	Nomenclator_Name nvarchar(700)  null,
--	Nomenclator_Tip nvarchar(100)  null,
--	Nomenclator_ed_Izm int,

--	constraint FK_Nomenclator_Ed_iz foreign key (Nomenclator_ed_Izm) references [Ed_iz](Ed_iz_ID)
--)
--go

--create table [Contragent]
--(
--	Contragent_ID int primary key identity(1,1),
--	Contragent_Name nvarchar(200) not null,
--	Contragent_INN nvarchar(12) null,
--	Contragent_Adres nvarchar(500) null,
--	Contragent_Telefon nvarchar(25) null,
--	Contragent_Tip nvarchar(100) not null,

--	constraint CHK_Contragent_Tip check (Contragent_Tip in (N'Покупатель', N'Поставщик'))
--)
--go
--create table [Product]
--(
--	Product_ID int primary key identity(1,1),
--	Product_Nomenclator int not null,
--	Product_Article nvarchar(50) null,
--	Product_Length int null,
--	Product_Width int null,
--	Product_Height int null,
--	Product_Weight decimal(10,2) null,

--	constraint FK_Product_Nomenclator foreign key (Product_Nomenclator) references [Nomenclator](Nomenclator_ID),
--	constraint CHK_Product_Length check (Product_Length is null or Product_Length > 0),
--	constraint CHK_Product_Width  check (Product_Width  is null or Product_Width  > 0),
--	constraint CHK_Product_Height check (Product_Height is null or Product_Height > 0),
--	constraint CHK_Product_Weight check (Product_Weight is null or Product_Weight > 0)
--)
--go
------Цены
--create table [Price]
--(
--	Nom_id int not null,
--	Price_Price decimal(12,2) not null,
--	Price_Data date not null,

--	constraint PK_Price primary key (Nom_id, Price_Data),
--	constraint FK_Price_Nomenclator foreign key (Nom_id) references [Nomenclator](Nomenclator_ID),
--	constraint CHK_Price_Price check (Price_Price >= 0)
--)
--go
------Спецификация
--create table [Spec]
--(
--	Spec_ID int primary key identity(1,1),
--	Spec_Prod_ID int not null,
--	Spec_Version int not null,
--	Spec_DateUtv date null,
--	Spec_Utverdil nvarchar(200) null,

--	constraint FK_Spec_Product foreign key (Spec_Prod_ID) references [Product](Product_ID),
--	constraint CHK_Spec_Version check (Spec_Version > 0),
--	constraint CHK_Spec_DateUtv check (Spec_DateUtv is null or Spec_DateUtv <= CURRENT_TIMESTAMP)
--)
--go

--create table [Spec_Materials]
--(
--	SpecMat_Spec_ID int not null,
--	SpecMat_Nom_ID int not null,
--	SpecMat_Quantity decimal(12,3) null,

--	constraint PK_Spec_Materials primary key (SpecMat_Spec_ID, SpecMat_Nom_ID),
--	constraint FK_SpecMat_Spec foreign key (SpecMat_Spec_ID) references [Spec](Spec_ID),
--	constraint FK_SpecMat_Nomenclator foreign key (SpecMat_Nom_ID) references [Nomenclator](Nomenclator_ID)
--)
--go

--create table [Spec_Operations]
--(
--	SpecOp_Spec_ID int not null,
--	SpecOp_Nom_ID int not null,
--	SpecOp_NormTime decimal(6,2) not null,
--	SpecOp_Quantity int not null,

--	constraint PK_Spec_Operations primary key (SpecOp_Spec_ID, SpecOp_Nom_ID),
--	constraint FK_SpecOp_Spec foreign key (SpecOp_Spec_ID) references [Spec](Spec_ID),
--	constraint FK_SpecOp_Nomenclator foreign key (SpecOp_Nom_ID) references [Nomenclator](Nomenclator_ID),
--	constraint CHK_SpecOp_NormTime check (SpecOp_NormTime > 0),
--	constraint CHK_SpecOp_Quantity check (SpecOp_Quantity > 0)
--)
----go
------Заказы покупателей
--create table [Customer_Order]
--(
--	CustOrd_ID int primary key identity(1,1),
--	CustOrd_Date date not null default CURRENT_TIMESTAMP,
--	CustOrd_Contragent int not null,
--	CustOrd_Executor nvarchar(200) null,

--	constraint FK_CustOrd_Contragent foreign key (CustOrd_Contragent) references [Contragent](Contragent_ID),
--	constraint CHK_CustOrd_Date check (CustOrd_Date >= '2020-01-01')
--)
--go

--create table [Customer_Order_Rows]
--(
--	CustOrdRow_Order_ID int not null,
--	CustOrdRow_Number int not null,
--	CustOrdRow_Product int not null,
--	CustOrdRow_Quantity int not null,
--	CustOrdRow_Price decimal(12,2) not null,
--	CustOrdRow_Discount decimal(12,2) null,

--	constraint PK_Customer_Order_Rows primary key (CustOrdRow_Order_ID, CustOrdRow_Number),
--	constraint FK_CustOrdRow_Order foreign key (CustOrdRow_Order_ID) references [Customer_Order](CustOrd_ID),
--	constraint FK_CustOrdRow_Product foreign key (CustOrdRow_Product) references [Product](Product_ID),
--	constraint CHK_CustOrdRow_Number   check (CustOrdRow_Number > 0),
--	constraint CHK_CustOrdRow_Quantity check (CustOrdRow_Quantity > 0),
--	constraint CHK_CustOrdRow_Price    check (CustOrdRow_Price >= 0),
--	constraint CHK_CustOrdRow_Discount check (CustOrdRow_Discount is null or CustOrdRow_Discount >= 0)
--)
--go
------Заказы на производство
--create table [Production_Order]
--(
--	ProdOrd_ID int primary key identity(1,1),
--	ProdOrd_DateStart date null,
--	ProdOrd_Division nvarchar(200) null,
--	ProdOrd_CustOrder int not null,
--	ProdOrd_Status int not null,

--	constraint FK_ProdOrd_CustomerOrder foreign key (ProdOrd_CustOrder) references [Customer_Order](CustOrd_ID),
--	constraint FK_ProdOrd_Status foreign key (ProdOrd_Status) references [Status_zakaz](Status_zakaz_ID),
--	constraint CHK_ProdOrd_DateStart check (ProdOrd_DateStart is null or ProdOrd_DateStart >= '2020-01-01')
--)
--go

--create table [Production_Order_Products]
--(
--	ProdOrdProd_Order_ID int not null,
--	ProdOrdProd_Product int not null,
--	ProdOrdProd_Quantity int not null,

--	constraint PK_Production_Order_Products primary key (ProdOrdProd_Order_ID, ProdOrdProd_Product),
--	constraint FK_ProdOrdProd_Order foreign key (ProdOrdProd_Order_ID) references [Production_Order](ProdOrd_ID),
--	constraint FK_ProdOrdProd_Product foreign key (ProdOrdProd_Product) references [Product](Product_ID),
--	constraint CHK_ProdOrdProd_Quantity check (ProdOrdProd_Quantity > 0)
--)
--go

--create table [Production_Order_Materials]
--(
--	ProdOrdMat_Order_ID int not null,
--	ProdOrdMat_Nom_ID int not null,
--	ProdOrdMat_Quantity decimal(12,3) not null,

--	constraint PK_Production_Order_Materials primary key (ProdOrdMat_Order_ID, ProdOrdMat_Nom_ID),
--	constraint FK_ProdOrdMat_Order foreign key (ProdOrdMat_Order_ID) references [Production_Order](ProdOrd_ID),
--	constraint FK_ProdOrdMat_Nomenclator foreign key (ProdOrdMat_Nom_ID) references [Nomenclator](Nomenclator_ID),
--	constraint CHK_ProdOrdMat_Quantity check (ProdOrdMat_Quantity > 0)
--)
--go

--create table [Production_Order_Operations]
--(
--	ProdOrdOp_Order_ID int not null,
--	ProdOrdOp_Nom_ID int not null,
--	ProdOrdOp_Quantity int not null,

--	constraint PK_Production_Order_Operations primary key (ProdOrdOp_Order_ID, ProdOrdOp_Nom_ID),
--	constraint FK_ProdOrdOp_Order foreign key (ProdOrdOp_Order_ID) references [Production_Order](ProdOrd_ID),
--	constraint FK_ProdOrdOp_Nomenclator foreign key (ProdOrdOp_Nom_ID) references [Nomenclator](Nomenclator_ID),
--	constraint CHK_ProdOrdOp_Quantity check (ProdOrdOp_Quantity > 0)
--)
--go

--use [Trade2]
--go

---- Роли
--create table [Role]
--(
--    Role_ID int primary key,
--    Role_Name nvarchar(100) not null,

--    constraint UQ_Role_Name unique (Role_Name)
--)
--go

---- Пользователи
--create table [User]
--(
--    User_ID int primary key,
--    User_Role int not null,
--    User_Surname nvarchar(100) not null,
--    User_Name nvarchar(100) not null,
--    User_Patronymic nvarchar(100) null,
--    User_Login nvarchar(50) not null,
--    User_Password nvarchar(50) not null,
--    User_Status     bit not null 
--        constraint DF_User_Status default 0,

--    constraint UQ_User_Login unique (User_Login),
--    constraint FK_User_Role 
--        foreign key (User_Role) references [Role](Role_ID),
--    constraint CHK_User_Status check (User_Status in (0, 1))
--)
--go

go

select 
    co.CustOrd_ID                as Номер_заказа,
    co.CustOrd_Date              as Дата,
    sum(
        cor.CustOrdRow_Quantity * (
            isnull(mat.MaterialCost, 0) 
          + isnull(op.OperationCost, 0)
        )
    )                            as Полная_стоимость
from [Customer_Order] co
join [Customer_Order_Rows] cor 
    on cor.CustOrdRow_Order_ID = co.CustOrd_ID

left join (
    select 
        s.Spec_Prod_ID                                as Product_ID,
        sum(sm.SpecMat_Quantity * pr.Price_Price)     as MaterialCost
    from [Spec] s
    join [Spec_Materials] sm on sm.SpecMat_Spec_ID = s.Spec_ID
    join [Price] pr          on pr.Nom_id = sm.SpecMat_Nom_ID
    group by s.Spec_Prod_ID
) mat on mat.Product_ID = cor.CustOrdRow_Product

left join (
    select 
        s.Spec_Prod_ID                                as Product_ID,
        sum(so.SpecOp_NormTime * pr.Price_Price)      as OperationCost
    from [Spec] s
    join [Spec_Operations] so on so.SpecOp_Spec_ID = s.Spec_ID
    join [Price] pr           on pr.Nom_id = so.SpecOp_Nom_ID
    group by s.Spec_Prod_ID
) op on op.Product_ID = cor.CustOrdRow_Product

group by co.CustOrd_ID, co.CustOrd_Date
go