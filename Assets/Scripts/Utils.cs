using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public static class Utils
{
    public static void BindSingleton<T>(this DiContainer container, T instance)
    {
        container.Bind<T>().FromInstance(instance).AsSingle();
    }
}
