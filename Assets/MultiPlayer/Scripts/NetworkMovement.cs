using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class NetworkMovement : MonoBehaviourPun, IPunObservable
{
    private Rigidbody _rb;
    private Queue<Snapshot> _snapshots = new Queue<Snapshot>();
    private Vector3 _velocity;
    private const double INTERPOLATION_DELAY = 0.05;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            _velocity = _rb.linearVelocity;
        }
        else
        {
            InterpolateRemote();
        }
    }

    private void InterpolateRemote()
    {
        if (_snapshots.Count < 2) return;

        double renderTime = PhotonNetwork.Time - INTERPOLATION_DELAY;

        Snapshot prev = _snapshots.First();
        Snapshot next = _snapshots.Last();

        foreach (var snap in _snapshots)
        {
            if (snap.Time <= renderTime)
                prev = snap;
            if (snap.Time >= renderTime)
            {
                next = snap;
                break;
            }
        }

        double dt = next.Time - prev.Time;

        if (dt > 0.0001)
        {
            float t = (float)((renderTime - prev.Time) / dt);
            transform.position = Vector3.Lerp(prev.Position, next.Position, t);
        }
        else
        {
            transform.position = next.Position;
        }
        if (renderTime > next.Time)
        {
            double extraTime = renderTime - next.Time;
            transform.position = next.Position + next.Velocity * (float)extraTime;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(_rb.linearVelocity);
            stream.SendNext(PhotonNetwork.Time);
        }
        else
        {
            Snapshot snap = new Snapshot
            {
                Position = (Vector3)stream.ReceiveNext(),
                Velocity = (Vector3)stream.ReceiveNext(),
                Time = (double)stream.ReceiveNext()
            };

            _snapshots.Enqueue(snap);

            while (_snapshots.Count > 50)
                _snapshots.Dequeue();
        }
    }
    private struct Snapshot
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public double Time;
    }
}
