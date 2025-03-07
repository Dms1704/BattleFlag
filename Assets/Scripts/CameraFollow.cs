using UnityEngine;
 
public class CameraFollow : MonoBehaviour
{
    public float followDistance = 10f; // 摄像机与目标之间的距离
    public float followSpeed = 5f; // 摄像机跟随目标的速度
    // public Vector2 cameraBoundsMin; // 摄像机可以移动的最小位置
    // public Vector2 cameraBoundsMax; // 摄像机可以移动的最大位置
    
    void LateUpdate()
    {
        Entity currentEntity = TurnOrderManager.instance.GetCurrentEntity();
        if (currentEntity)
        {
            // 计算目标位置与摄像机当前位置之间的向量
            Vector3 desiredPosition = currentEntity.transform.position + Vector3.back * followDistance;
 
            // 限制摄像机的位置在边界内
            // desiredPosition.x = Mathf.Clamp(desiredPosition.x, cameraBoundsMin.x, cameraBoundsMax.x);
            // desiredPosition.y = Mathf.Clamp(desiredPosition.y, cameraBoundsMin.y, cameraBoundsMax.y);
 
            // 平滑地移动摄像机到目标位置
            Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * followSpeed);
 
            // 更新摄像机的位置
            transform.position = new Vector3(smoothPosition.x, smoothPosition.y, transform.position.z);
        }
    }
}