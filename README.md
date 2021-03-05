# UserManagement_usingJwtAPI

1) Create this trigger below after run migration, update-database
  +) run: USE [UserManagement_usingJwt2] 
  +) run this code: 
            CREATE TRIGGER [dbo].[trg_userInsert]
            on [dbo].[Users]
            for insert
            as 
            begin
               set nocount on;
               insert into Profiles(Birthdate)
               values(GETDATE());

               declare @_Id int;
               set @_Id = @@IDENTITY;

               update Users
               set Profile_Id = @_Id
               where Id = (SELECT inserted.id FROM inserted)
            end
  
