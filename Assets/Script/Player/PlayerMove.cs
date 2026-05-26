using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public float speed = 5f;
    public float mouseSensitivity = 2f;
    public float gravity = -9.81f;
    public float terrainEditRadius = 5f;  // 編集範囲
    public float terrainEditStrength = 0.01f; // 編集強度
    private CharacterController controller;
    private float rotationY = 0f;
    private Vector3 velocity;
    private Terrain terrain;
    private TerrainData terrainData;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        terrain = Terrain.activeTerrain;
        terrainData = terrain.terrainData;
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float h = 0f;
        float v = 0f;

        if (keyboard.dKey.isPressed) h = 5f;
        if (keyboard.aKey.isPressed) h = -5f;
        if (keyboard.wKey.isPressed) v = 5f;
        if (keyboard.sKey.isPressed) v = -5f;

        var mouse = Mouse.current;
        if (mouse != null)
        {
            float mouseX = mouse.delta.x.ReadValue() * mouseSensitivity;
            rotationY += mouseX;
            transform.rotation = Quaternion.Euler(0, rotationY, 0);

            // 左クリックで盛り上げる
            //if (mouse.leftButton.isPressed)
            //    EditTerrain(transform.position, terrainEditStrength);

            // 右クリックでへこませる
            //if (mouse.rightButton.isPressed)
            //    EditTerrain(transform.position, -terrainEditStrength);
        }

        Vector3 move = transform.right * h + transform.forward * v;
        controller.Move(move * speed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void EditTerrain(Vector3 worldPos, float strength)
    {
        // ワールド座標をTerrain座標に変換
        Vector3 terrainPos = terrain.transform.position;
        int mapX = (int)((worldPos.x - terrainPos.x) / terrainData.size.x * terrainData.heightmapResolution);
        int mapZ = (int)((worldPos.z - terrainPos.z) / terrainData.size.z * terrainData.heightmapResolution);

        int radius = (int)(terrainEditRadius);
        int startX = Mathf.Clamp(mapX - radius, 0, terrainData.heightmapResolution - 1);
        int startZ = Mathf.Clamp(mapZ - radius, 0, terrainData.heightmapResolution - 1);
        int width = Mathf.Clamp(mapX + radius, 0, terrainData.heightmapResolution - 1) - startX;
        int height = Mathf.Clamp(mapZ + radius, 0, terrainData.heightmapResolution - 1) - startZ;

        float[,] heights = terrainData.GetHeights(startX, startZ, width, height);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                heights[z, x] += strength;
            }
        }

        terrainData.SetHeights(startX, startZ, heights);
    }
}