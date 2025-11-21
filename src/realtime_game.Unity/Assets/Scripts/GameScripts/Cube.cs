using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class MoveOnSpline3D_Switch : MonoBehaviour
{
    public List<SplineContainer> splines; // すべてのスプライン
    public int currentSplineIndex = 0;     // 現在のスプライン
    public float moveSpeed = 3f;
    public float rotateSpeed = 10f;

    public float t = 0f;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (splines.Count == 0) return;

        var spline = splines[currentSplineIndex].Spline;
        if (spline.Count < 2) return;

        float input = Input.GetAxis("Horizontal");
        float delta = input * moveSpeed * Time.deltaTime;

        // ループ対応
        t = (t + delta) % 1f;
        if (t < 0) t += 1f;

        // 分岐判定：t が 0.9 を超えたら次のスプラインに切り替え
        if (t >= 0.5f)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                currentSplineIndex = 0;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                currentSplineIndex = 1;
            }   
                

            //t = 0f;
        }
    

        Vector3 pos = splines[currentSplineIndex].EvaluatePosition(t);
        transform.position = pos;

        Vector3 tangent = splines[currentSplineIndex].EvaluateTangent(t);
        if (tangent.sqrMagnitude > 0.0001f)
        {
            Vector3 dir = tangent.normalized;
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }
    }

    void SwitchSpline()
    {
        // 次のスプラインに切り替え（ループ）
        //currentSplineIndex = (currentSplineIndex + 1) % splines.Count;


    }
}
