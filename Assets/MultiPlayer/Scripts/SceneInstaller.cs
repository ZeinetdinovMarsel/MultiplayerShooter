using Photon.Pun;
using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 20;

        Container.Bind<IFactory<Vector3, Quaternion, GameObject>>()
            .To<CustomPlayerFactory>()
            .AsSingle();

    }
}
