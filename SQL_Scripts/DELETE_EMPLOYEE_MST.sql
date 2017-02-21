ALTER PROCEDURE [dbo].[DELETE_EMPLOYEE_MST]
(
	 @TEM_ID						INT
	,@TEM_MODIFIED_BY				INT
	,@TEM_MODIFIED_ON				DATETIME2
	,@RET_VAL						INT OUT
)
AS
BEGIN
	IF(@TEM_MODIFIED_BY = 0)
		SET @TEM_MODIFIED_BY = NULL

	UPDATE 
		TBL_EMPLOYEE_MST
	SET 
		IS_DELETED					=	1
		,MODIFIED_ON				=	@TEM_MODIFIED_ON
		,MODIFIED_BY				=	@TEM_MODIFIED_BY
	WHERE
		ID						=	@TEM_ID
			
	SET @RET_VAL = @TEM_ID
	
	--SELECT @RET_VAL
END

