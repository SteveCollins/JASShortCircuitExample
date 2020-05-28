drop proc if exists jas.api_product_post;
go
create proc jas.api_product_post
  @body     				nvarchar(max),
  @id		    			bigint output,
  @response					nvarchar(max) output
as
set nocount on;
set xact_abort on;

begin transaction
begin try
    /* insert row taking value from input json */
    insert jas.products(product_name, created_dt) values(json_value(@body, N'strict $.product_name'), sysutcdatetime())
    
    /* return id value # to indicate insert succeeded.  Return generic 'Ok' reply message */
    select 
      @id=cast(scope_identity() as bigint),
      @response=(select N'Ok' reply_message for json path, without_array_wrapper);		

	commit transaction;
end try
begin catch
    /* return id value 0 to indicate insert failed.  Return error message from system function */
	select 
      @id=cast(0 as bigint),
      @response=(select error_message() [error_message] for json path, without_array_wrapper);

    rollback transaction;  
end catch

set xact_abort off;
set nocount off;
go

--select * from jas.products

declare
  @output_id                bigint,
  @output_response          nvarchar(max);

exec jas.api_product_post 
    @body='{ "product_name": "Phone 1" }',
    @id=@output_id output,
    @response=@output_response output;

if @output_id>0
    begin
        print (concat('The id created by the procedure: ', cast(@output_id as varchar(12))));
        print (@output_response);
    end
else
    print (concat('The error message returned by the procedure: ', @output_response));



