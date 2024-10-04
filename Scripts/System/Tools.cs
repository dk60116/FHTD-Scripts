using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

public class Tools
{
    static public T[] Shuffle<T>(T[] _array)
    {
        int _iRandom1, _iRandom2;
        T temp;

        for (int i = 0; i < _array.Length; ++i)
        {
            _iRandom1 = UnityEngine.Random.Range(0, _array.Length);
            _iRandom2 = UnityEngine.Random.Range(0, _array.Length);

            temp = _array[_iRandom1];
            _array[_iRandom1] = _array[_iRandom2];
            _array[_iRandom2] = temp;
        }

        return _array;
    }

    static public List<T> ShuffleList<T>(List<T> _list)
    {
        int _iRandom1, _iRandom2;
        T temp;

        for (int i = 0; i < _list.Count; ++i)
        {
            _iRandom1 = UnityEngine.Random.Range(0, _list.Count);
            _iRandom2 = UnityEngine.Random.Range(0, _list.Count);

            temp = _list[_iRandom1];
            _list[_iRandom1] = _list[_iRandom2];
            _list[_iRandom2] = temp;
        }

        return _list;
    }

    static public bool GetRandomForPercentResult(float _fValue)
    {
        float _fRandom = UnityEngine.Random.Range(0, 100f);

        return _fRandom < _fValue;
    }

    static public string GetMiddleString(string _strText, string _strBegin, string _strEnd)
    {
        if (string.IsNullOrEmpty(_strText))
            return string.Empty;

        string _strResult = string.Empty;

        if (_strText.IndexOf(_strText) > -1)
        {
            _strText = _strText.Substring(_strText.IndexOf(_strBegin) + _strBegin.Length);

            if (_strText.IndexOf(_strEnd) > -1)
                _strResult = _strText.Substring(0, _strText.IndexOf(_strEnd));
            else
                _strResult = _strText;
        }

        return _strResult;
    }

    public static float GetStringCalculator(string expression)
    {
        DataTable table = new DataTable();
        table.Columns.Add("expression", string.Empty.GetType(), expression);
        DataRow row = table.NewRow();
        table.Rows.Add(row);
        return float.Parse((string)row["expression"]);
    }

    static public int WordCountCheck(string _strText, string _strTarget)
    {
        MatchCollection _cMaches = Regex.Matches(_strText, _strTarget);

        return _cMaches.Count;
    }

    static public string GenerateRandomHex(int _length)
    {
        const string hexChars = "0123456789ABCDEF";
        char[] chars = new char[_length];
        System.Random random = new System.Random();

        for (int i = 0; i < _length; i++)
            chars[i] = hexChars[random.Next(hexChars.Length)];

        return new string(chars);
    }
}
