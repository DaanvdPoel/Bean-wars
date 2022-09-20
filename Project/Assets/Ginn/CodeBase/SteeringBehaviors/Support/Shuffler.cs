using System.Collections.Generic;

public static class Shuffler
{
    /// <summary>
    /// Shuffle the items in a list into a random order.
    /// </summary>
    /// <typeparam name="T">The type of the list.</typeparam>
    /// <param name="list">The list itself.</param>
    /// <param name="rnd">The randomizer.</param>
    public static void Shuffle<T>(this IList<T> list, System.Random rnd)
    {
        for (var i = list.Count; i > 0; i--)
            list.Swap(0, rnd.Next(0, i));
    }

    /// <summary>
    /// Swap two items in a list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">The list itself.</param>
    /// <param name="i">The first index to be swapped with the second index.</param>
    /// <param name="j">The second index to be swapped with the first index.</param>
    public static void Swap<T>(this IList<T> list, int i, int j)
    {
        var temp = list[i];
        list[i] = list[j];
        list[j] = temp;
    }
}