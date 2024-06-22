using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    private float _duration = 0.5f;
    
    private void OnEnable()
    {
        PathManager.OnPathPositionCompleted += InitMovement;
    }

    private void OnDisable()
    {
        PathManager.OnPathPositionCompleted -= InitMovement;
    }

    private void InitMovement(List<GridData> path)
    {
        StartCoroutine(FollowPath(path));
    }
    
    private IEnumerator FollowPath(List<GridData> path)
    {
        return path.Select(nextGrid => StartCoroutine(Movement(nextGrid))).GetEnumerator();
    }

    private IEnumerator Movement(GridData gridData)
    {
        float elapsedTime = 0;
        
        while (elapsedTime < _duration)
        {
            transform.position = Vector3.Lerp(transform.position, gridData.Position, elapsedTime / _duration);;
            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }
}
