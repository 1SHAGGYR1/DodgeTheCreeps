using System;
using Godot;

namespace First2DGame.Extensions;

public static class GodotArrayExtensions
{
    public static int FindIndex<[MustBeVariant] T>(this Godot.Collections.Array<T> array, Predicate<T> predicate)
    {
        for (var index = 0; index < array.Count; index++)
        {
            if (predicate.Invoke(array[index]))
            {
                return index;
            }
        }
        
        return -1;
    }
}