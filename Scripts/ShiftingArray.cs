using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// This Array allows new content to be added either to the top or the bottom
// without shifting every slot up or down by using a pointer to its head
public class ShiftingArray<T>
{
    private int size;
    private int head;
    private T[] data;

    public ShiftingArray(int size)
    {
        this.size = size;
        head = 0;
        data = new T[size];
    }

    public int getSize()
    {
        return this.size;
    }

    private void reduceHead()
    {
        head = head == 0 ? size - 1 : head - 1;
    }

    public void set(int index, T newData)
    {
        int trueIndex = (head + index) % this.size;
        data[trueIndex] = newData;
    }

    public T get(int index)
    {
        if (index >= size)
        {
            throw new IndexOutOfRangeException();
        }
        int trueIndex = (head + index) % size;
        return data[trueIndex];
    }

    // TODO: Ist das getestet?
    public void addToTop(T newData)
    {
        int trueIndex = head == 0 ? size - 1 : head - 1;
        data[trueIndex] = newData;
    }

    public void addToBot(T newData)
    {
        reduceHead();
        data[head] = newData;
    }
}
