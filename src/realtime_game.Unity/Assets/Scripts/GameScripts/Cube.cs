using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(CharacterController))]
public class MoveOnSpline3D_Looping : MonoBehaviour
{
    public SplineContainer spline;
    public float moveSpeed = 3f;
    public float rotateSpeed = 10f;

    private CharacterController controller;
    private float t = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (spline == null || spline.Spline == null || spline.Spline.Count < 2)
            return; // スプライン未設定エラー防止

        float input = Input.GetAxis("Horizontal");

        // --- ループ対応 ---
        float delta = input * moveSpeed * Time.deltaTime;
        t = (t + delta) % 1f;
        if (t < 0) t += 1f;

        // --- 位置 ---
        Vector3 pos = spline.EvaluatePosition(t);

        // CharacterController の enable/disable は使わない（Editorバグ回避）
        transform.position = pos;

        // --- 向き（安全版 Tangent） ---
        Vector3 tangent = spline.EvaluateTangent(t);
        if (tangent.sqrMagnitude > 0.0001f) // Zero対策
        {
            Vector3 dir = tangent.normalized;
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }
    }
}
