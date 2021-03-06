select * from DocumentSequences
select * from DocumentNumbers

--delete from DocumentSequences
--delete from DocumentNumbers

declare @user_id nvarchar(450) = (select top 1 id from [dbo].[AspNetUsers] where UserName = 'marcin');

--invoice
declare @docSeqInvPK uniqueidentifier = NEWID();

insert into DocumentSequences
values (@docSeqInvPK, @user_id, getdate(), null, null, 1, 'invoice sequence', 'invoice sequence for 2018', 1, 1, 99999, '2018-01-01', '2019-01-01')

declare @docNumInvPK uniqueidentifier = NEWID();

insert into DocumentNumbers
values (@docNumInvPK, @user_id, getdate(), null, null, 1, 'invoice numbering', 'invoice numbering for 2018', 2, @docSeqInvPK, 'FV', '/')

--budget
declare @docSeqBudPK uniqueidentifier = NEWID();

insert into DocumentSequences
values (@docSeqBudPK, @user_id, getdate(), null, null, 1, 'budget sequence', 'budget sequence for 2018', 1, 1, 99999, '2018-01-01', '2019-01-01')

declare @docNumBudPK uniqueidentifier = NEWID();

insert into DocumentNumbers
values (@docNumBudPK, @user_id, getdate(), null, null, 1, 'budget numbering', 'budget numbering for 2018', 1, @docSeqBudPK, 'B', '/')
