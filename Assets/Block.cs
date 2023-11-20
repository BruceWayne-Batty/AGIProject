using UnityEngine;

public class Block : MonoBehaviour
{
    public float fallSpeed = 5f; // 下落速度

    void Update()
    {
        CheckFall();
    }

    void CheckFall()
    {
        // 向下发射射线
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            // 如果下方没有碰撞体，说明需要下落
            if (hit.collider == null)
            {
                StartFalling();
            }
        }
    }

    public void StartFalling()
    {
        // 下落逻辑
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }
}