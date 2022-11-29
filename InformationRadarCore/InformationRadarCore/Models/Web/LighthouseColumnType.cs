using System.Text.RegularExpressions;

namespace InformationRadarCore.Models.Web
{
    public static class LighthouseColumnUtil
    {
        // Parses type from platform agnostic type string
        public static ILighthouseColumnType Parse(string s)
        {
            var match = Regex.Match(@"^(?<typename>[a-zA-Z]+)(?:\((?<length>[1-9]\d{0,3})\))?$", s);

            if (match == null || !match.Success)
            {
                throw new LighthouseColumnException($"Invalid type \"{s}\"");
            }

            var typename = match.Groups[1].Value;

            LighthouseSizedRecordFieldType sized;
            if (match.Groups.ContainsKey("length") && Enum.TryParse(typename, out sized))
            {
                var len = uint.Parse(match.Groups["length"].Value);

                if (sized == LighthouseSizedRecordFieldType.Float && (len == 0 || len > 53))
                {
                    throw new LighthouseColumnException("Float size out of bounds");
                }
                else if (len == 0 || len > 4000)
                {
                    throw new LighthouseColumnException("Char size out of bounds");
                }

                return new LighthouseSizedColumnType()
                {
                    Size = len,
                    Type = sized,
                };
            }

            LighthouseUnsizedRecordFieldType unsized;
            if (Enum.TryParse(typename, out unsized))
            {
                return new LighthouseUnsizedColumnType()
                {
                    Type = unsized,
                };
            }

            throw new LighthouseColumnException($"Could not parse type \"{s}\"");
        }

        public static IDictionary<string, ILighthouseColumnType> ProcessRawCols(IDictionary<string, string> raw)
        {
            if (raw == null || raw.Count() == 0)
            {
                throw new LighthouseColumnException("Empty columns!");
            }

            var columns = new Dictionary<string, ILighthouseColumnType>();

            foreach (var (key, value) in raw)
            {
                if (!Regex.IsMatch(key, @"^[a-zA-Z]\w{0,99}$"))
                {
                    throw new LighthouseColumnException(
                        "Column name \"" + key + "\" is invalid.  " +
                        "Column name must start with a letter and contain only " +
                        "word chars (alphanumeric chars and '_')");
                }

                columns.Add(key, Parse(value));
            }

            return columns;
        }
    }

    public class LighthouseColumnException : Exception 
    {
        public LighthouseColumnException(string s) : base("Error parsing column type: " + s) { }
    }
   
    public interface ILighthouseColumnType
    {
        // Converts the column type to a dialect specific SQL column type
        string SqlColumn();
        // Seralizes the column type to a generic-type syntax for cross-DB storage
        string Seralize();
    }

    public enum LighthouseUnsizedRecordFieldType
    {
        Int,
        Long,
        Small,
        Real,
        Text,
        Date,
    }

    public class LighthouseUnsizedColumnType : ILighthouseColumnType
    {
        public LighthouseUnsizedRecordFieldType Type { get; set; }

        public string Seralize() {
            var name = Enum.GetName<LighthouseUnsizedRecordFieldType>(Type);
            if (string.IsNullOrEmpty(name)) {
                throw new Exception("Null or out of range unsized type");
            }
            return name;
        }

        public string SqlColumn()
        {
            switch (Type)
            {
                case LighthouseUnsizedRecordFieldType.Int:
                    return "INT";
                case LighthouseUnsizedRecordFieldType.Long:
                    return "BIGINT";
                case LighthouseUnsizedRecordFieldType.Small:
                    return "SMALLINT";
                case LighthouseUnsizedRecordFieldType.Real:
                    return "REAL";
                case LighthouseUnsizedRecordFieldType.Text:
                    return "TEXT";
                case LighthouseUnsizedRecordFieldType.Date:
                    return "DATE";
            }

            throw new Exception("Unsupported unsized column type");
        }
    }

    public enum LighthouseSizedRecordFieldType
    {
        Float,
        Char,
        Varchar,
    }

    public class LighthouseSizedColumnType : ILighthouseColumnType
    {
        public LighthouseSizedRecordFieldType Type { get; set; }
        public uint Size { get; set; }

        public string Seralize()
        {
            var name = Enum.GetName<LighthouseSizedRecordFieldType>(Type);
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Null or out of range sized type");
            }
            return name + "(" + Size + ")";
        }

        public string SqlColumn()
        {
            switch (Type)
            {
                case LighthouseSizedRecordFieldType.Float:
                    return "FLOAT(" + Size + ")";
                case LighthouseSizedRecordFieldType.Char:
                    return "CHAR(" + Size + ")";
                case LighthouseSizedRecordFieldType.Varchar:
                    return "NVARCHAR(" + Size + ")";
            }

            throw new Exception("Unsupported sized column type");
        }
    }
}
