using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    // Assign target directly from code when creating split-screen cameras
    public Transform target;
    // Optional tag to find target if not assigned (keeps existing behavior)
    public string playerTag = "Player";

    private void Update()
    {
        if (target == null)
        {
            if (!string.IsNullOrEmpty(playerTag))
            {
                GameObject player = GameObject.FindGameObjectWithTag(playerTag);
                if (player != null)
                    target = player.transform;
            }
            return;
        }

        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
    }
}