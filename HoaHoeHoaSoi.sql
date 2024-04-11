USE [master]
GO
/****** Object:  Database [HoaHoeHoaSoi]    Script Date: 4/11/2024 1:29:21 PM ******/
CREATE DATABASE [HoaHoeHoaSoi]
 
ALTER DATABASE [HoaHoeHoaSoi] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [HoaHoeHoaSoi].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [HoaHoeHoaSoi] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET ARITHABORT OFF 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET  DISABLE_BROKER 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET  MULTI_USER 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [HoaHoeHoaSoi] SET DB_CHAINING OFF 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [HoaHoeHoaSoi] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [HoaHoeHoaSoi] SET QUERY_STORE = OFF
GO
USE [HoaHoeHoaSoi]
GO
/****** Object:  Table [dbo].[Admin]    Script Date: 4/11/2024 1:29:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Admin](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Address] [nvarchar](50) NULL,
	[Phone] [varchar](15) NULL,
	[Username] [nvarchar](50) NULL,
	[Password] [nvarchar](50) NULL,
 CONSTRAINT [PK_Admin] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customer]    Script Date: 4/11/2024 1:29:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Address] [nvarchar](50) NULL,
	[Phone] [varchar](15) NULL,
	[Email] [nvarchar](50) NULL,
 CONSTRAINT [PK__Customer__3214EC07CEEF024F] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Feedback]    Script Date: 4/11/2024 1:29:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Feedback](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NULL,
	[Content] [nvarchar](50) NULL,
 CONSTRAINT [PK_Feedback] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Order_Line]    Script Date: 4/11/2024 1:29:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Order_Line](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Ordered_Id] [int] NOT NULL,
	[Products_Id] [int] NOT NULL,
	[Price] [float] NULL,
	[Quantity] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ordered]    Script Date: 4/11/2024 1:29:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ordered](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Customer_Id] [int] NOT NULL,
	[Date] [date] NULL,
	[Total] [float] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 4/11/2024 1:29:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NULL,
	[Price] [float] NULL,
	[Img] [image] NULL,
	[Quantity] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[Admin] ([Id], [Name], [Address], [Phone], [Username], [Password]) VALUES (1, N'An ', N'Ha Noi', N'0987213', N'Admin', N'1')
GO
SET IDENTITY_INSERT [dbo].[Customer] ON 

INSERT [dbo].[Customer] ([Id], [Name], [Address], [Phone], [Email]) VALUES (73, N'Truong An Do', NULL, N'0382123456', N'an@gmail.com')
INSERT [dbo].[Customer] ([Id], [Name], [Address], [Phone], [Email]) VALUES (74, N'An', N'Ha Noi', N'0123456789', NULL)
SET IDENTITY_INSERT [dbo].[Customer] OFF
GO
SET IDENTITY_INSERT [dbo].[Feedback] ON 

INSERT [dbo].[Feedback] ([Id], [CustomerId], [Content]) VALUES (28, 73, N'dsadsadasdasd')
SET IDENTITY_INSERT [dbo].[Feedback] OFF
GO
SET IDENTITY_INSERT [dbo].[Order_Line] ON 

INSERT [dbo].[Order_Line] ([Id], [Ordered_Id], [Products_Id], [Price], [Quantity]) VALUES (58, 43, 2, 10000, 5)
INSERT [dbo].[Order_Line] ([Id], [Ordered_Id], [Products_Id], [Price], [Quantity]) VALUES (59, 43, 3, 123, 3)
INSERT [dbo].[Order_Line] ([Id], [Ordered_Id], [Products_Id], [Price], [Quantity]) VALUES (60, 43, 4, 123, 4)
INSERT [dbo].[Order_Line] ([Id], [Ordered_Id], [Products_Id], [Price], [Quantity]) VALUES (61, 43, 8, 10, 1)
INSERT [dbo].[Order_Line] ([Id], [Ordered_Id], [Products_Id], [Price], [Quantity]) VALUES (62, 43, 9, 204, 2)
SET IDENTITY_INSERT [dbo].[Order_Line] OFF
GO
SET IDENTITY_INSERT [dbo].[Ordered] ON 

INSERT [dbo].[Ordered] ([Id], [Customer_Id], [Date], [Total]) VALUES (43, 74, CAST(N'2024-04-11' AS Date), 51279)
SET IDENTITY_INSERT [dbo].[Ordered] OFF
GO
SET IDENTITY_INSERT [dbo].[Products] ON 

INSERT [dbo].[Products] ([Id], [Name], [Price], [Img], [Quantity]) VALUES (2, N'dfdsf', 10000, NULL, 1)
INSERT [dbo].[Products] ([Id], [Name], [Price], [Img], [Quantity]) VALUES (3, N'fsdfs', 123, NULL, 2)
INSERT [dbo].[Products] ([Id], [Name], [Price], [Img], [Quantity]) VALUES (4, N'dfdsf', 123, NULL, 3)
INSERT [dbo].[Products] ([Id], [Name], [Price], [Img], [Quantity]) VALUES (8, N'fffffffffffffffff', 10, NULL, 4)
INSERT [dbo].[Products] ([Id], [Name], [Price], [Img], [Quantity]) VALUES (9, N'aaaaaaaaaaaaaaaaaaa', 204, NULL, 5)
SET IDENTITY_INSERT [dbo].[Products] OFF
GO
ALTER TABLE [dbo].[Feedback]  WITH CHECK ADD  CONSTRAINT [FK_Feedback_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([Id])
GO
ALTER TABLE [dbo].[Feedback] CHECK CONSTRAINT [FK_Feedback_Customer]
GO
ALTER TABLE [dbo].[Order_Line]  WITH CHECK ADD  CONSTRAINT [FK_Order_Line_Ordered] FOREIGN KEY([Ordered_Id])
REFERENCES [dbo].[Ordered] ([Id])
GO
ALTER TABLE [dbo].[Order_Line] CHECK CONSTRAINT [FK_Order_Line_Ordered]
GO
ALTER TABLE [dbo].[Order_Line]  WITH CHECK ADD  CONSTRAINT [FK_Order_Line_Products] FOREIGN KEY([Products_Id])
REFERENCES [dbo].[Products] ([Id])
GO
ALTER TABLE [dbo].[Order_Line] CHECK CONSTRAINT [FK_Order_Line_Products]
GO
ALTER TABLE [dbo].[Ordered]  WITH CHECK ADD  CONSTRAINT [FK__Ordered__Custome__267ABA7A] FOREIGN KEY([Customer_Id])
REFERENCES [dbo].[Customer] ([Id])
GO
ALTER TABLE [dbo].[Ordered] CHECK CONSTRAINT [FK__Ordered__Custome__267ABA7A]
GO
USE [master]
GO
ALTER DATABASE [HoaHoeHoaSoi] SET  READ_WRITE 
GO
