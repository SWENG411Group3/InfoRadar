using CsvHelper;
using System.Data.Common;
using System.Dynamic;

namespace InformationRadarCore.Models
{
    public class GenericRecordTable
    {
        public class RecordRow
        {
            public int Id { get; set; }
            public IList<object> Values { get; set; }
            public DateTime Created { get; set; }
        }

        public class JsonRecord
        {
            public IDictionary<string, object> Values { get; set; }
            public DateTime Created { get; set; }
        }

        public IEnumerable<DbColumn> Columns { get; set; }
        public IList<RecordRow> Rows { get; set; } = new List<RecordRow>();

        public IEnumerable<JsonRecord> ToJson()
        {
            return Rows.Select(processJsonRow);
        }
        
        public IEnumerable<dynamic> ToCsv()
        {
            return Rows.Select(processCsvRow);
        }

        private object processCsvRow(RecordRow row)
        {
            dynamic values = new ExpandoObject();
            values.Record_Created = row.Created;

            var dictValue = values as IDictionary<string, object>;
            foreach (var (col, val) in Columns.Skip(2).Zip(row.Values))
            {
                dictValue.Add(col.ColumnName.Substring(6), val);
            }

            return values;
        }

        private JsonRecord processJsonRow(RecordRow row)
        {
            return new JsonRecord()
            { 
                Created = row.Created,
                Values = Columns.Skip(2).Zip(row.Values)
                    .ToDictionary(col => col.First.ColumnName.Substring(6), val => val.Second),
            };
        }
    }
}
