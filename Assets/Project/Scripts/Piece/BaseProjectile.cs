using System.Collections;
using UnityEngine;

public class BaseProjectile : MonoBehaviour
{
    private float speed;
    private float minDistance = 0.1f;

    public System.Action OnHitTarget;
    
    public void GoToTarget(Vector3 targetPos, float _speed)
    {
        speed = _speed;
        StartCoroutine(Move(targetPos));
    }

    IEnumerator Move(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > minDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
        
        OnHitTarget?.Invoke();
        transform.position = target;
        Destroy(gameObject);
    }
}