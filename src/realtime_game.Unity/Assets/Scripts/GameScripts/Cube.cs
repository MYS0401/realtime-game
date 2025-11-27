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

    Rigidbody rb;

    public float t = 0;
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

        // 分岐判定
        if(currentSplineIndex == 0)
        {//白色上分岐地点
            if (Input.GetKeyUp(KeyCode.E))
            {
                if (t >= 0.06f && 0.07f >= t)
                {
                    currentSplineIndex = 1;
                    t = 0.91f;
                }

                if (t >= 0.182f && 0.186f >= t)
                {      
                    currentSplineIndex = 1;      
                    t = 0.192f;
                }

                if (t >= 0.558f && 0.564f >= t)
                {
                    currentSplineIndex = 1;
                    t = 0.412f;
                }

                if (t >= 0.682f && 0.687f >= t)
                {
                    currentSplineIndex = 1;
                    t = 0.694f;
                }
            }
        }
        else if (currentSplineIndex == 1)
        {//青線上分岐地点
            if (Input.GetKeyUp(KeyCode.E))
            {
                if (t >= 0.9f && 0.95f >= t)
                {       
                    currentSplineIndex = 0;           
                    t = 0.065f;
                }

                if (t >= 0.19f && 0.195f >= t)
                {               
                    currentSplineIndex = 0;
                    t = 0.184f;
                }

                if (t >= 0.41f && 0.415f >= t)
                {
                    currentSplineIndex = 0;
                    t = 0.56f;
                }

                if (t >= 0.692f && 0.696f >= t)
                {
                    currentSplineIndex = 0;
                    t = 0.684f;
                }
            }
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
