using System;

public class Heap<T> where T : IHeapItem<T> {
    private readonly T[] items;

    public Heap(int maxSize) {
        items = new T[maxSize];
    }

    public void Add(T item) {
        item.HeapIndex = Count;
        items[Count] = item;

        SortUp(item);
        Count++;
    }

    public T RemoveFirst() {
        var firstItem = items[0];
        Count--;
        items[0] = items[Count];
        items[0].HeapIndex = 0;
        SortDown(items[0]);

        return firstItem;
    }

    public void UpdateItem(T item) {
        SortUp(item);
    }

    public int Count { get; private set; }

    public bool Contains(T item) => Equals(items[item.HeapIndex], item);

    private void SortDown(T item) {
        while (true) {
            var childIndexLeft = item.HeapIndex * 2 + 1;
            var childIndexRight = item.HeapIndex * 2 + 2;

            if (childIndexLeft < Count) {
                var swapIndex = childIndexLeft;

                if (childIndexRight < Count) {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0) {
                        swapIndex = childIndexRight;
                    }
                }

                if (item.CompareTo(items[swapIndex]) < 0) {
                    Swap(item, items[swapIndex]);
                }
                else return;
            }
            else return;
        }
    }

    private void SortUp(T item) {
        var parentIndex = (item.HeapIndex - 1) / 2;
        while (true) {
            var parent = items[parentIndex];
            if (item.CompareTo(parent) > 0) {
                Swap(item, parent);
            }
            else {
                break;
            }

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    private void Swap(T a, T b) {
        items[a.HeapIndex] = b;
        items[b.HeapIndex] = a;

        var aIndex = a.HeapIndex;
        a.HeapIndex = b.HeapIndex;
        b.HeapIndex = aIndex;
    }
}

public interface IHeapItem<T> : IComparable<T> {
    int HeapIndex { get; set; }
}