namespace FotNET.DATA.CSV;

public struct DataConfig {
    public int StartRow;
    public int InputColumnStart;
    public int InputColumnEnd;
    public int OutputColumnStart;
    public int OutputColumnEnd;
    public string[] Delimiters;
}