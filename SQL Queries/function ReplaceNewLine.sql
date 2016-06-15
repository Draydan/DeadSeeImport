USE [DeadSeaCatalogueDAL.ProductContext]
GO

/****** Object:  UserDefinedFunction [dbo].[ReplaceNewLine]    Script Date: 16.06.2016 0:10:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
ALTER FUNCTION [dbo].[ReplaceNewLine] 
(
	-- Add the parameters for the function here
	@text varchar(1024),
	@replacement varchar(1024)
)
RETURNS varchar(1024)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result varchar(1024)

	-- Add the T-SQL statements to compute the return value here
	SELECT @Result = 
	--set @Result = @text
	replace(replace(
	replace(@text, CHAR(13) + CHAR(10), @replacement)
	, CHAR(10), @replacement), 	CHAR(13), @replacement)

	-- Return the result of the function
	RETURN @Result
END

GO


