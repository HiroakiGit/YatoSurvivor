using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackIndicator : MonoBehaviour
{
    [Header("Arrow")]
    public GameObject arrowPrefab; 
    public float distanceFromPlayer = 1f; 
    private GameObject arrowInstance;
    public float movementSpeed = 5f;
    [Header("Circle")]
    public GameObject circlePrefab;
    private GameObject circleInstance;

    void Start()
    {
        // 矢印のプレハブをプレイヤーの周りに生成する
        arrowInstance = Instantiate(arrowPrefab, transform.position, Quaternion.identity, transform);
        arrowInstance.transform.localPosition = new Vector3(0f, distanceFromPlayer, 0f); 

        circleInstance = Instantiate(circlePrefab, transform.position, Quaternion.identity, transform);
    }

    void LateUpdate()
    {
        // プレイヤーの位置を基準にしたマウスカーソルの方向を取得する
        Vector3 playerToMouse = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        float angle = Mathf.Atan2(playerToMouse.y, playerToMouse.x) * Mathf.Rad2Deg;

        // 矢印がマウスカーソルの方向を向くようにする
        arrowInstance.transform.rotation = Quaternion.Euler(0f, 0f, angle-90);

        // プレイヤーの周りを移動する
        Vector3 offset = Quaternion.Euler(0f, 0f, angle-90) * Vector3.up * distanceFromPlayer;
        Vector3 targetPosition = transform.position + offset;
        arrowInstance.transform.position = Vector3.MoveTowards(arrowInstance.transform.position, targetPosition, movementSpeed * Time.deltaTime);

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        circleInstance.transform.position = Vector2.MoveTowards(circleInstance.transform.position, mousePosition, 20 * Time.deltaTime);
    }
}
