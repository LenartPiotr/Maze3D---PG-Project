using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Serializable2DArray<T> : ISerializationCallbackReceiver
{
    private T[,] array;
    public Serializable2DArray(int x, int y)
    {
        array = new T[x, y];
        width = x;
        height = y;
    }

    [SerializeField, HideInInspector] private List<Element> serializable;
    [SerializeField, HideInInspector] private int width;
    [SerializeField, HideInInspector] private int height;

    [Serializable]
    struct Element
    {
        public int x;
        public int y;
        public T value;
        public Element(int x, int y, T value)
        {
            this.x = x;
            this.y = y;
            this.value = value;
        }
    }

    public void OnBeforeSerialize()
    {
        serializable = new List<Element>();
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                serializable.Add(new Element(i, j, array[i, j]));
            }
        }
    }

    public void OnAfterDeserialize()
    {
        array = new T[width, height];
        foreach (var package in serializable)
        {
            array[package.x, package.y] = package.value;
        }
    }

    public T this[int x, int y]
    {
        get => array[x, y];
        set => array[x, y] = value;
    }

    public int Width => width;
    public int Height => height;
}
