using UnityEngine;

public abstract class NPC : MonoBehaviour
{
    public bool followingCommand = false;
    protected bool seeingTarget = false;
    [SerializeField] protected float seeingDistance = 10;
    [SerializeField] protected LayerMask obstacleLayer;

    protected Transform target;
    protected Vector3 eyePos;
    protected Vector3 targetAtEyeHeight;
    [SerializeField] protected float height = 1f;
    [HideInInspector] public Vector3 movement = new();
    protected readonly System.Random rand = new();
}