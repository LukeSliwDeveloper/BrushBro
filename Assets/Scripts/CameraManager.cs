using DG.Tweening;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public void Move()
    {
        transform.DOMove(new Vector3(0f, 8f, -4.5f), .5f);
        transform.DORotate(new Vector3(70f, 0, 0), .5f);
    }
}
