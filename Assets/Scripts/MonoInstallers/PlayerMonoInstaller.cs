using UnityEngine;
using Zenject;

public class PlayerMonoInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        EventManager eventManager = GetComponent<EventManager>();
        Container.BindSingleton(eventManager);

        PlayerManager playerManager = GetComponent<PlayerManager>();
        Container.BindSingleton(playerManager);
    }
}