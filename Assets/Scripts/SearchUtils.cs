using System.Collections.Generic;

public interface IID2String
{
    string ID { get; }
}

public interface IID2Int
{
    int ID { get; }
}

public interface IID2Short
{
    short ID { get; }
}



public class SearchUtils
{
    public static T ChooseOne<T>(List<T> dataLst) => dataLst[UnityEngine.Random.Range(0, dataLst.Count)];

    #region Search By ID
    public static T BinarySearchByID<T>(List<T> DataLst, string searchID) where T : IID2String
    {
        int left = 0;
        int right = DataLst.Count - 1;
        while (left <= right)
        {
            int mid = (left + right) / 2;
            string tmpData = DataLst[mid].ID;
            if (tmpData.Equals(searchID))
            {
                return DataLst[mid];
            }
            else if (tmpData.CompareTo(searchID) < 0)
            {
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }

        return default(T);
    }

    public static T BinarySearchByID<T>(List<T> DataLst, int searchID) where T : IID2Int
    {
        int left = 0;
        int right = DataLst.Count - 1;
        while (left <= right)
        {
            int mid = (left + right) / 2;
            int tmpData = DataLst[mid].ID;
            if (tmpData.Equals(searchID))
            {
                return DataLst[mid];
            }
            else if (tmpData.CompareTo(searchID) < 0)
            {
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }

        return default(T);
    }

    public static T BinarySearchByID<T>(List<T> DataLst, short searchID) where T : IID2Short
    {
        int left = 0;
        int right = DataLst.Count - 1;
        while (left <= right)
        {
            int mid = (left + right) / 2;
            short tmpData = DataLst[mid].ID;
            if (tmpData.Equals(searchID))
            {
                return DataLst[mid];
            }
            else if (tmpData.CompareTo(searchID) < 0)
            {
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }

        return default(T);
    }
    #endregion

}
