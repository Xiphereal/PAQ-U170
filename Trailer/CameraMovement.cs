using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField, Range(0.001f, 60f)]
    private float speed;

    void Update()
    {
        ChangeSpeed();
        Move();

        void ChangeSpeed()
        {
            if (Input.GetButtonDown("Fire1"))
                speed *= 1.05f;
            else if (Input.GetButtonDown("Fire3"))
                speed *= 0.95f;
        }
        void Move()
        {
            transform.Translate(Vector3.right * Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime);
        }
    }
}
