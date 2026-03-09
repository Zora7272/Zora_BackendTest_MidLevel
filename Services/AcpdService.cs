using System.Data;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Data.Common;

namespace WebApplication1.Services
{
    public class AcpdService : IAcpdService
    {
        private readonly AppDbContext _db;

        public AcpdService(AppDbContext db)
        {
            _db = db;
        }

        
        public async Task<List<MyOfficeAcpd>> GetAllAsync()
        {
            return await _db.MyOfficeACPD
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<MyOfficeAcpd?> GetByIdAsync(string id)
        {
            var entity = await _db.MyOfficeACPD
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.ACPD_SID == id);
            return entity;
        }

        public async Task<(MyOfficeAcpd Entity, string? LogJson)> CreateAsync(MyOfficeAcpd entity)
        {
            var conn = _db.Database.GetDbConnection();
            await conn.OpenAsync();
            // begin a DbTransaction on the underlying connection and let EF use it
            var dbTransaction = await conn.BeginTransactionAsync();
            _db.Database.UseTransaction(dbTransaction);
            string? outJson = null;
            try
            {
                // Generate a new SID using NEWSID stored procedure
                var sid = await GenerateNewsIdAsync(conn, dbTransaction, "MyOffice_ACPD");
                entity.ACPD_SID = sid;

                // persist entity
                _db.MyOfficeACPD.Add(entity);
                await _db.SaveChangesAsync();

                // call usp_AddLog via helper
                var actionJson = $"{{ \"ACPD_SID\": \"{sid}\", \"Action\": \"Insert\" }}";
                outJson = await CallAddLogAsync(conn, dbTransaction, "NEWSID", "CreateWithNewsId", actionJson);

                await dbTransaction.CommitAsync();
                return (entity, outJson);
            }
            catch
            {
                try { await dbTransaction.RollbackAsync(); } catch { }
                throw;
            }
            finally
            {
                try { conn.Close(); } catch { }
            }
        }

        public async Task UpdateAsync(string id, MyOfficeAcpd newMyOfficeAcpd)
        {
            var myOfficeAcpd = await _db.MyOfficeACPD
                            .FirstOrDefaultAsync(x => x.ACPD_SID == id);
            if (myOfficeAcpd == null) {
                throw new KeyNotFoundException();
            }
            myOfficeAcpd.ACPD_Cname = newMyOfficeAcpd.ACPD_Cname;
            myOfficeAcpd.ACPD_Ename = newMyOfficeAcpd.ACPD_Ename;
            myOfficeAcpd.ACPD_Sname = newMyOfficeAcpd.ACPD_Sname;
            myOfficeAcpd.ACPD_Email = newMyOfficeAcpd.ACPD_Email;
            myOfficeAcpd.ACPD_Status = newMyOfficeAcpd.ACPD_Status;
            myOfficeAcpd.ACPD_Stop = newMyOfficeAcpd.ACPD_Stop;
            myOfficeAcpd.ACPD_StopMemo = newMyOfficeAcpd.ACPD_StopMemo;
            myOfficeAcpd.ACPD_LoginID = newMyOfficeAcpd.ACPD_LoginID;
            myOfficeAcpd.ACPD_LoginPWD = newMyOfficeAcpd.ACPD_LoginPWD;
            myOfficeAcpd.ACPD_Memo = newMyOfficeAcpd.ACPD_Memo;
            myOfficeAcpd.ACPD_NowDateTime = newMyOfficeAcpd.ACPD_NowDateTime;
            myOfficeAcpd.ACPD_NowID = newMyOfficeAcpd.ACPD_NowID;
            myOfficeAcpd.ACPD_UPDDateTime = newMyOfficeAcpd.ACPD_UPDDateTime;
            myOfficeAcpd.ACPD_UPDID = newMyOfficeAcpd.ACPD_UPDID;

            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var myOfficeAcpd = await _db.MyOfficeACPD
                            .FirstOrDefaultAsync(x => x.ACPD_SID == id);
            if (myOfficeAcpd == null) {
                throw new KeyNotFoundException();
            }
            _db.MyOfficeACPD.Remove(myOfficeAcpd);
            await _db.SaveChangesAsync();
        }
        private static async Task<string> GenerateNewsIdAsync(DbConnection conn, DbTransaction dbTransaction, string tableName)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "NEWSID";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Transaction = dbTransaction;

            var pTable = cmd.CreateParameter();
            pTable.ParameterName = "@TableName";
            pTable.DbType = DbType.String;
            pTable.Value = tableName;
            cmd.Parameters.Add(pTable);

            var pOut = cmd.CreateParameter();
            pOut.ParameterName = "@ReturnSID";
            pOut.DbType = DbType.String;
            pOut.Size = 20;
            pOut.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(pOut);

            await cmd.ExecuteNonQueryAsync();
            var sid = pOut.Value?.ToString();
            if (string.IsNullOrEmpty(sid))
            {
                throw new Exception("Failed to generate SID from NEWSID stored procedure.");
            }

            return sid!;
        }

        private static async Task<string?> CallAddLogAsync(DbConnection conn, DbTransaction dbTransaction, string spName, string exProgram, string actionJson)
        {
            using var logCmd = conn.CreateCommand();
            logCmd.CommandText = "usp_AddLog";
            logCmd.CommandType = CommandType.StoredProcedure;
            logCmd.Transaction = dbTransaction;

            var pReadId = logCmd.CreateParameter();
            pReadId.ParameterName = "@_InBox_ReadID";
            pReadId.DbType = DbType.Byte;
            pReadId.Value = (byte)0;
            logCmd.Parameters.Add(pReadId);

            var pSpName = logCmd.CreateParameter();
            pSpName.ParameterName = "@_InBox_SPNAME";
            pSpName.DbType = DbType.String;
            pSpName.Value = spName;
            logCmd.Parameters.Add(pSpName);

            var pGroup = logCmd.CreateParameter();
            pGroup.ParameterName = "@_InBox_GroupID";
            pGroup.DbType = DbType.Guid;
            pGroup.Value = Guid.NewGuid();
            logCmd.Parameters.Add(pGroup);

            var pExProg = logCmd.CreateParameter();
            pExProg.ParameterName = "@_InBox_ExProgram";
            pExProg.DbType = DbType.String;
            pExProg.Value = exProgram;
            logCmd.Parameters.Add(pExProg);

            var pActionJson = logCmd.CreateParameter();
            pActionJson.ParameterName = "@_InBox_ActionJSON";
            pActionJson.DbType = DbType.String;
            pActionJson.Value = actionJson;
            logCmd.Parameters.Add(pActionJson);

            var pOutLog = logCmd.CreateParameter();
            pOutLog.ParameterName = "@_OutBox_ReturnValues";
            pOutLog.DbType = DbType.String;
            pOutLog.Size = -1; //代表 nvarchar 大小為 MAX
            pOutLog.Direction = ParameterDirection.Output;
            logCmd.Parameters.Add(pOutLog);

            await logCmd.ExecuteNonQueryAsync();
            return pOutLog.Value?.ToString();
        }

    }
}
