using System.Text;

namespace TestTask;

public static class Compression
{
    public static string Compress(string str)
    {
            
        int count = 1;
        StringBuilder res = new StringBuilder();
        for (int i = 1; i < str.Length; i++)
        {
            if (str[i] ==  str[i - 1]) count++;
            else
            {
                res.Append(str[i - 1]);
                if (count > 1)
                {
                    res.Append(count);
                }

                count = 1;
            }
        }
        res.Append(str[str.Length - 1]);
        if  (count > 1) res.Append(count);
        return res.ToString();
    }

    public static string Decompress(string str)
    {
            
        StringBuilder res = new StringBuilder();

        for (int i = 0; i < str.Length; i++)
        {
            char current = str[i];
            int count = 0;

            while (i+1 < str.Length && char.IsDigit(str[i+1]))
            {
                count = count*10+(str[i+1]-'0');
                i++;
            }

            if (count == 0)
            {
                count = 1;
            }

            for (int j = 0; j < count; j++)
            {
                res.Append(current);
            }
        }
        return res.ToString();
    }
}