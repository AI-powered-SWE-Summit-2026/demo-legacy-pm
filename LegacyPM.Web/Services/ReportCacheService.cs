using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LegacyPM.Core;
using LegacyPM.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LegacyPM.Web.Services
{
    public class ReportCacheService
    {
        private readonly ProjectDbContext _dbContext;

        public ReportCacheService(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public byte[] SerializeReportData(object data)
        {
            AppContext.SetSwitch("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", true);
            var formatter = new BinaryFormatter();
            using var ms = new MemoryStream();
#pragma warning disable SYSLIB0011
            formatter.Serialize(ms, data);
#pragma warning restore SYSLIB0011
            return ms.ToArray();
        }

        public T DeserializeReportData<T>(byte[] data)
        {
            AppContext.SetSwitch("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", true);
            var formatter = new BinaryFormatter();
            using var ms = new MemoryStream(data);
#pragma warning disable SYSLIB0011
            return (T)formatter.Deserialize(ms);
#pragma warning restore SYSLIB0011
        }

        public Report GetReport(int id)
        {
            return _dbContext.Reports.Include(r => r.Project).FirstOrDefaultAsync(r => r.Id == id).Result;
        }
    }
}