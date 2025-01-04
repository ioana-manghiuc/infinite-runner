using UnityEngine;

public class MoveLeft : MonoBehaviour
{
   public float speed = 10;
    void Start()
    {
        
    }

    
    void Update()
    {
        transform.Translate(Vector3.left * Time.deltaTime * speed);
    }
}
