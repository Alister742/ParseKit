namespace Core.ExternalTypes
{
    //public class SortedList<T> : ArrayList
    //{
        
    //    //int* ss = stackalloc int[100];

        

    //    List< elements = new ArrayList(11);
    //    //ArrayList elements = new ArrayList();
    //    IComparer<T> _comparer;
    //    public SortedList(IComparer<T> comparer)
    //    {
    //        _comparer = comparer;
    //    }

    //    public void Add(T element)
    //    {
    //        if (element == null)
    //            return;
	
    //        int range = elements.Count;
    //        //int stage = 1;
    //        int index = range / 2;//stage++;

    //        while (true)
    //        {
    //            int step = index / 2;
    //            if (step == 0)
    //                step = 1;

    //            if (_comparer.Compare((T)elements[index], element) > 0)
    //            {
    //                index -= step;
    //            }
    //            else if (_comparer.Compare((T)elements[index], element) < 0)
    //            {
    //                index += step;
    //            }
    //            else
    //                break;
    //        }
    //        elements.Insert(index, element);
    //    }

    //    public void AddRange(ICollection<T> elements)
    //    {
    //        foreach (var item in elements)
    //        {
    //            this.Add(item);
    //        }
    //    }

    //    public void SwitchElements(ref T first, ref T second)
    //    {
    //        T temp = first;
    //        first = second;
    //        second = temp;
    //    }
    //}
}
