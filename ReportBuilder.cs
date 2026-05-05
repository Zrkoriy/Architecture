using System.Text;

class ReportBuilder
{
    private DatabaseManager _db;
    private string _sql = "";
    private string _title = "";
    private string[] _headers = new string[0];
    private int[] _widths = new int[0];
    private bool _numbered = false;

    public ReportBuilder(DatabaseManager db)
    {
        _db = db;
    }

    public ReportBuilder Query(string sql)
    {
        _sql = sql;
        return this;
    }

    public ReportBuilder Title(string title)
    {
        _title = title;
        return this;
    }

    public ReportBuilder Header(params string[] cols)
    {
        _headers = cols;
        return this;
    }

    public ReportBuilder ColumnWidths(params int[] widths)
    {
        _widths = widths;
        return this;
    }

    public ReportBuilder Numbered()
    {
        _numbered = true;
        return this;
    }

    public string Build()
    {
        var (cols, rows) = _db.ExecuteQuery(_sql);
        var sb = new StringBuilder();

        if (_title != "")
        {
            sb.AppendLine();
            sb.AppendLine($"=== {_title} ===");
        }

        string[] headers = _headers.Length > 0 ? _headers : cols;
        int colCount = headers.Length;

        int[] widths = new int[colCount];
        if (_widths.Length >= colCount)
        {
            for (int i = 0; i < colCount; i++) widths[i] = _widths[i];
        }
        else
        {
            for (int i = 0; i < colCount; i++) widths[i] = 20;
        }

        int numWidth = _numbered ? 5 : 0;

        if (_numbered)
            sb.Append("№".PadRight(numWidth));

        for (int i = 0; i < colCount; i++)
            sb.Append(headers[i].PadRight(widths[i]));
        sb.AppendLine();

        int totalWidth = numWidth;
        for (int i = 0; i < colCount; i++) totalWidth += widths[i];
        sb.AppendLine(new string('-', totalWidth));

        for (int r = 0; r < rows.Count; r++)
        {
            if (_numbered)
                sb.Append((r + 1).ToString().PadRight(numWidth));

            for (int c = 0; c < rows[r].Length && c < colCount; c++)
                sb.Append(rows[r][c].PadRight(widths[c]));
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public void Print()
    {
        Console.WriteLine(Build());
    }
}